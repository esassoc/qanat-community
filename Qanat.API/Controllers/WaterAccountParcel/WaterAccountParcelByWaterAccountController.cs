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
[Route("water-accounts/{waterAccountID}/parcels")]
public class WaterAccountParcelByWaterAccountController(QanatDbContext dbContext, ILogger<WaterAccountParcelByWaterAccountController> logger, IOptions<QanatConfiguration> qanatConfiguration, [FromServices] UserDto callingUser)
    : SitkaController<WaterAccountParcelByWaterAccountController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public ActionResult<List<ParcelIndexGridDto>> GetWaterAccountParcels([FromRoute] int waterAccountID)
    {
        var parcelIndexGridDtos = Parcels.ListByWaterAccountIDAsIndexGridDtos(_dbContext, waterAccountID);
        return Ok(parcelIndexGridDtos);
    }

    [HttpGet("minimals/years/{year}")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public async Task<ActionResult<List<ParcelMinimalDto>>> GetCurrentParcelsFromAccountID([FromRoute] int waterAccountID, [FromRoute] int year)
    {
        var parcelMinimalDtos = await WaterAccountParcels.ListByWaterAccountIDAndYearAsync(_dbContext, waterAccountID, year);
        return Ok(parcelMinimalDtos);
    }
    
    [HttpPut]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Update)]
    public async Task<ActionResult<ParcelMinimalDto>> UpdateWaterAccountParcels([FromRoute] int waterAccountID, [FromBody] WaterAccountParcelsUpdateDto updateDto)
    {
        var errors = await WaterAccountParcels.ValidateUpdateWaterAccountParcelByWaterAccountAndReportingPeriodAsync(_dbContext, waterAccountID, updateDto, callingUser);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var parcelMinimal = await WaterAccountParcels.UpdateWaterAccountParcelByWaterAccountAndReportingPeriodAsync(_dbContext, waterAccountID, updateDto.ReportingPeriodID, updateDto.ParcelIDs, callingUser);
        return Ok(parcelMinimal);
    }

    [HttpPut("{parcelID}")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Update)]
    public async Task<ActionResult<ParcelMinimalAndReportingPeriodSimpleDto>> AddOrphanedParcelToWaterAccount([FromRoute] int waterAccountID, [FromRoute] int parcelID)
    {
        var errors = await WaterAccountParcels.ValidateAddOrphanedParcelToWaterAccountAsync(_dbContext, waterAccountID, parcelID);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var parcelMinimalAndReportingPeriodSimpleDto = await WaterAccountParcels.AddOrphanedParcelToWaterAccountAsync(_dbContext, waterAccountID, parcelID, callingUser);
        return Ok(parcelMinimalAndReportingPeriodSimpleDto);
    }

    [HttpGet("history")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public async Task<ActionResult<List<ParcelWaterAccountHistorySimpleDto>>> GetWaterAccountParcelHistory([FromRoute] int waterAccountID)
    {
        var historySimpleDtos = await ParcelWaterAccountHistories.ListByWaterAccountIDAsync(dbContext, waterAccountID);
        return Ok(historySimpleDtos);
    }
}