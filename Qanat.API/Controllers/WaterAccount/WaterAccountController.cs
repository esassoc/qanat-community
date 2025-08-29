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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("water-accounts")]
public class WaterAccountController(QanatDbContext dbContext, ILogger<WaterAccountController> logger, IOptions<QanatConfiguration> qanatConfiguration, [FromServices] UserDto callingUser)
    : SitkaController<WaterAccountController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet("{waterAccountID}/minimal")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public ActionResult<WaterAccountMinimalDto> GetWaterAccountByIDMinimal([FromRoute] int waterAccountID)
    {
        var waterAccountMinimalDto = WaterAccounts.GetByIDAsMinimalDto(_dbContext, waterAccountID);
        return Ok(waterAccountMinimalDto);
    }

    [HttpGet("{waterAccountID}")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public ActionResult<WaterAccountDto> GetByID([FromRoute] int waterAccountID)
    {
        var waterAccountDto = WaterAccounts.GetByIDAsDto(_dbContext, waterAccountID);
        return Ok(waterAccountDto);
    }

    [HttpGet("{waterAccountID}/reporting-periods/{reportingPeriodID}")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public ActionResult<WaterAccountDto> GetByIDAndReportingPeriodID([FromRoute] int waterAccountID, [FromRoute] int reportingPeriodID)
    {
        var waterAccountDto = WaterAccounts.GetByIDAsDto(_dbContext, waterAccountID, reportingPeriodID);
        return Ok(waterAccountDto);
    }

    [HttpPut("{waterAccountID}")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public ActionResult<WaterAccountDto> Update([FromRoute] int waterAccountID, [FromBody] WaterAccountUpdateDto waterAccountUpdateDto)
    {
        var waterAccountDto = WaterAccounts.UpdateWaterAccount(_dbContext, waterAccountID, waterAccountUpdateDto);
        return Ok(waterAccountDto);
    }

    [HttpGet("{waterAccountID}/water-account-contact")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public async Task<ActionResult<WaterAccountContactDto>> GetWaterAccountContactByWaterAccountID([FromRoute] int waterAccountID)
    {
        var waterAccountContactDto = await WaterAccountContacts.GetByWaterAccountIDAsDto(_dbContext, waterAccountID);
        return Ok(waterAccountContactDto);
    }

    [HttpPut("{waterAccountID}/water-account-contact")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Update)]
    public ActionResult<WaterAccountDto> UpdateWaterAccountContact([FromRoute] int waterAccountID, [FromBody] WaterAccountWaterAccountContactUpdateDto waterAccountWaterAccountUpdateDto)
    {
        if (waterAccountWaterAccountUpdateDto.WaterAccountContactID.HasValue)
        {
            var errors = WaterAccounts.ValidateUpdateWaterAccountContact(_dbContext, waterAccountID, waterAccountWaterAccountUpdateDto.WaterAccountContactID.Value);
            errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
        }

        var waterAccountDto = WaterAccounts.UpdateWaterAccountContact(_dbContext, waterAccountID, waterAccountWaterAccountUpdateDto.WaterAccountContactID);
        return Ok(waterAccountDto);
    }

    [HttpDelete("{waterAccountID}")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Delete)]
    public async Task<ActionResult> Delete([FromRoute] int waterAccountID)
    {
        await WaterAccounts.DeleteWaterAccount(_dbContext, waterAccountID, callingUser);
        return Ok();
    }

    [HttpPut("{waterAccountID}/merge/{secondaryWaterAccountID}")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [EntityNotFound(typeof(WaterAccount), "secondaryWaterAccountID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Update)]
    public async Task<ActionResult<WaterAccountMinimalDto>> Merge([FromRoute] int waterAccountID, [FromRoute] int secondaryWaterAccountID, [FromBody] MergeWaterAccountsDto mergeDto)
    {
        var errors = await WaterAccounts.ValidateMergeWaterAccounts(_dbContext, waterAccountID, secondaryWaterAccountID, mergeDto, callingUser);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var primaryAccount = await WaterAccounts.MergeWaterAccounts(_dbContext, waterAccountID, secondaryWaterAccountID, mergeDto, callingUser);
        return Ok(primaryAccount);
    }

    [HttpGet("{waterAccountID}/water-budget-stats/years/{year}")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public async Task<ActionResult<WaterAccountBudgetStatDto>> GetWaterMeasurementStatsForWaterBudget([FromRoute] int waterAccountID, [FromRoute] int year)
    {
        var waterAccount = WaterAccounts.GetByIDAsMinimalDto(_dbContext, waterAccountID);
        var budgetStats = await WaterMeasurements.GetWaterMeasurementStatsForWaterBudget(_dbContext, waterAccount.Geography.GeographyID, waterAccountID, year, callingUser);
        return Ok(budgetStats);
    }


    [HttpGet("{waterAccountID}/water-type-monthly-supply/years/{year}")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Read)]
    public ActionResult<List<WaterAccountWaterTypeMonthlySupplyDto>> GetWaterTypeSupplyFromAccountID([FromRoute] int waterAccountID, [FromRoute] int year)
    {
        var waterAccountTotalWaterTypeSupplyDtos = WaterTypeMonthlySupplies.ListByYearAndWaterAccount(_dbContext, year, waterAccountID);
        return Ok(waterAccountTotalWaterTypeSupplyDtos);
    }

    [HttpGet("{waterAccountID}/parcel-supplies/years/{year}/monthly-usage-summary")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Read)]
    public ActionResult<List<WaterAccountParcelWaterMeasurementDto>> GetMonthlyUsageSummaryForWaterAccountAndReportingPeriod([FromRoute] int waterAccountID, [FromRoute] int year)
    {
        var userID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext).UserID;
        var waterAccountParcelWaterMeasurementDtos = MonthlyUsageSummary.ListByWaterAccountAndYear(_dbContext, waterAccountID, year, userID);
        return Ok(waterAccountParcelWaterMeasurementDtos);
    }

    [HttpGet("{waterAccountID}/recent-effective-dates/years/{year}")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Read)]
    public ActionResult<MostRecentEffectiveDatesDto> GetMostRecentSupplyAndUsageDateByAccountID([FromRoute] int waterAccountID, int year)
    {
        var mostRecentEffectiveDates = WaterAccountMostRecentEffectiveDate.GetMostRecentEffectiveDatesByWaterAccount(_dbContext, waterAccountID, year);
        return Ok(mostRecentEffectiveDates);
    }

    [HttpGet("{waterAccountID}/allocation-plans")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.AllocationPlanRights, RightsEnum.Read)]
    public ActionResult<List<AllocationPlanMinimalDto>> GetAccountAllocationPlansByAccountID([FromRoute] int waterAccountID)
    {
        var waterAccountDisplayDto = WaterAccounts.GetByIDAsDisplayDto(_dbContext, waterAccountID);

        var geographyAllocationPlan = GeographyAllocationPlanConfigurations.GetByGeographyID(_dbContext, waterAccountDisplayDto.GeographyID);
        if (geographyAllocationPlan == null)
        {
            return Ok(new List<AllocationPlanMinimalDto>());
        }

        var parcelIDs = Parcels.ListByWaterAccountID(_dbContext, waterAccountID).Select(x => x.ParcelID).ToList();

        var parcelZoneIDs = _dbContext.ParcelZones
            .Include(x => x.Zone).AsNoTracking()
            .Where(x => parcelIDs.Contains(x.ParcelID) && x.Zone.ZoneGroupID == geographyAllocationPlan.ZoneGroupID)
            .Select(x => x.ZoneID).ToList();

        var allocationPlans =
            AllocationPlans.ListByGeographyIDAndZoneIDsAsSimpleDto(_dbContext, waterAccountDisplayDto.GeographyID, parcelZoneIDs);

        return allocationPlans;
    }

    [HttpGet("current-user")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public ActionResult<List<WaterAccountMinimalDto>> ListByCurrentUser()
    {
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

        var waterAccountMinimalDtos = WaterAccounts.ListByUserAsMinimalDtos(_dbContext, user);

        return Ok(waterAccountMinimalDtos);
    }
}