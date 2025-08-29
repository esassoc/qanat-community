using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/meter-readings")]
public class MeterReadingByGeographyController(QanatDbContext dbContext, ILogger<MeterReadingByGeographyController> logger, IOptions<QanatConfiguration> qanatConfiguration)
    : SitkaController<MeterReadingByGeographyController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet()]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<List<MeterReadingGridDto>>> ListByGeographyID(int geographyID)
    {
        var meterReadings = await MeterReadings.ListByGeographyIDAsync(_dbContext, geographyID);
        return Ok(meterReadings);
    }
}