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

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/water-measurement-self-reports")]
public class WaterMeasurementReviewSelfReportController(QanatDbContext dbContext, ILogger<WaterMeasurementReviewSelfReportController> logger, IOptions<QanatConfiguration> qanatConfiguration)
    : SitkaController<WaterMeasurementReviewSelfReportController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<List<WaterMeasurementSelfReportSimpleDto>>> ReadList([FromRoute] int geographyID)
    {
        var selfReportSimpleDtos = await WaterMeasurementSelfReports.ListAsSimpleDtoForGeographyAsync(_dbContext, geographyID);
        return Ok(selfReportSimpleDtos);
    }
}