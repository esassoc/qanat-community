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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/open-et-syncs")]
public class OpenETSyncController(QanatDbContext dbContext, ILogger<OpenETSyncController> logger, IOptions<QanatConfiguration> qanatConfiguration, OpenETSyncService openETSyncService, IBackgroundJobClient backgroundJobClient, FileService fileService, RasterProcessingService rasterProcessingService)
    : SitkaController<OpenETSyncController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<List<OpenETSyncDto>> ListOpenETSyncs([FromRoute] int geographyID)
    {
        var openETSyncDtos = OpenETSyncs.ListByGeographyID(_dbContext, geographyID);
        return Ok(openETSyncDtos);
    }

    [HttpPost]
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

        var backgroundJobID = backgroundJobClient.Enqueue(() => openETSyncService.SyncOpenETRasterCompositeForGeographyYearMonthDataTypeID(geographyID, openETRunDto.Year, openETRunDto.Month, openETRunDto.OpenETDataTypeID, openETSyncHistory.OpenETSyncHistoryID));
        var result = new HangfireBackgroundJobResultDto()
        {
            BackgroundJobID = backgroundJobID
        };

        return Ok(result);
    }

    [HttpPut("{openETSyncID}/calculate")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(OpenETSync), "openETSyncID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Update)]
    public async Task<ActionResult<HangfireBackgroundJobResultDto>> CalculateRasterForOpenETSync([FromRoute] int geographyID, [FromRoute] int openETSyncID, [FromBody] RecalculateRasterDto recalculateRasterDto)
    {
        var openETSync = OpenETSyncs.GetByID(_dbContext, openETSyncID);

        var latestHistory = openETSync.OpenETSyncHistories.MaxBy(x => x.CreateDate);
        if (latestHistory == null || latestHistory.OpenETSyncResultTypeID != OpenETSyncResultType.Succeeded.OpenETSyncResultTypeID || !latestHistory.RasterFileResourceID.HasValue)
        {
            return BadRequest("The latest sync must be successful to calculate the raster.");
        }

        var waterMeasurementTypes = WaterMeasurementTypes.ListAsSimpleDto(_dbContext, geographyID).Where(x => x.IsActive);
        var waterMeasurementType = waterMeasurementTypes.FirstOrDefault(x => x.WaterMeasurementTypeName == $"OpenET {openETSync.OpenETDataType.OpenETDataTypeName}"); //MK 9/11/2024: I'd like a better way to match to the raster datatype but this will do for now.
        if (waterMeasurementType == null)
        {
            return BadRequest("The Water Measurement Type for the OpenET data type must be active.");
        }

        var reportedDate = openETSync.ReportedDate.AddMonths(1).AddDays(-1);

        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsSimpleDtoAsync(_dbContext, geographyID, reportedDate.Year);
        if (reportingPeriod == null)
        {
            return BadRequest("The Reporting Period for the reported date must exist.");
        }

        var usageLocations = await UsageLocations.ListByGeographyAndReportingPeriodAsync(_dbContext, geographyID, reportingPeriod.ReportingPeriodID);
        if (usageLocations.Count == 0)
        {
            return BadRequest("There must be Usage Locations for the Geography and Reporting Period to run calculations.");
        }

        var geography = Geographies.GetByID(_dbContext, geographyID);
        var geographyAsDto = geography.AsDto();

        //MK 9/19/2024 -- Assumes OpenET always returns Inches, somewhat of a smell.
        var backgroundJobID = backgroundJobClient.Enqueue(() => rasterProcessingService.ProcessRasterByFileCanonicalNameForUsageLocations(geographyAsDto, recalculateRasterDto.UsageLocationIDs == null ? latestHistory.OpenETSyncHistoryID : null, waterMeasurementType.WaterMeasurementTypeID, UnitType.Inches.UnitTypeID, reportedDate, latestHistory.RasterFileResource.FileResourceCanonicalName, recalculateRasterDto.UsageLocationIDs, false, false));

        var continuedWithJobID = backgroundJobClient.ContinueJobWith(backgroundJobID, () => rasterProcessingService.RunCalculations(geographyID, waterMeasurementType.WaterMeasurementTypeID, reportedDate, recalculateRasterDto.UsageLocationIDs));

        var result = new HangfireBackgroundJobResultDto()
        {
            BackgroundJobID = backgroundJobID,
            ContinuedWithJobID = continuedWithJobID
        };

        return Ok(result);
    }

    [HttpPut("{openETSyncID}/finalize")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(OpenETSync), "openETSyncID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Update)]
    public ActionResult<OpenETSyncDto> FinalizeOpenETSync([FromRoute] int geographyID, [FromRoute] int openETSyncID)
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

    [HttpDelete("{openETSyncID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(OpenETSync), "openETSyncID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Delete)]
    public async Task<ActionResult> DeleteHistoriesAndFileResources([FromRoute] int geographyID, [FromRoute] int openETSyncID)
    {
        var openETSync = OpenETSyncs.GetByID(_dbContext, openETSyncID);

        var waterMeasurementTypes = WaterMeasurementTypes.ListAsSimpleDto(_dbContext, geographyID).Where(x => x.IsActive);
        var waterMeasurementType = waterMeasurementTypes.FirstOrDefault(x => x.WaterMeasurementTypeName == $"OpenET {openETSync.OpenETDataType.OpenETDataTypeName}"); //MK 4/23/2025: I'd still like a better way to match to the raster datatype but this will do for now.

        var fileResourceCanonicalNames = await OpenETSyncs.DeleteHistoriesAndFileResourcesAsync(_dbContext, openETSync, waterMeasurementType?.WaterMeasurementTypeID);

        fileResourceCanonicalNames.ForEach(fileService.DeleteFileStreamFromBlobStorage);

        return NoContent();
    }
}