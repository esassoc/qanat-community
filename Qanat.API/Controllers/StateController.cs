using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Qanat.API.Services;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services.Authorization;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class StateController : SitkaController<StateController>
{
    public StateController(QanatDbContext dbContext, ILogger<StateController> logger,
        IOptions<QanatConfiguration> qanatConfiguration)
        : base(dbContext, logger, qanatConfiguration)
    {
    }

    [HttpGet("public/states")]
    [AllowAnonymous]
    public ActionResult<List<StateSimpleDto>> List()
    {
        var stateList = State.AllAsSimpleDto;
        return Ok(stateList);
    }
}