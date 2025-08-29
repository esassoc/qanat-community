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
public class WaterAccountByGeographyController(QanatDbContext dbContext, ILogger<WaterAccountByGeographyController> logger, IOptions<QanatConfiguration> qanatConfiguration, HierarchyContext hierarchyContext, [FromServices] UserDto callingUser)
    : SitkaController<WaterAccountByGeographyController>(dbContext, logger, qanatConfiguration)
{
    [HttpPost]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Create)]
    public async Task<ActionResult<WaterAccountSimpleDto>> CreateWaterAccount([FromRoute] int geographyID, [FromBody] WaterAccountUpsertDto waterAccountUpsertDto)
    {
        var errors = WaterAccounts.ValidateWaterAccountName(_dbContext, geographyID, waterAccountUpsertDto.WaterAccountName);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var waterAccount = await WaterAccounts.CreateWaterAccount(_dbContext, geographyID, waterAccountUpsertDto);
        var waterAccountSimpleDto = waterAccount.AsSimpleDto();
        return Ok(waterAccountSimpleDto);
    }

    [HttpGet("current-user")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public async Task<ActionResult<List<WaterAccountIndexGridDto>>> ListByCurrentUser([FromRoute] int geographyID)
    {
        var hasPermission = new WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read).HasPermission(callingUser, hierarchyContext);
        var waterAccountIndexGridDtos = hasPermission
            ? await WaterAccounts.ListByGeographyIDAsIndexGridDtos(_dbContext, geographyID)
            : await WaterAccounts.ListByGeographyIDAndUserIDAsIndexGridDtos(_dbContext, geographyID, callingUser.UserID);

        return Ok(waterAccountIndexGridDtos);
    }

    [HttpGet("years/{year}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public async Task<ActionResult<List<WaterAccountIndexGridDto>>> ListByGeographyIDAndYear([FromRoute] int geographyID, [FromRoute] int year)
    {
        var waterAccountIndexGridDtos = await WaterAccounts.ListByGeographyIDAndYearAsIndexGridDtos(_dbContext, geographyID, year);
        return Ok(waterAccountIndexGridDtos);
    }

    [HttpGet("budget-reports/years/{year}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public async Task<ActionResult<List<WaterAccountBudgetReportDto>>> GetWaterAccountsBudget([FromRoute] int geographyID, [FromRoute] int year)
    {
        var reportingPeriodDto = await ReportingPeriods.GetByGeographyIDAndYearAsync(_dbContext, geographyID, year, callingUser);
        if (reportingPeriodDto == null)
        {
            var geography = await Geographies.GetByIDAsMinimalDtoAsync(_dbContext, geographyID);
            return BadRequest($"No reporting period found for the year {year} for {geography.GeographyDisplayName}.");
        }

        var waterAccountBudgetReport = WaterAccountBudgetReportByGeographyAndReportingPeriod.ListByGeographyAndReportingPeriod(_dbContext, geographyID, reportingPeriodDto);
        return Ok(waterAccountBudgetReport);
    }

    [HttpPost("suggested/create")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Create)]
    public async Task<ActionResult<WaterAccountMinimalDto>> CreateWaterAccountFromSuggestion([FromRoute] int geographyID, [FromBody] CreateWaterAccountFromSuggestionDto dto)
    {
        // might want to validate if parcelIDs posted in the same geography
        var waterAccountMinimalDto = await WaterAccounts.CreateWaterAccountFromSuggestion(_dbContext, geographyID, dto, callingUser);
        return Ok(waterAccountMinimalDto);
    }

    [HttpPost("suggested/bulk-create")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Create)]
    public async Task<IActionResult> BulkCreateWaterAccountFromSuggestion([FromRoute] int geographyID, [FromBody] List<CreateWaterAccountFromSuggestionDto> dtos)
    {
        // might want to validate if parcelIDs posted in the same geography
        foreach (var dto in dtos)
        {
            await WaterAccounts.CreateWaterAccountFromSuggestion(_dbContext, geographyID, dto, callingUser);
        }

        return Ok();
    }

    [HttpPost("suggested/reject")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Create)]
    public async Task<IActionResult> RejectWaterAccountSuggestions([FromRoute] int geographyID, [FromBody] List<int> parcelIDs)
    {
        // might want to validate if parcelIDs posted in the same geography
        var parcels = _dbContext.Parcels.Where(x => parcelIDs.Contains(x.ParcelID)).ToList();
        foreach (var parcel in parcels)
        {
            parcel.WaterAccountID = null;
            parcel.ParcelStatusID = (int)ParcelStatusEnum.Excluded;
            var parcelHistory = ParcelHistories.CreateNew(parcel, callingUser.UserID);
            await _dbContext.ParcelHistories.AddAsync(parcelHistory);
        }

        await _dbContext.SaveChangesAsync();

        var parcelIDsToReview = parcels.Select(x => x.ParcelID).ToList();
        await ParcelHistories.MarkAsReviewedByParcelIDsAsync(_dbContext, parcelIDsToReview);

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
            .Where(x => x.WaterAccount.GeographyID == geographyID && x.WaterAccountRoleID == WaterAccountRole.WaterAccountHolder.WaterAccountRoleID)
            .AsEnumerable().DistinctBy(x => x.UserID)
            .Select(x => x.User.AsUserDto()).AsEnumerable()
            .OrderBy(x => x.LastName).ToList();

        return Ok(userDtos);
    }

    [HttpGet("{waterAccountID}/parcels/years/{year}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public async Task<ActionResult<List<ParcelMinimalDto>>> ListParcelsByWaterAccountID([FromRoute] int geographyID, [FromRoute] int waterAccountID, [FromRoute] int year)
    {
        var reportingPeriodDto = await ReportingPeriods.GetByGeographyIDAndYearAsync(_dbContext, geographyID, year, callingUser);
        if (reportingPeriodDto == null)
        {
            var geography = await Geographies.GetByIDAsMinimalDtoAsync(_dbContext, geographyID);
            return BadRequest($"No reporting period found for the year {year} for {geography.GeographyDisplayName}.");
        }

        var parcels = Parcels.ListParcelsFromAccountIDAndEndDate(_dbContext, waterAccountID, reportingPeriodDto.EndDate);
        return Ok(parcels);
    }
}