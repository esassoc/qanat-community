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
public class SearchController : SitkaController<SearchController>
{
    public SearchController(QanatDbContext dbContext, ILogger<SearchController> logger,
        IOptions<QanatConfiguration> qanatConfiguration)
        : base(dbContext, logger, qanatConfiguration)
    {
    }

    [HttpGet("search/geography/{geographyID}/water-accounts")]
    [WithGeographyRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public ActionResult<WaterAccountSearchSummaryDto> SearchWaterAccounts([FromQuery] string searchString, [FromRoute] int geographyID)
    {

        var results = WaterAccounts.GetBySearchString(_dbContext, geographyID, searchString);
        return Ok(results);
    }
}