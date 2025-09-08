using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("parcels/{parcelID}/water-accounts")]
public class WaterAccountParcelByParcelController(QanatDbContext dbContext, ILogger<WaterAccountParcelByParcelController> logger, IOptions<QanatConfiguration> qanatConfiguration, [FromServices] UserDto callingUser)
    : SitkaController<WaterAccountParcelByParcelController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public async Task<ActionResult<List<WaterAccountMinimalAndReportingPeriodSimpleDto>>> GetWaterAccountParcelsForParcel([FromRoute] int parcelID)
    {
        var waterAccountParcels = await WaterAccountParcels.ListByParcelIDAsync(dbContext, parcelID);
        return Ok(waterAccountParcels);
    }

    [HttpPut]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Update)]
    public async Task<ActionResult<List<WaterAccountMinimalAndReportingPeriodSimpleDto>>> UpdateWaterAccountParcelsForParcel([FromRoute] int parcelID, [FromBody] UpdateWaterAccountParcelsByParcelDto updateParcelWaterAccountsDto)
    {
        var errorMessages = await WaterAccountParcels.ValidateWaterAccountParcelUpdateByParcelAsync(dbContext, parcelID, updateParcelWaterAccountsDto);
        errorMessages.ForEach(em => ModelState.AddModelError(em.Type, em.Message));
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedWaterAccountParcels = await WaterAccountParcels.UpdateWaterAccountParcelsByParcelAsync(dbContext, parcelID, updateParcelWaterAccountsDto, callingUser);
        return Ok(updatedWaterAccountParcels);
    }

    [HttpGet("history")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithGeographyRolePermission(PermissionEnum.ParcelRights, RightsEnum.Read)]
    public async Task<ActionResult<List<ParcelWaterAccountHistorySimpleDto>>> GetWaterAccountParcelHistory([FromRoute] int parcelID)
    {
        var waterAccountParcelDtos = await ParcelWaterAccountHistories.ListAsync(dbContext, parcelID);
        return Ok(waterAccountParcelDtos);
    }
}