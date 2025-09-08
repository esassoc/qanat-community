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
[Route("geographies/{geographyID}/water-account-fallow-statuses")]
public class WaterAccountFallowSummaryController(QanatDbContext dbContext, ILogger<WaterAccountFallowSummaryController> logger, IOptions<QanatConfiguration> qanatConfiguration, UserDto callingUser)
    : SitkaController<WaterAccountFallowSummaryController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<List<SelfReportSummaryDto>>> ListSummaries([FromRoute] int geographyID)
    {
        var fallowSummaries = await WaterAccountFallowStatuses.ListSummariesByGeographyIDAsync(_dbContext, geographyID, callingUser);
        return Ok(fallowSummaries);
    }
}