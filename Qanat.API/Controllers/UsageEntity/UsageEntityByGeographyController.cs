using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/usage-entities")]
public class UsageEntityByGeographyController(
    QanatDbContext dbContext,
    ILogger<UsageEntityByGeographyController> logger,
    IOptions<QanatConfiguration> qanatConfiguration,
    GDALAPIService gdalApiService,
    FileService fileService)
    : SitkaController<UsageEntityByGeographyController>(dbContext, logger, qanatConfiguration)
{
    [HttpPost]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRolePermission(PermissionEnum.UsageEntityRights, RightsEnum.Update)]
    public async Task<ActionResult<UploadUsageEntityGdbResponseDto>> UploadGDBAndParseFeatureClasses(
        [FromForm] UploadedGdbRequestDto uploadedGdbRequestDto, [FromRoute] int geographyID)
    {
        if (geographyID != 7) // todo: remove this hard-coded check
        {
            return BadRequest(
                "This endpoint currently only works for the ETSGSA geography. The code running this needs to be abstracted for other geographies if used there.");
        }

        var geography = _dbContext.Geographies.Single(x => x.GeographyID == geographyID);
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        var fileResource = await fileService.CreateFileResource(_dbContext, uploadedGdbRequestDto.File, user.UserID);
        var ogrInfoRequestDto = new OgrInfoRequestDto
            { BlobContainer = FileService.FileContainerName, CanonicalName = fileResource.FileResourceCanonicalName };
        var srid = await gdalApiService.OgrInfoGdbGetSRID(ogrInfoRequestDto);
        var sourceWkt = await gdalApiService.OgrInfoGdbGetSRIDWKT(ogrInfoRequestDto);
        var geographyCoordinateSystemWkt =
            await gdalApiService.GdalSrsInfoGetWktForCoordinateSystem(geography.CoordinateSystem);
        var featureClassInfos = await gdalApiService.OgrInfoGdbToFeatureClassInfo(ogrInfoRequestDto);

        var request = new GdbToGeoJsonRequestDto()
        {
            BlobContainer = FileService.FileContainerName,
            CanonicalName = fileResource.FileResourceCanonicalName,
            GdbLayerOutputs = featureClassInfos.Select(x => new GdbLayerOutput()
            {
                Columns = x.Columns,
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
            switch (geographyID)
            {
                case 7:
                    var fromFeatureCollection =
                        await GeoJsonSerializer.DeserializeFromFeatureCollection<ETSGSAUsageEntityGdbFeature>(features,
                            GeoJsonSerializer.CreateGeoJSONSerializerOptions(14, 4), srid);
                    var issues = await UsageEntities.ProcessETSGSAUsageEntityGDBUpload(_dbContext,
                        fromFeatureCollection, sourceWkt, geographyCoordinateSystemWkt);

                    var messages = new List<AlertMessageDto>();
                    if (!string.IsNullOrEmpty(issues))
                    {
                        messages.Add(new AlertMessageDto()
                            { AlertMessageType = AlertMessageTypeEnum.Warn, Message = issues });
                    }

                    messages.Add(new AlertMessageDto()
                    {
                        AlertMessageType = AlertMessageTypeEnum.Success,
                        Message = "Successfully uploaded Usage Entities"
                    });
                    var response = new UploadUsageEntityGdbResponseDto()
                    {
                        Messages = messages
                    };
                    return Ok(response);
                default:
                    throw new Exception("This geography is not supported.");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to process GDB upload");
            throw;
        }
    }
}