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
using Qanat.Models.DataTransferObjects.User;
using Qanat.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies")]
public class GeographyController(
    QanatDbContext dbContext,
    ILogger<GeographyController> logger,
    IOptions<QanatConfiguration> qanatConfiguration,
    GeographyGISBoundaryService geographyGisBoundaryService,
    GDALAPIService gdalApiService,
    FileService fileService, UserDto callingUser)
    : SitkaController<GeographyController>(dbContext, logger, qanatConfiguration)
{
    private readonly GDALAPIService _gdalApiService = gdalApiService;
    private readonly FileService _fileService = fileService;

    #region Read/UpdateAsync

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

    [HttpPut("{geographyID}/self-reporting")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRolePermission(PermissionEnum.GeographyRights, RightsEnum.Update)]
    public async Task<ActionResult<GeographyMinimalDto>> UpdateGeographySelfReporting([FromRoute] int geographyID, [FromBody] GeographySelfReportEnabledUpdateDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var geography = await Geographies.UpdateGeographySelfReportingAsync(_dbContext, geographyID, updateDto);
        return Ok(geography);
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
        callingUser.Flags.TryGetValue(Flag.IsSystemAdmin.FlagName, out var isAdmin);
        if (isAdmin)
        {
            var geographiesForAdmin = Geographies.ListAsGeographyMinimalDto(_dbContext);
            return Ok(geographiesForAdmin);
        }

        var geographyIDsWhereUserIsManager = callingUser.GeographyFlags.Where(x => x.Value[Flag.HasManagerDashboard.FlagName]).Select(x => x.Key);
        var waterAccounts = WaterAccounts.ListByUserAsMinimalDtos(_dbContext, callingUser);
        var geographyIDs = waterAccounts.Select(x => x.GeographyID).Union(geographyIDsWhereUserIsManager).Distinct().ToList();
        var geographies = Geographies.ListByIDsAsGeographyMinimalDto(_dbContext, geographyIDs);
        return Ok(geographies);
    }

    [HttpPut("{geographyID}/water-managers")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public ActionResult<GeographyDto> EditGeographyWaterManagers([FromBody] List<GeographyWaterManagerDto> geographyWaterManagerDtos, [FromRoute] int geographyID)
    {
        var errors = Geographies.ValidateUpdateGeographyWaterManagers(_dbContext, geographyID, geographyWaterManagerDtos);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var geography = Geographies.UpdateGeographyWaterManagers(_dbContext, geographyID, geographyWaterManagerDtos);
        return Ok(geography);
    }

    [HttpGet("{geographyID}/landing-page")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public ActionResult<GeographyLandingPageDto> GetNumberOfWellsAndParcelsRegisteredToUser([FromRoute] int geographyID)
    {
        var wellRegistrationCount = Users.GetNumberOfWellRegistrationsForUser(_dbContext, callingUser.UserID, geographyID);
        var waterAccountCount = Users.GetNumberOfWaterAccountForUser(_dbContext, callingUser.UserID, geographyID);
        return Ok(new GeographyLandingPageDto()
        {
            NumberOfWaterAccounts = waterAccountCount,
            NumberOfWellRegistrations = wellRegistrationCount
        });
    }


    [HttpGet("{geographyID}/users/{userID}/water-accounts")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public ActionResult ListWaterAccountsOwnedByCurrentUser([FromRoute] int geographyID, [FromRoute] int userID)
    {
        var waterAccountDtos = WaterAccounts.ListByGeographyIDAndUserIDAsWaterAccountRequestChangesDto(_dbContext, geographyID, userID);

        return Ok(waterAccountDtos);
    }

    [HttpPut("{geographyID}/users/{userID}/water-accounts")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public async Task<ActionResult> UpdateWaterAccountsOwnedByCurrentUser([FromRoute] int geographyID, [FromRoute] int userID, [FromBody] WaterAccountParcelsRequestChangesDto requestDto)
    {
        var errors = WaterAccounts.ValidateRequestedWaterAccountChanges(_dbContext, geographyID, userID, requestDto);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await WaterAccounts.ApplyRequestedWaterAccountChanges(_dbContext, geographyID, userID, requestDto, callingUser);
        return Ok();
    }

    [HttpPut("gsa-boundaries")]
    [WithRolePermission(PermissionEnum.GeographyRights, RightsEnum.Update)]
    public async Task<ActionResult<List<GeographyBoundarySimpleDto>>> RefreshGSABoundaries()
    {
        try
        {
            await geographyGisBoundaryService.RefreshGeographyGSABoundaries();
            var geographyBoundarySimpleDtos = _dbContext.GeographyBoundaries.AsNoTracking().Select(x => x.AsSimpleDto()).ToList();
            return Ok(geographyBoundarySimpleDtos);
        }
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}