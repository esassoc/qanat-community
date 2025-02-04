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
[Route("geographies/{geographyID}/irrigation-methods")]
public class IrrigationMethodController : SitkaController<IrrigationMethodController>
{
    public IrrigationMethodController(QanatDbContext dbContext, ILogger<IrrigationMethodController> logger, IOptions<QanatConfiguration> qanatConfiguration) : base(dbContext, logger, qanatConfiguration)
    {
    }

    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.WaterTypeRights, RightsEnum.Read)]
    public async Task<ActionResult<List<IrrigationMethodSimpleDto>>> ReadList([FromRoute] int geographyID)
    {
        var irrigationMethodDtos = await IrrigationMethods.ListAsSimpleDtoAsync(_dbContext, geographyID);
        return Ok(irrigationMethodDtos);
    }
}