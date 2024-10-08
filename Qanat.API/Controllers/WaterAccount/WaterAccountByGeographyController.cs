using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/water-accounts")]
public class WaterAccountByGeographyController : SitkaController<WaterAccountByGeographyController>
{
    private readonly HierarchyContext _hierarchyContext;

    public WaterAccountByGeographyController(QanatDbContext dbContext, ILogger<WaterAccountByGeographyController> logger,
        IOptions<QanatConfiguration> qanatConfiguration, HierarchyContext hierarchyContext) : base(dbContext, logger, qanatConfiguration)
    {
        _hierarchyContext = hierarchyContext;
    }

    [HttpPost]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Create)]
    public async Task<ActionResult<WaterAccountDto>> CreateWaterAccount([FromRoute] int geographyID,
        [FromBody] WaterAccountCreateDto waterAccountDto)
    {
        var errors = WaterAccounts.ValidateWaterAccountName(_dbContext, geographyID, waterAccountDto.WaterAccountName);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var waterAccount = await WaterAccounts.CreateWaterAccount(_dbContext, geographyID, waterAccountDto);
        var waterAccountSimpleDto = waterAccount.AsSimpleDto();
        return Ok(waterAccountSimpleDto);
    }

    [HttpGet("current-user")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public ActionResult<List<WaterAccountIndexGridDto>> List([FromRoute] int geographyID)
    {
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        
        var hasPermission = new WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read).HasPermission(user, _hierarchyContext);
        var waterAccountIndexGridDtos = hasPermission 
            ? WaterAccounts.ListByGeographyIDAsIndexGridDtos(_dbContext, geographyID) 
            : WaterAccounts.ListByGeographyIDAndUserIDAsIndexGridDtos(_dbContext, geographyID, user.UserID);

        return Ok(waterAccountIndexGridDtos);
    }

    [HttpGet("budget-reports/years/{year}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public ActionResult<List<WaterAccountBudgetReportDto>> GetWaterAccountsBudget([FromRoute] int geographyID, [FromRoute] int year)
    {
        var reportingPeriod = ReportingPeriods.GetByGeographyID(_dbContext, geographyID);
        if (reportingPeriod == null)
        {
            return BadRequest(
                ("There are no reporting periods for this geography. Please make one in the configure dashboard."));
        }

        var fReportingPeriod = ReportingPeriods.GetByGeographyIDAndYear(_dbContext, geographyID, year);
        var startDate = fReportingPeriod.StartDate;
        var endDate = fReportingPeriod.EndDate;

        var waterAccountBudgetReport =
            WaterAccountBudgetReportByGeographyAndYear.ListByGeographyAndEffectiveDate(_dbContext, geographyID, startDate, endDate);

        return Ok(waterAccountBudgetReport);
    }

    [HttpPost("suggested/create")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Create)]
    public async Task<ActionResult<WaterAccountMinimalDto>> CreateWaterAccountFromSuggestion([FromRoute] int geographyID,
        [FromBody] CreateWaterAccountFromSuggestionDto dto)
    {
        // might want to validate if parcelIDs posted in the same geography
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        var waterAccountMinimalDto = await WaterAccounts.CreateWaterAccountFromSuggestion(_dbContext, geographyID, dto, user.UserID);
        return Ok(waterAccountMinimalDto);
    }

    [HttpPost("suggested/bulk-create")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Create)]
    public async Task<IActionResult> BulkCreateWaterAccountFromSuggestion([FromRoute] int geographyID,
        [FromBody] List<CreateWaterAccountFromSuggestionDto> dtos)
    {
        // might want to validate if parcelIDs posted in the same geography
        foreach (var dto in dtos)
        {
            await WaterAccounts.CreateWaterAccountFromSuggestion(_dbContext, geographyID, dto, 77);
        }
        return Ok();
    }

    [HttpPost("suggested/reject")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Create)]
    public async Task<IActionResult> RejectWaterAccountSuggestions([FromRoute] int geographyID,
        [FromBody] List<int> parcelIDs)
    {
        // might want to validate if parcelIDs posted in the same geography
        var parcels = _dbContext.Parcels.Where(x => parcelIDs.Contains(x.ParcelID)).ToList();
        foreach (var parcel in parcels)
        {
            parcel.WaterAccountID = null;
            parcel.ParcelStatusID = (int)ParcelStatusEnum.Excluded;
        }

        var waterAccountParcels = _dbContext.WaterAccountParcels.Where(x => parcelIDs.Contains(x.ParcelID) && x.EndYear != null).ToList();
        foreach (var waterAccountParcel in waterAccountParcels)
        {
            waterAccountParcel.EndYear = DateTime.UtcNow.Year; // do we need this to be passed in?
        }
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("suggested")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Create)]
    public ActionResult<List<WaterAccountSuggestionDto>> ListSuggested([FromRoute] int geographyID)
    {
        return Ok(WaterAccountSuggestions.ListByGeographyAsDto(_dbContext, geographyID));
    }

    [HttpGet("water-account-holders")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public ActionResult<List<UserDto>> ListWaterAccountHoldersByGeographyID([FromRoute] int geographyID)
    {
        var userDtos = _dbContext.WaterAccountUsers.AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.WaterAccount)
            .Where(x => x.WaterAccount.GeographyID == geographyID &&
                        x.WaterAccountRoleID == WaterAccountRole.WaterAccountHolder.WaterAccountRoleID)
            .AsEnumerable().DistinctBy(x => x.UserID)
            .Select(x => x.User.AsUserDto()).AsEnumerable()
            .OrderBy(x => x.LastName).ToList();

        return Ok(userDtos);
    }
}