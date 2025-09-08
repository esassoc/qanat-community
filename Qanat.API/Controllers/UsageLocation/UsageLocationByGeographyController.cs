using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.Common.GeoSpatial;
using Qanat.Common.Services.GDAL;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/usage-locations")]
public class UsageLocationByGeographyController(QanatDbContext dbContext, ILogger<UsageLocationByGeographyController> logger, IOptions<QanatConfiguration> qanatConfiguration, GDALAPIService gdalApiService, FileService fileService, UserDto callingUser)
    : SitkaController<UsageLocationByGeographyController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet("reporting-periods/{reportingPeriodID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public async Task<ActionResult<List<UsageLocationByReportingPeriodIndexGridDto>>> ListByGeographyAndReportingPeriod([FromRoute] int geographyID, [FromRoute] int reportingPeriodID)
    {
        var usageLocations = await UsageLocations.ListByGeographyAndReportingPeriodForUserAsync(dbContext, geographyID, reportingPeriodID, callingUser);
        return Ok(usageLocations);
    }

    [HttpPost("reporting-periods/{reportingPeriodID}/replace-from-parcels")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [WithGeographyRolePermission(PermissionEnum.UsageLocationRights, RightsEnum.Update)]
    public async Task<ActionResult<List<UsageLocationSimpleDto>>> ReplaceFromParcels([FromRoute] int geographyID, [FromRoute] int reportingPeriodID)
    {
        var errors = await UsageLocations.ValidateReplaceFromParcelsAsync(dbContext, geographyID, reportingPeriodID);
        errors.ForEach(e => ModelState.AddModelError(e.Type, e.Message));

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newUsageLocations = await UsageLocations.ReplaceFromParcelsAsync(dbContext, geographyID, reportingPeriodID);
        return Ok(newUsageLocations);
    }

    [HttpPost("upload-gdb")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.UsageLocationRights, RightsEnum.Update)]
    public async Task<ActionResult> UploadGDB([FromRoute] int geographyID, [FromForm] UploadedGdbRequestDto uploadedGdbRequestDto)
    {
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        var fileResource = await fileService.CreateFileResource(_dbContext, uploadedGdbRequestDto.File, user.UserID);
        return Ok();
    }

    [HttpPost("upload-gdb/columns")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.UsageLocationRights, RightsEnum.Update)]
    public async Task<ActionResult<GDBColumnsDto>> UploadGDBAndParseFeatureClassesForColumns([FromRoute] int geographyID)
    {
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        var file = dbContext.FileResources.Where(x => x.CreateUserID == user.UserID && x.OriginalFileExtension == "zip")
            .OrderByDescending(x => x.CreateDate).First();
        var ogrInfoRequestDto = new OgrInfoRequestDto
        {
            BlobContainer = FileService.FileContainerName,
            CanonicalName = file.FileResourceCanonicalName
        };
        // save the gdb file contents to UploadedGdb so user doesn't have to wait for upload of file again
        try
        {
            var featureClassInfos = await gdalApiService.OgrInfoGdbToFeatureClassInfo(ogrInfoRequestDto);

            var uploadReferenceLayerInfoDto = new GDBColumnsDto
            {
                FileResourceID = file.FileResourceID,
                FeatureClasses = featureClassInfos
            };

            return Ok(uploadReferenceLayerInfoDto);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return BadRequest("Error reading GDB file!");
        }
    }

    [HttpPost("reporting-periods/{reportingPeriodID}/upload-gdb/{isReplace}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [WithGeographyRolePermission(PermissionEnum.UsageLocationRights, RightsEnum.Update)]
    public async Task<ActionResult<UploadUsageLocationGdbResponseDto>> UploadGDBAndParseFeatureClasses([FromRoute] int geographyID, [FromRoute] int reportingPeriodID, [FromRoute] bool isReplace, [FromBody] List<ColumnMappingDto> columnMappingDtos)
    {
        var geography = _dbContext.Geographies.AsNoTracking().Single(x => x.GeographyID == geographyID);
        var fileResource = dbContext.FileResources.Where(x => x.CreateUserID == callingUser.UserID && x.OriginalFileExtension == "zip")
            .OrderByDescending(x => x.CreateDate).First();
        var ogrInfoRequestDto = new OgrInfoRequestDto
        {
            BlobContainer = FileService.FileContainerName,
            CanonicalName = fileResource.FileResourceCanonicalName
        };
        var srid = await gdalApiService.OgrInfoGdbGetSRID(ogrInfoRequestDto);
        var sourceWkt = await gdalApiService.OgrInfoGdbGetSRIDWKT(ogrInfoRequestDto);
        var geographyCoordinateSystemWkt = await gdalApiService.GdalSrsInfoGetWktForCoordinateSystem(geography.CoordinateSystem);
        var featureClassInfos = await gdalApiService.OgrInfoGdbToFeatureClassInfo(ogrInfoRequestDto);
        var columnsWithValues = columnMappingDtos.Where(x => !string.IsNullOrEmpty(x.SourceColumnName))
            .OrderBy(x => x.SourceColumnName).Select(x => $"{x.SourceColumnName} as {x.DestinationColumnName}")
            .ToList();

        var columnsWithoutValues = columnMappingDtos.Where(x => string.IsNullOrEmpty(x.SourceColumnName))
            .Select(x => $"NULL as {x.DestinationColumnName}")
            .ToList();

        var columns = columnsWithValues.Concat(columnsWithoutValues).ToList();

        var request = new GdbToGeoJsonRequestDto()
        {
            BlobContainer = FileService.FileContainerName,
            CanonicalName = fileResource.FileResourceCanonicalName,
            GdbLayerOutputs = featureClassInfos.Select(x => new GdbLayerOutput()
            {
                Columns = columns,
                FeatureLayerName = x.LayerName,
                NumberOfSignificantDigits = 4,
                Filter = "",
                CoordinateSystemID = srid,
                Extent = null
            }).ToList()
        };

        var features = await gdalApiService.Ogr2OgrGdbToGeoJson(request);
        try
        {
            var fromFeatureCollection = await GeoJsonSerializer.DeserializeFromFeatureCollection<UsageLocationGdbFeature>(features, GeoJsonSerializer.CreateGeoJSONSerializerOptions(14, 4), srid);
            var issues = await UsageLocations.ProcessUsageLocationGDBUpload(_dbContext, geographyID, reportingPeriodID, fromFeatureCollection, sourceWkt, geographyCoordinateSystemWkt, isReplace, callingUser);
            var response = new UploadUsageLocationGdbResponseDto()
            {
                Messages = issues
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to process GDB upload");
            var messages = new List<AlertMessageDto>
            {
                new()
                {
                    AlertMessageType = AlertMessageTypeEnum.Error,
                    Message = e.Message
                }
            };

            var response = new UploadUsageLocationGdbResponseDto()
            {
                Messages = messages
            };
            return response;
        }
    }

    [HttpPut("reporting-periods/{reportingPeriodID}/bulk-update-usage-location-type")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [WithGeographyRolePermission(PermissionEnum.UsageLocationRights, RightsEnum.Update)]
    public async Task<ActionResult<UsageLocationDto>> BulkUpdateUsageLocationType([FromRoute] int geographyID, [FromRoute] int reportingPeriodID, [FromBody] UsageLocationBulkUpdateUsageLocationTypeDto updateDto)
    {
        var errors = await UsageLocations.ValidateBulkUpdateUsageLocationTypeAsync(_dbContext, geographyID, updateDto);
        errors.ForEach(e => ModelState.AddModelError(e.Type, e.Message));
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedUsageLocationDtos = await UsageLocations.BulkUpdateUsageLocationTypeAsync(_dbContext, geographyID, reportingPeriodID, updateDto, callingUser);
        return Ok(updatedUsageLocationDtos);
    }
}