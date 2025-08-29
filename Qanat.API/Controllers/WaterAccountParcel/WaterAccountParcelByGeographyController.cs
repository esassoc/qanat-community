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
[Route("geographies/{geographyID}/water-account-parcels")]
public class WaterAccountParcelByGeographyController(QanatDbContext dbContext, ILogger<WaterAccountParcelByGeographyController> logger, IOptions<QanatConfiguration> qanatConfiguration, [FromServices] UserDto callingUser)
    : SitkaController<WaterAccountParcelByGeographyController>(dbContext, logger, qanatConfiguration)
{
    [HttpPost("by-reporting-period/{toReportingPeriodID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(ReportingPeriod), "toReportingPeriodID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<List<WaterAccountParcelSimpleDto>>> CopyFromReportingPeriod([FromRoute] int geographyID, [FromRoute] int toReportingPeriodID, [FromBody] CopyWaterAccountParcelsFromReportingPeriodDto copyDto)
    {
        var validationErrors = await WaterAccountParcels.ValidateCopyFromReportingPeriodAsync(dbContext, geographyID, copyDto.FromReportingPeriodID, toReportingPeriodID, callingUser);
        validationErrors.ForEach(x => ModelState.AddModelError(x.Type, x.Message));

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var copiedWaterAccountParcels = await WaterAccountParcels.CopyFromReportingPeriodAsync(_dbContext, geographyID, copyDto.FromReportingPeriodID, toReportingPeriodID, callingUser);
        return Ok(copiedWaterAccountParcels);
    }
}