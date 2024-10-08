using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("water-accounts/{waterAccountID}/parcels")]
public class WaterAccountParcelController : SitkaController<WaterAccountParcelController>
{
    public WaterAccountParcelController(QanatDbContext dbContext, ILogger<WaterAccountParcelController> logger,
        IOptions<QanatConfiguration> qanatConfiguration) : base(dbContext, logger, qanatConfiguration)
    {
    }

    [HttpGet]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public ActionResult<List<ParcelDetailDto>> GetWaterAccountParcels([FromRoute] int waterAccountID)
    {
        var parcelDetailedDtos = Parcels.GetParcelDetailDtoListByWaterAccountID(_dbContext, waterAccountID);
        return Ok(parcelDetailedDtos);
    }

    [HttpPut("{parcelID}")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Update)]
    public async Task<ActionResult<WaterAccountMinimalDto>> AddOrphanedParcelToWaterAccount([FromRoute] int waterAccountID,
        [FromRoute] int parcelID)
    {
        var errors = WaterAccounts.ValidateAddOrphanedParcelToWaterAccount(_dbContext, waterAccountID, parcelID);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var waterAccount = await WaterAccounts.AddOrphanedParcelToWaterAccount(_dbContext, waterAccountID, parcelID);

        return Ok(waterAccount);
    }
}