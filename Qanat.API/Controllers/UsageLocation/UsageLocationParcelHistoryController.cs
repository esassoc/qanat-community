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
[Route("geographies/{geographyID}/parcels/{parcelID}/usage-location-parcel-histories")]
public class UsageLocationParcelHistoryController(QanatDbContext dbContext, ILogger<UsageLocationParcelHistoryController> logger, IOptions<QanatConfiguration> qanatConfiguration)
    : SitkaController<UsageLocationParcelHistoryController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithWaterAccountRolePermission(PermissionEnum.UsageLocationRights, RightsEnum.Read)]
    public async Task<ActionResult<List<UsageLocationParcelHistoryDto>>> List([FromRoute] int geographyID, [FromRoute] int parcelID)
    {
        var usageLocationParcelHistories = await UsageLocationParcelHistories.ListByParcelIDAsync(_dbContext, geographyID, parcelID);
        return Ok(usageLocationParcelHistories);
    }
}