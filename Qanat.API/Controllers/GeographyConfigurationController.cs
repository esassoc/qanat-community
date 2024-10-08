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
public class GeographyConfigurationController : SitkaController<GeographyConfigurationController>
{

    public GeographyConfigurationController(QanatDbContext dbContext, ILogger<GeographyConfigurationController> logger,
        IOptions<QanatConfiguration> qanatConfiguration)
        : base(dbContext, logger, qanatConfiguration)
    {
    }


    [HttpPut("geographies/{geographyID}/configuration/toggle-well-registry")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.GeographyRights, RightsEnum.Update)]
    public async Task<ActionResult<bool>> ToggleWellRegistryConfiguration([FromRoute] int geographyID, [FromBody] bool toggleEnabled)
    {
        await GeographyConfigurations.ToggleWellRegistry(_dbContext, geographyID, toggleEnabled);

        return Ok(true);
    }

    [HttpPut("geographies/{geographyID}/configuration/toggle-landing-page")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.GeographyRights, RightsEnum.Update)]
    public async Task<ActionResult<bool>> ToggleLandingPageConfiguration([FromRoute] int geographyID, [FromBody] bool toggleEnabled)
    {
        await GeographyConfigurations.ToggleLandingPage(_dbContext, geographyID, toggleEnabled);

        return Ok(true);
    }

    [HttpPut("geographies/{geographyID}/configuration/meter-configuration")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.GeographyRights, RightsEnum.Update)]
    public async Task<ActionResult<bool>> ToggleMeterConfiguration([FromRoute] int geographyID, [FromBody] bool toggleEnabled)
    {
        await GeographyConfigurations.ToggleMeterConfiguration(_dbContext, geographyID, toggleEnabled);

        return Ok(true);
    }

    [HttpGet("geography-configurations")]
    [AuthenticatedWithUser]
    public async Task<ActionResult<List<GeographyConfigurationDto>>> GetGeographyConfigurations()
    {
        var configurations = await GeographyConfigurations.GetGeographyConfigurations(_dbContext);
        return configurations;
    }
}