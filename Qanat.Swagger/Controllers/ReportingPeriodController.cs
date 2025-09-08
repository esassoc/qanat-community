using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Qanat.EFModels.Entities;
using Qanat.Swagger.Filters;
using Qanat.Swagger.Models;

namespace Qanat.Swagger.Controllers;

[Authorize]
[ApiController]
[Tags("Reporting Periods")]
public class ReportingPeriodController(QanatDbContext dbContext, ILogger<ReportingPeriodController> logger)
    : ControllerBase
{
    [EndpointSummary("List by Geography")]
    [EndpointDescription("List all reporting periods for a specified geography")]
    [NoAccessBlock]
    [GeographyAccess]
    [HttpGet("geographies/{geographyID}/reporting-periods")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Produces("application/json")]
    public ActionResult<List<ReportingPeriodConsumerDto>> ListReportingPeriods([FromRoute] int geographyID)
    {
        var reportingPeriodConsumerDtos = dbContext.ReportingPeriods.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .Select(x => new ReportingPeriodConsumerDto
            {
                ReportingPeriodID = x.ReportingPeriodID,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
            }).ToList();

        return Ok(reportingPeriodConsumerDtos);
    }
}