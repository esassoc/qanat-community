using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.API.Services.OpenET;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class OpenETSyncController : SitkaController<OpenETSyncController>
{
    private readonly OpenETSyncService _openETSyncService;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly FileService _fileService;
    private readonly RasterProcessingService _rasterProcessingService;

    public OpenETSyncController(QanatDbContext dbContext, ILogger<OpenETSyncController> logger, IOptions<QanatConfiguration> qanatConfiguration, OpenETSyncService openETSyncService, IBackgroundJobClient backgroundJobClient, FileService fileService, RasterProcessingService rasterProcessingService) : base(dbContext, logger, qanatConfiguration)
    {
        _openETSyncService = openETSyncService;
        _backgroundJobClient = backgroundJobClient;
        _fileService = fileService;
        _rasterProcessingService = rasterProcessingService;
    }

    [HttpGet("geographies/{geographyID}/open-et-syncs")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<List<OpenETSyncDto>> ListOpenETSyncs([FromRoute] int geographyID)
    {
        var openETSyncDtos = OpenETSyncs.ListByGeographyID(_dbContext, geographyID);
        return Ok(openETSyncDtos);
    }

    [HttpPost("geographies/{geographyID}/open-et-syncs")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Update)]
    public ActionResult<HangfireBackgroundJobResultDto> QueueOpenETSync([FromRoute] int geographyID, [FromBody] OpenETRunDto openETRunDto)
    {
        var geography = Geographies.GetByID(_dbContext, geographyID);

        var latestSyncHistory = _dbContext.OpenETSyncHistories.AsNoTracking()
            .Include(x => x.OpenETSync)
            .Where(x => x.OpenETSync.Year == openETRunDto.Year
                     && x.OpenETSync.Month == openETRunDto.Month
                     && x.OpenETSync.GeographyID == geographyID
                     && x.OpenETSync.OpenETDataTypeID == openETRunDto.OpenETDataTypeID)
            .OrderByDescending(x => x.CreateDate)
            .FirstOrDefault();

        var syncInProgress = latestSyncHistory is { OpenETSyncResultTypeID: (int)OpenETSyncResultTypeEnum.Created or (int)OpenETSyncResultTypeEnum.InProgress };
        if (syncInProgress)
        {
            return BadRequest("Sync already in progress.");
        }

        var openETSyncHistory = OpenETSyncHistories.CreateNew(_dbContext, openETRunDto.Year, openETRunDto.Month, openETRunDto.OpenETDataTypeID, geography.GeographyID);

        var backgroundJobID = _backgroundJobClient.Enqueue(() => _openETSyncService.SyncOpenETRasterCompositeForGeographyYearMonthDataTypeID(geographyID, openETRunDto.Year, openETRunDto.Month, openETRunDto.OpenETDataTypeID, openETSyncHistory.OpenETSyncHistoryID));
        var result = new HangfireBackgroundJobResultDto()
        {
            BackgroundJobID = backgroundJobID
        };

        return Ok(result);
    }

    [HttpPut("geographies/{geographyID}/open-et-syncs/{openETSyncID}/calculate")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Update)]
    public async Task<ActionResult> CalculateRasterForOpenETSync([FromRoute] int geographyID, int openETSyncID)
    {
        var geography = Geographies.GetByID(_dbContext, geographyID);

        var openETSync = OpenETSyncs.GetByID(_dbContext, openETSyncID);
        if (openETSync == null)
        {
            return NotFound();
        }

        var latestHistory = openETSync.OpenETSyncHistories.MaxBy(x => x.CreateDate);
        if (latestHistory == null || latestHistory.OpenETSyncResultTypeID != OpenETSyncResultType.Succeeded.OpenETSyncResultTypeID || !latestHistory.RasterFileResourceID.HasValue)
        {
            return BadRequest("The latest sync must be successful to calculate the raster.");
        }

        var waterMeasurementTypes = WaterMeasurementTypes.ListAsSimpleDto(_dbContext, geographyID).Where(x => x.IsActive);
        var waterMeasurementType = waterMeasurementTypes.FirstOrDefault(x => x.WaterMeasurementTypeName == $"OpenET {openETSync.OpenETDataType.OpenETDataTypeName}"); //MK 9/11/2024 I'd like a better way to match to the raster datatype but this will do for now.
        if (waterMeasurementType == null)
        {
            return BadRequest("The water measurement type for the OpenET data type must be active.");
        }

        var reportedDate = openETSync.ReportedDate.AddMonths(1).AddDays(-1);

        var geographyAsDto = geography.AsDto();

        //MK 9/19/2024 -- Assumes OpenET always returns Inches, somewhat of a smell.
        var backgroundJobID = _backgroundJobClient.Enqueue(() => _rasterProcessingService.ProcessRasterByFileCanonicalNameForAllUsageEntities(geographyAsDto, latestHistory.OpenETSyncHistoryID, waterMeasurementType.WaterMeasurementTypeID, UnitType.Inches.UnitTypeID, reportedDate, latestHistory.RasterFileResource.FileResourceCanonicalName, false, false));

        var continuedWithJobID = _backgroundJobClient.ContinueJobWith(backgroundJobID, () => _rasterProcessingService.RunCalculations(geographyID, waterMeasurementType.WaterMeasurementTypeID, reportedDate));

        var result = new HangfireBackgroundJobResultDto()
        {
            BackgroundJobID = backgroundJobID,
            ContinuedWithJobID = continuedWithJobID
        };

        return Ok(result);
    }

    [HttpPut("geographies/{geographyID}/open-et-syncs/{openETSyncID}/finalize")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Update)]
    public ActionResult<OpenETSyncDto> FinalizeOpenETSync([FromRoute] int geographyID, int openETSyncID)
    {
        var openETSync = OpenETSyncs.GetByID(_dbContext, openETSyncID);
        var latestHistory = openETSync.OpenETSyncHistories.MaxBy(x => x.CreateDate);

        if (latestHistory.OpenETRasterCalculationResultTypeID != OpenETRasterCalculationResultType.Succeeded.OpenETRasterCalculationResultTypeID)
        {
            return BadRequest("The raster calculation must be successful to finalize the sync.");
        }

        var updatedETSync = OpenETSyncs.FinalizeSyncByID(_dbContext, openETSyncID);
        return Ok(updatedETSync);
    }
}