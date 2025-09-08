using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/review-water-account-fallow-statuses")]
public class WaterAccountFallowStatusReviewController(QanatDbContext dbContext, ILogger<WaterAccountFallowStatusReviewController> logger, IOptions<QanatConfiguration> qanatConfiguration, UserDto callingUser)
    : SitkaController<WaterAccountFallowStatusReviewController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<List<WaterAccountFallowStatusDto>>> List([FromRoute] int geographyID)
    {
        var waterAccountFallowStatuses = await WaterAccountFallowStatuses.ListByGeographyIDAsync(_dbContext, geographyID);
        return Ok(waterAccountFallowStatuses);
    }

    [HttpPut("approve")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<List<WaterAccountFallowStatusDto>>> Approve([FromRoute] int geographyID, [FromBody] List<int> coverCropStatusIDs)
    {
        var errors = await WaterAccountFallowStatuses.ValidateApproveAsync(_dbContext, geographyID, coverCropStatusIDs);
        errors.ForEach(x => ModelState.AddModelError(x.Type, x.Message));
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await WaterAccountFallowStatuses.ApproveAsync(dbContext, geographyID, coverCropStatusIDs, callingUser);

        var waterAccountFallowStatuses = await WaterAccountFallowStatuses.ListByGeographyIDAsync(_dbContext, geographyID);
        var updatedFallowStatuses = waterAccountFallowStatuses.Where(x => coverCropStatusIDs.Contains(x.WaterAccountFallowStatusID!.Value)).ToList();
        return Ok(updatedFallowStatuses);
    }

    [HttpPut("return")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<List<WaterAccountFallowStatusDto>>> Return([FromRoute] int geographyID, [FromBody] List<int> coverCropStatusIDs)
    {
        var errors = await WaterAccountFallowStatuses.ValidateReturnAsync(_dbContext, geographyID, coverCropStatusIDs);
        errors.ForEach(x => ModelState.AddModelError(x.Type, x.Message));
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await WaterAccountFallowStatuses.ReturnAsync(dbContext, geographyID, coverCropStatusIDs, callingUser);

        var waterAccountFallowStatuses = await WaterAccountFallowStatuses.ListByGeographyIDAsync(_dbContext, geographyID);
        var updatedFallowStatuses = waterAccountFallowStatuses.Where(x => coverCropStatusIDs.Contains(x.WaterAccountFallowStatusID!.Value)).ToList();
        return Ok(updatedFallowStatuses);
    }
}