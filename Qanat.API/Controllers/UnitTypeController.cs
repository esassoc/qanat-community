using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.API.Services.Filters;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class UnitTypeController : SitkaController<UnitTypeController>
{
    public UnitTypeController(QanatDbContext dbContext, ILogger<UnitTypeController> logger,
        IOptions<QanatConfiguration> qanatConfiguration)
        : base(dbContext, logger, qanatConfiguration)
    {
    }

    [HttpGet("geographies/{geographyID}/unitTypes/")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterTransactionRights, RightsEnum.Read)]
    public ActionResult<List<UnitTypeSimpleDto>> GetUsageWaterTypes([FromRoute] int geographyID)
    {
        var unitTypeDtos = UnitType.AllAsSimpleDto;
        return Ok(unitTypeDtos);
    }
}