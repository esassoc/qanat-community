using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.Common.Services.GDAL;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.Geography;
using Qanat.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies")]
public class GeographyController : SitkaController<GeographyController>
{
    private readonly GeographyGISBoundaryService _geographyGISBoundaryService;
    private readonly GDALAPIService _gdalApiService;
    private readonly FileService _fileService;

    public GeographyController(QanatDbContext dbContext, ILogger<GeographyController> logger, IOptions<QanatConfiguration> qanatConfiguration, GeographyGISBoundaryService geographyGISBoundaryService, GDALAPIService gdalApiService, FileService fileService)
        : base(dbContext, logger, qanatConfiguration)
    {
        _geographyGISBoundaryService = geographyGISBoundaryService;
        _gdalApiService = gdalApiService;
        _fileService = fileService;
    }

    #region Read/Update

    [HttpGet]
    [WithRolePermission(PermissionEnum.GeographyRights, RightsEnum.Read)]
    public ActionResult<List<GeographyDto>> List()
    {
        var geographyList = Geographies.ListAsDto(_dbContext);
        return Ok(geographyList);
    }

    [HttpGet("{geographyID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRolePermission(PermissionEnum.GeographyRights, RightsEnum.Read)]
    public ActionResult<GeographyDto> GetGeographyByID([FromRoute] int geographyID)
    {
        var geographyDto = Geographies.GetByIDAsDto(_dbContext, geographyID);
        return Ok(geographyDto);
    }

    [HttpPut("{geographyID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRolePermission(PermissionEnum.GeographyRights, RightsEnum.Update)]
    public ActionResult UpdateGeography([FromRoute] int geographyID, [FromBody] GeographyForAdminEditorsDto requestDto)
    {
        var errors = Geographies.ValidateGeographyUpdate(_dbContext, requestDto);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Geographies.UpdateBasicGeographyFields(_dbContext, geographyID, requestDto);

        return Ok();
    }

    #endregion
        
    [HttpGet("geography-name/{geographyName}/for-admin-editor")]
    [WithRolePermission(PermissionEnum.GeographyRights, RightsEnum.Read)]
    public ActionResult<GeographyForAdminEditorsDto> GetByGeographyNameForAdminEditor([FromRoute] string geographyName)
    {
        var geography = _dbContext.Geographies.AsNoTracking().SingleOrDefault(x => x.GeographyName == geographyName);
        if (geography == null)
        {
            return NotFound();
        }

        var adminGeographyUpdateRequestDto = Geographies.GetAsAdminGeographyUpdateRequestDto(_dbContext, geography.GeographyID);
        return Ok(adminGeographyUpdateRequestDto);
    }

    [HttpGet("geography-name/{geographyName}/minimal")]
    [WithRolePermission(PermissionEnum.GeographyRights, RightsEnum.Read)]
    public async Task<ActionResult<GeographyMinimalDto>> GetByNameAsMinimalDto([FromRoute] string geographyName)
    {
        var geography = await Geographies.GetByNameAsMinimalDtoAsync(_dbContext, geographyName);
        if (geography == null)
        {
            return NotFound();
        }

        return Ok(geography);
    }

    [HttpGet("current-user")]
    [AuthenticatedWithUser]
    public ActionResult<List<GeographyMinimalDto>> ListForCurrentUser()
    {
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        var waterAccounts = WaterAccounts.ListByUserAsMinimalDtos(_dbContext, user);
        var geographyIDs = waterAccounts.Select(x => x.GeographyID).Distinct().ToList();
        var geographies = Geographies.ListByIDsAsGeographyMinimalDto(_dbContext, geographyIDs);
        return Ok(geographies);
    }

    [HttpGet("{geographyID}/effectiveYears")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public ActionResult<List<int>> GetEffectiveYearsForGeography([FromRoute] int geographyID)
    {
        var startYear = WaterAccountParcels.GetNextAvailableEffectiveYearForGeography(_dbContext, geographyID);
        var maxYear = DateTime.UtcNow.Year + 1;
        var years = Enumerable.Range(startYear, maxYear - startYear + 1).ToList();
        return Ok(years);
    }

    [HttpPost("{geographyID}/upload-parcel-gdb")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [RequestSizeLimit(10L * 1024L * 1024L * 1024L)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10L * 1024L * 1024L * 1024L)]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Create)]
    public async Task<ActionResult> UploadGDBAndParseFeatureClasses([FromForm] UploadedGdbRequestDto uploadedGdbRequestDto, [FromRoute] int geographyID)
    {
        if (ParcelHistories.GeographyHasUnreviewedParcels(_dbContext, geographyID))
        {
            return BadRequest("This geography has unreviewed parcel changes. Please review all current changes before uploading any new parcel data.");
        }

        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

        // save the gdb file contents to UploadedGdb so user doesn't have to wait for upload of file again
        var uploadedGdb = await UploadedGdbs.CreateNew(_dbContext, user.UserID, geographyID);
        await using (var stream = uploadedGdbRequestDto.File.OpenReadStream())
        {
            await _fileService.SaveFileStreamToAzureBlobStorage(uploadedGdb.CanonicalName, stream);
        }

        var ogrInfoRequestDto = new OgrInfoRequestDto { BlobContainer = FileService.FileContainerName, CanonicalName = uploadedGdb.CanonicalName };
        var srid = await _gdalApiService.OgrInfoGdbGetSRID(ogrInfoRequestDto);

        uploadedGdb.SRID = srid;
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPut("{geographyID}/water-managers")]
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

    [HttpPut("gsa-boundaries")]
    [WithRolePermission(PermissionEnum.GeographyRights, RightsEnum.Update)]
    public async Task<ActionResult<List<GeographyBoundarySimpleDto>>> RefreshGSABoundaries()
    {
        try
        {
            await _geographyGISBoundaryService.RefreshGeographyGSABoundaries();
            var geographyBoundarySimpleDtos = _dbContext.GeographyBoundaries.AsNoTracking().Select(x => x.AsSimpleDto()).ToList();
            return Ok(geographyBoundarySimpleDtos);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }

}