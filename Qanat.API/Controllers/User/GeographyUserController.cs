using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects.Geography;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies")]
public class GeographyUserController(QanatDbContext dbContext, ILogger<GeographyUserController> logger, IOptions<QanatConfiguration> qanatConfiguration)
    : SitkaController<GeographyUserController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet("{geographyID}/users")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<List<GeographyUserDto>>> List([FromRoute] int geographyID)
    {
        var userDtos = await GeographyUsers.ListAsync(_dbContext, geographyID);
        return Ok(userDtos);
    }
}