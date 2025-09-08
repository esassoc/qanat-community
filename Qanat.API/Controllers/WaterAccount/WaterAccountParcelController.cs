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
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("water-accounts/{waterAccountID}/parcels")]
public class WaterAccountParcelController : SitkaController<WaterAccountParcelController>
{
    private readonly UserDto _callingUser;
    public WaterAccountParcelController(QanatDbContext dbContext, ILogger<WaterAccountParcelController> logger, IOptions<QanatConfiguration> qanatConfiguration, [FromServices] UserDto callingUser) : base(dbContext, logger, qanatConfiguration)
    {
        _callingUser = callingUser;
    }

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
    public ActionResult<List<ParcelMinimalDto>> GetCurrentParcelsFromAccountID([FromRoute] int waterAccountID, [FromRoute] int year)
    {
        var parcels = WaterAccountParcels.ListByWaterAccountIDAndYear(_dbContext, waterAccountID, year).Select(x => x.AsParcelMinimalDto()).ToList();
        return Ok(parcels);
    }

    [HttpPut("{parcelID}")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Update)]
    public async Task<ActionResult<WaterAccountMinimalDto>> AddOrphanedParcelToWaterAccount([FromRoute] int waterAccountID, [FromRoute] int parcelID)
    {
        var errors = WaterAccounts.ValidateAddOrphanedParcelToWaterAccount(_dbContext, waterAccountID, parcelID);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var waterAccount = await WaterAccounts.AddOrphanedParcelToWaterAccount(_dbContext, waterAccountID, parcelID, _callingUser);
        return Ok(waterAccount);
    }
}