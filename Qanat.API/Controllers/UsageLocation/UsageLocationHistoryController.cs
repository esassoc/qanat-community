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
[Route("geographies/{geographyID}")]
public class UsageLocationHistoryController(QanatDbContext dbContext, ILogger<UsageLocationHistoryController> logger, IOptions<QanatConfiguration> qanatConfiguration)
    : SitkaController<UsageLocationHistoryController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet("water-accounts/{waterAccountID}/usage-location-histories")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithWaterAccountRolePermission(PermissionEnum.UsageLocationRights, RightsEnum.Read)]
    public async Task<ActionResult<List<UsageLocationHistoryDto>>> ListByWaterAccount([FromRoute] int geographyID, [FromRoute] int waterAccountID)
    {
        var locationHistoryDtos = await UsageLocationHistories.ListByGeographyIDAndWaterAccountIDAsync(_dbContext, geographyID, waterAccountID);
        return Ok(locationHistoryDtos);
    }

    [HttpGet("parcels/{parcelID}/usage-location-histories")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(Parcel), "parcelID")]
    [WithWaterAccountRolePermission(PermissionEnum.UsageLocationRights, RightsEnum.Read)]
    public async Task<ActionResult<List<UsageLocationHistoryDto>>> ListByParcel([FromRoute] int geographyID, [FromRoute] int parcelID)
    {
        var locationHistoryDtos = await UsageLocationHistories.ListByGeographyIDAndParcelIDAsync(_dbContext, geographyID, parcelID);
        return Ok(locationHistoryDtos);
    }
}