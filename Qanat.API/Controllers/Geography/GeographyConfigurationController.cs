using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.Security;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/configuration")]
public class GeographyConfigurationController(
    QanatDbContext dbContext,
    ILogger<GeographyConfigurationController> logger,
    IOptions<QanatConfiguration> qanatConfiguration)
    : SitkaController<GeographyConfigurationController>(dbContext, logger, qanatConfiguration)
{
    [HttpPut("toggle-well-registry")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.GeographyRights, RightsEnum.Update)]
    public async Task<ActionResult<bool>> ToggleWellRegistryConfiguration([FromRoute] int geographyID, [FromBody] bool toggleEnabled)
    {
        await GeographyConfigurations.ToggleWellRegistry(_dbContext, geographyID, toggleEnabled);

        return Ok(true);
    }

    [HttpPut("toggle-landing-page")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.GeographyRights, RightsEnum.Update)]
    public async Task<ActionResult<bool>> ToggleLandingPageConfiguration([FromRoute] int geographyID, [FromBody] bool toggleEnabled)
    {
        await GeographyConfigurations.ToggleLandingPage(_dbContext, geographyID, toggleEnabled);

        return Ok(true);
    }

    [HttpPut("meter-configuration")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.GeographyRights, RightsEnum.Update)]
    public async Task<ActionResult<bool>> ToggleMeterConfiguration([FromRoute] int geographyID, [FromBody] bool toggleEnabled)
    {
        await GeographyConfigurations.ToggleMeterConfiguration(_dbContext, geographyID, toggleEnabled);

        return Ok(true);
    }
}