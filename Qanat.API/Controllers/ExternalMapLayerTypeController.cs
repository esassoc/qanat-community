using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
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
public class ExternalMapLayerTypeController : SitkaController<ExternalMapLayerTypeController>
{
    public ExternalMapLayerTypeController(QanatDbContext dbContext, ILogger<ExternalMapLayerTypeController> logger, IOptions<QanatConfiguration> qanatConfiguration) : base(dbContext, logger, qanatConfiguration)
    {
    }

    [HttpGet("external-map-layer-types")]
    [Authorize]
    public ActionResult<List<ExternalMapLayerTypeSimpleDto>> Get()
    {
        return Ok(ExternalMapLayerType.AllAsSimpleDto);
    }
}