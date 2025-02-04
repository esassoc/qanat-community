using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;

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

    [HttpPost("search/water-accounts")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public ActionResult<WaterAccountSearchSummaryDto> SearchWaterAccounts([FromBody] WaterAccountSearchDto waterAccountSearchDto)
    {
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        var results = WaterAccounts.GetBySearchString(_dbContext, waterAccountSearchDto.GeographyID, waterAccountSearchDto.SearchString, user);
        return Ok(results);
    }

    [HttpPost("search/parcels")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public ActionResult<ParcelSearchSummaryDto> SearchParcels([FromBody] ParcelSearchDto parcelSearchDto)
    {
        var user = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);
        var results = Parcels.GetBySearchString(_dbContext, parcelSearchDto.GeographyID, parcelSearchDto.SearchString, user);
        return Ok(results);
    }
}