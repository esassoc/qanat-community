using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
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
public class GeographyController : SitkaController<GeographyController>
{
    private readonly GeographyGISBoundaryService _geographyGISBoundaryService;
    private readonly GDALAPIService _gdalApiService;
    private readonly FileService _fileService;

    public GeographyController(QanatDbContext dbContext, ILogger<GeographyController> logger,
        IOptions<QanatConfiguration> qanatConfiguration, GeographyGISBoundaryService geographyGISBoundaryService, 
        GDALAPIService gdalApiService, FileService fileService)
        : base(dbContext, logger, qanatConfiguration)
    {
        _geographyGISBoundaryService = geographyGISBoundaryService;
        _gdalApiService = gdalApiService;
        _fileService = fileService;
    }

    [HttpGet("public/geographies")]
    [AllowAnonymous]
    public ActionResult<List<GeographyDto>> List()
    {
        var geographyList = Geographies.ListAsDto(_dbContext);
        return Ok(geographyList);
    }

    [HttpGet("geographies/{geographyID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRolePermission(PermissionEnum.GeographyRights, RightsEnum.Read)]
    public ActionResult<GeographyDto> GetGeographyByID([FromRoute] int geographyID)
    {
        var geographyDto = Geographies.GetByIDAsDto(_dbContext, geographyID);
        return Ok(geographyDto);
    }

    [HttpGet("public/geography/name/{geographyName}")]
    [AllowAnonymous] // this is used on public geography dashboard
    public ActionResult<GeographyDto> GetGeographyByName([FromRoute] string geographyName)
    {
        var geographyDto = Geographies.GetByNameAsDto(_dbContext, geographyName);
        if (geographyDto == null)
        {
            return NotFound();
        }

        return geographyDto;
    }

    [HttpGet("public/geography/boundingBox/{geographyName}")]
    [AllowAnonymous] // this is used on public geography dashboard
    public ActionResult<GeographyWithBoundingBoxDto> GetGeographyByNameWithBoundingBox([FromRoute] string geographyName)
    {
        var geography = _dbContext.Geographies.Include(x => x.GeographyBoundary).AsNoTracking()
            .SingleOrDefault(x => x.GeographyName == geographyName);
        if (geography == null) return NotFound(geographyName);
        var geographyWithBoundingBoxDto = new GeographyWithBoundingBoxDto()
        {
            GeographyID = geography.GeographyID,
            GeographyName = geography.GeographyName,
            GeographyDisplayName = geography.GeographyDisplayName,
            BoundingBox = new BoundingBoxDto(new List<Geometry>() { geography.GeographyBoundary.BoundingBox })
        };
        return Ok(geographyWithBoundingBoxDto);
    }

    [HttpGet("public/geographyBoundaries")]
    [AllowAnonymous]
    public ActionResult<List<GeographyBoundarySimpleDto>> ListBoundaries()
    {
        var geographyBoundarySimpleDtos =
            _dbContext.GeographyBoundaries.AsNoTracking().Select(x => x.AsSimpleDto()).ToList();
        return Ok(geographyBoundarySimpleDtos);
    }

    [HttpGet("geography/{geographyName}/edit")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRolePermission(PermissionEnum.GeographyRights, RightsEnum.Read)]
    public ActionResult<AdminGeographyUpdateRequestDto> GetGeographyForAdminEditor([FromRoute] string geographyName)
    {
        var geography = _dbContext.Geographies.AsNoTracking().SingleOrDefault(x => x.GeographyName == geographyName);
        if (geography == null)
        {
            return NotFound();
        }

        var adminGeographyUpdateRequestDto =
            Geographies.GetAsAdminGeographyUpdateRequestDto(_dbContext, geography.GeographyID);
        return Ok(adminGeographyUpdateRequestDto);
    }


    [HttpPut("geographies/{geographyID}/edit")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRolePermission(PermissionEnum.GeographyRights, RightsEnum.Update)]
    public ActionResult UpdateGeography([FromRoute] int geographyID,
        [FromBody] AdminGeographyUpdateRequestDto requestDto)
    {
        var errors = Geographies.ValidateGeographyUpdate(requestDto);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Geographies.UpdateBasicGeographyFields(_dbContext, geographyID, requestDto);

        return Ok();
    }


    [HttpPut("geographies/{geographyID}/edit-water-managers")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public ActionResult<GeographyDto> EditGeographyWaterManagers([FromBody] List<UserDto> userDetailedDtos, [FromRoute] int geographyID)
    {
        var errors = Geographies.ValidateUpdateGeographyWaterManagers(_dbContext, geographyID, userDetailedDtos);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var geography = Geographies.UpdateGeographyWaterManagers(_dbContext, geographyID, userDetailedDtos);
        return Ok(geography);
    }

    [HttpGet("user/{userID}/permissions")]
    [WithRolePermission(PermissionEnum.GeographyRights, RightsEnum.Read)]
    public ActionResult<List<GeographyUserDto>> GetGeographyPermissionsForUser([FromRoute] int userID)
    {
        var permissions = _dbContext.GeographyUsers
            .Include(x => x.Geography)
            .Include(x => x.User)
            .Where(x => x.UserID == userID)
            .OrderBy(x => x.Geography.GeographyDisplayName)
            .Select(x => x.AsGeographyUserDto())
            .ToList();

        return Ok(permissions);
    }

    [HttpPut("geographies/gsa-boundaries")]
    [WithRolePermission(PermissionEnum.GeographyRights, RightsEnum.Update)]
    public async Task<ActionResult<List<GeographyBoundarySimpleDto>>> RefreshGSABoundaries()
    {
        try
        {
            await _geographyGISBoundaryService.RefreshGeographyGSABoundaries();
            var geographyBoundarySimpleDtos =
                _dbContext.GeographyBoundaries.AsNoTracking().Select(x => x.AsSimpleDto()).ToList();
            return Ok(geographyBoundarySimpleDtos);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

    [HttpPost("geographies/{geographyID}/upload-usage-entity-gdb")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRolePermission(PermissionEnum.UsageEntityRights, RightsEnum.Update)]
    public async Task<ActionResult<UploadUsageEntityGdbResponseDto>> UploadGDBAndParseFeatureClasses(
        [FromForm] UploadedGdbRequestDto uploadedGdbRequestDto, [FromRoute] int geographyID)
    {
        if (geographyID != 7) // todo: remove this hard-coded check
        {
            return BadRequest("This endpoint currently only works for the ETSGSA geography. The code running this needs to be abstracted for other geographies if used there.");
        }
        var geography = _dbContext.Geographies.Single(x => x.GeographyID == geographyID);
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        var fileResource = await _fileService.CreateFileResource(_dbContext, uploadedGdbRequestDto.File, user.UserID);
        var ogrInfoRequestDto = new OgrInfoRequestDto
            { BlobContainer = FileService.FileContainerName, CanonicalName = fileResource.FileResourceCanonicalName };
        var srid = await _gdalApiService.OgrInfoGdbGetSRID(ogrInfoRequestDto);
        var sourceWkt = await _gdalApiService.OgrInfoGdbGetSRIDWKT(ogrInfoRequestDto);
        var geographyCoordinateSystemWkt = await _gdalApiService.GdalSrsInfoGetWktForCoordinateSystem(geography.CoordinateSystem);
        var featureClassInfos = await _gdalApiService.OgrInfoGdbToFeatureClassInfo(ogrInfoRequestDto);
        
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

        var features = await _gdalApiService.Ogr2OgrGdbToGeoJson(request);

        try
        {
            switch (geographyID)
            {
                case 7:
                    var fromFeatureCollection = await GeoJsonSerializer.DeserializeFromFeatureCollection<ETSGSAUsageEntityGdbFeature>(features,
                        GeoJsonSerializer.CreateGeoJSONSerializerOptions(14, 4), srid);
                    var issues = await UsageEntities.ProcessETSGSAUsageEntityGDBUpload(_dbContext, fromFeatureCollection, sourceWkt, geographyCoordinateSystemWkt);

                    var messages = new List<AlertMessageDto>();
                    if (!string.IsNullOrEmpty(issues))
                    {
                        messages.Add(new AlertMessageDto() { AlertMessageType = AlertMessageTypeEnum.Warn, Message = issues });
                    }
                    messages.Add(new AlertMessageDto() { AlertMessageType = AlertMessageTypeEnum.Success, Message = "Successfully uploaded Usage Entities"});
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

    // todo: discuss with H2O devs
    // How do we want to handle basic logged in user permissions? [Authorize] vs. new flag
    // How do we feel about direct role references in contexts like this?
    [HttpGet("geographies/current-user")]
    [Authorize]
    public ActionResult<List<GeographySimpleDto>> ListWaterAccountGeographiesByCurrentUser()
    {
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

        var waterAccounts = WaterAccounts.ListByUserAsMinimalDtos(_dbContext, user);
        var geographySimpleDtos = waterAccounts.Select(x => x.Geography).DistinctBy(x => x.GeographyID).ToList();
        
        return Ok(geographySimpleDtos);
    }
}