using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
[Route("models")]
public class ModelController(QanatDbContext dbContext, ILogger<ModelController> logger, IOptions<QanatConfiguration> qanatConfiguration, UserDto callingUser)
    : SitkaController<ModelController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithScenarioPlannerRolePermission(PermissionEnum.ModelRights, RightsEnum.Read)]
    public async Task<ActionResult<List<ModelSimpleDto>>> ListModels()
    {
        var models = await ModelUsers.ListModelsByUserID(_dbContext, callingUser);
        return Ok(models);
    }

    [HttpGet("{modelShortName}")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithScenarioPlannerRolePermission(PermissionEnum.ModelRights, RightsEnum.Read)]
    public async Task<ActionResult<ModelSimpleDto>> GetModelByID([FromRoute] string modelShortName)
    {
        var model = Model.All.SingleOrDefault(x => x.ModelShortName == modelShortName);

        //MK 1/23/2025:Authorization checks should happen before not founds, as bad actors can get info from not founds. Probably doesn't matter much in this context but it's still good to be aware.
        if (model != null)
        {
            var userHasAccess = await ModelUsers.CheckIfUserHasModelAccessAsync(_dbContext, model.ModelID, callingUser);
            if (!userHasAccess)
            {
                return Forbid();
            }
        }

        if (CheckAndLogIfNotFound(model, "Model", modelShortName, out var result))
        {
            return result;
        }
            
        return Ok(model.AsSimpleDto());
    }

    [HttpGet("{modelID}/image")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithScenarioPlannerRolePermission(PermissionEnum.ModelRights, RightsEnum.Read)]
    public async Task<ActionResult<string>> GetModelImageByID([FromRoute] int modelID)
    {
        var model = Model.All.SingleOrDefault(x => x.ModelID == modelID);

        //MK 1/23/2025: Authorization checks should happen before not founds, as bad actors can get info from not founds. Probably doesn't matter much in this context but it's still good to be aware.
        if (model != null)
        {
            var userHasAccess = await ModelUsers.CheckIfUserHasModelAccessAsync(_dbContext, model.ModelID, callingUser);
            if (!userHasAccess)
            {
                return Forbid();
            }
        }

        if (CheckAndLogIfNotFound(model, "Model", modelID, out var result))
        {
            return result;
        }

        return Ok(model!.ModelImage);
    }

    [HttpGet("{modelShortName}/boundary")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithScenarioPlannerRolePermission(PermissionEnum.ModelRights, RightsEnum.Read)]
    public async Task<ActionResult<ModelBoundaryDto>> GetModelBoundaryByModelShortName([FromRoute] string modelShortName)
    {
        var model = Model.All.SingleOrDefault(x => x.ModelShortName == modelShortName);

        //MK 1/23/2025: Authorization checks should happen before not founds, as bad actors can get info from not founds. Probably doesn't matter much in this context but it's still good to be aware.
        if (model != null)
        {
            var userHasAccess = await ModelUsers.CheckIfUserHasModelAccessAsync(_dbContext, model.ModelID, callingUser);
            if (!userHasAccess)
            {
                return Forbid();
            }
        }

        if (CheckAndLogIfNotFound(model, "Model", modelShortName, out var result))
        {
            return result;
        }

        var modelBoundaryDto = _dbContext.ModelBoundaries.AsNoTracking()
            .SingleOrDefault(x => x.ModelID == model.ModelID)?.AsModelBoundaryDto();

        return Ok(modelBoundaryDto);
    }

    [HttpGet("{modelShortName}/scenario-runs")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithScenarioPlannerRolePermission(PermissionEnum.ModelRights, RightsEnum.Read)]
    public async Task<ActionResult<List<ScenarioRunDto>>> ListActionsForModel([FromRoute] string modelShortName)
    {
        var model = Model.All.SingleOrDefault(x => x.ModelShortName == modelShortName);

        //MK 1/23/2025: Authorization checks should happen before not founds, as bad actors can get info from not founds. Probably doesn't matter much in this context but it's still good to be aware.
        if (model != null)
        {
            var userHasAccess = await ModelUsers.CheckIfUserHasModelAccessAsync(_dbContext, model.ModelID, callingUser);
            if (!userHasAccess)
            {
                return Forbid();
            }
        }

        if (CheckAndLogIfNotFound(model, "Model", modelShortName, out var result))
        {
            return result;
        }

        var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        if (userDto.GETRunCustomerID != null)
        {
            var byModelIDAndUserID = ScenarioRuns.ListByModelIDAndGETRunCustomerID(_dbContext, model!.ModelID, userDto.GETRunCustomerID.Value);
            return Ok(byModelIDAndUserID);
        }

        var scenarioRuns = ScenarioRuns.ListByModelID(_dbContext, model!.ModelID);
        return Ok(scenarioRuns);
    }
}