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
[Route("geographies/{geographyID}/wells/{wellID}/meter-readings")]
public class MeterReadingByWellController(QanatDbContext dbContext, ILogger<MeterReadingByWellController> logger, IOptions<QanatConfiguration> qanatConfiguration)
    : SitkaController<MeterReadingByWellController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(Well), "wellID")]
    [WithWaterAccountRolePermission(PermissionEnum.WellMeterReadingRights, RightsEnum.Read)]
    public async Task<ActionResult<List<MeterReadingDto>>> ListMeterReadingsByWell([FromRoute] int geographyID, [FromRoute] int wellID)
    {
        var meterReadingDtos = await MeterReadings.ListByWellIDAsDtoAsync(_dbContext, geographyID, wellID);
        return Ok(meterReadingDtos);
    }

    [HttpGet("monthly-interpolations")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(Well), "wellID")]
    [WithWaterAccountRolePermission(PermissionEnum.WellMeterReadingRights, RightsEnum.Read)]
    public async Task<ActionResult<List<MeterReadingMonthlyInterpolationSimpleDto>>> ListMonthlyInterpolationsByWell([FromRoute] int geographyID, [FromRoute] int wellID)
    {
        var currentMeter = await Meters.GetCurrentWellMeterByWellIDAsDtoAsync(_dbContext, wellID);
        if (currentMeter == null)
        {
            return NotFound();
        }

        var monthlyInterpolations = await MeterReadingMonthlyInterpolations.ListMonthlyInterpolationsByWellIDAsDtoAsync(_dbContext, geographyID, wellID, currentMeter.MeterID);
        return Ok(monthlyInterpolations);
    }
}