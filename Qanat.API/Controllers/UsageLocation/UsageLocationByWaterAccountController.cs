using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
[Route("geographies/{geographyID}/water-accounts/{waterAccountID}/usage-locations")]
public class UsageLocationByWaterAccountController(QanatDbContext dbContext, ILogger<UsageLocationByWaterAccountController> logger, IOptions<QanatConfiguration> qanatConfiguration, UserDto callingUser)
    : SitkaController<UsageLocationByWaterAccountController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.UsageLocationRights, RightsEnum.Read)]
    public async Task<ActionResult<List<UsageLocationDto>>> List([FromRoute] int geographyID, [FromRoute] int waterAccountID)
    {
        var usageLocationDtos = await UsageLocations.ListByWaterAccountAsync(_dbContext, geographyID, waterAccountID);
        return Ok(usageLocationDtos);
    }
}