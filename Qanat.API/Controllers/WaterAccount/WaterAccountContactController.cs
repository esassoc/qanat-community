using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.API.Services.OpenET;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("water-account-contacts")]
public class WaterAccountContactController(QanatDbContext dbContext, ILogger<WaterAccountContactController> logger,
        IOptions<QanatConfiguration> qanatConfiguration, [FromServices] UserDto callingUser)
    : SitkaController<WaterAccountContactController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet("{waterAccountContactID}")]
    [EntityNotFound(typeof(WaterAccountContact), "waterAccountContactID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public async Task<ActionResult<WaterAccountContactDto>> GetByID([FromRoute] int waterAccountContactID)
    {
        var waterAccountContactDto = await WaterAccountContacts.GetByIDAsDto(_dbContext, waterAccountContactID);
        return Ok(waterAccountContactDto);
    }

    [HttpPut("{waterAccountContactID}")]
    [EntityNotFound(typeof(WaterAccountContact), "waterAccountContactID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Update)]
    public async Task<ActionResult<WaterAccountContactDto>> Update([FromRoute] int waterAccountContactID, [FromBody] WaterAccountContactUpsertDto waterAccountContactUpsertDto)
    {
        var waterAccountContactDto = await WaterAccountContacts.Update(_dbContext, waterAccountContactID, waterAccountContactUpsertDto);
        return Ok(waterAccountContactDto);
    }

    [HttpDelete("{waterAccountContactID}")]
    [EntityNotFound(typeof(WaterAccountContact), "waterAccountContactID")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Delete)]
    public async Task<ActionResult> Delete([FromRoute] int waterAccountContactID)
    {
        var errors = WaterAccountContacts.ValidateDelete(_dbContext, waterAccountContactID);
        errors.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await WaterAccountContacts.Delete(_dbContext, waterAccountContactID);
        return NoContent();
    }
}