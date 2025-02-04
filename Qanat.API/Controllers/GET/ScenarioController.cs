using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("scenarios")]
public class ScenarioController(QanatDbContext dbContext, ILogger<ScenarioController> logger, IOptions<QanatConfiguration> qanatConfiguration)
    : SitkaController<ScenarioController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithScenarioPlannerRolePermission(PermissionEnum.ScenarioRights, RightsEnum.Read)]
    public ActionResult<List<ScenarioSimpleDto>> ListScenarios()
    {
        var scenarios = Scenario.AllAsSimpleDto;
        return Ok(scenarios);
    }

    [HttpGet("{scenarioShortName}")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithScenarioPlannerRolePermission(PermissionEnum.ScenarioRights, RightsEnum.Read)]
    public ActionResult<ScenarioSimpleDto> GetScenarioByID([FromRoute] string scenarioShortName)
    {
        var scenario = Scenario.AllAsSimpleDto.SingleOrDefault(x => x.ScenarioShortName == scenarioShortName);
        if (CheckAndLogIfNotFound(scenario, "Scenario", scenarioShortName, out var result))
        {
            return result;
        }

        return Ok(scenario);
    }

    [HttpGet("{scenarioID}/image")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithScenarioPlannerRolePermission(PermissionEnum.ScenarioRights, RightsEnum.Read)]
    public ActionResult<string> GetScenarioImageByID([FromRoute] int scenarioID)
    {
        var scenario = Scenario.All.SingleOrDefault(x => x.ScenarioID == scenarioID);
        if (CheckAndLogIfNotFound(scenario, "Scenario", scenarioID, out var result))
        {
            return result;
        }

        return Ok(scenario!.ScenarioImage);
    }
}