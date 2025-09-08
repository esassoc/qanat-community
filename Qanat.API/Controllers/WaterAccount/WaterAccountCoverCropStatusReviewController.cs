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
[Route("geographies/{geographyID}/review-water-account-cover-crop-statuses")]
public class WaterAccountCoverCropStatusReviewController(QanatDbContext dbContext, ILogger<WaterAccountCoverCropStatusReviewController> logger, IOptions<QanatConfiguration> qanatConfiguration, UserDto callingUser)
    : SitkaController<WaterAccountCoverCropStatusReviewController>(dbContext, logger, qanatConfiguration)
{
    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<List<WaterAccountCoverCropStatusDto>>> List([FromRoute] int geographyID)
    {
        var waterAccountCoverCropStatuses = await WaterAccountCoverCropStatuses.ListByGeographyIDAsync(_dbContext, geographyID);
        return Ok(waterAccountCoverCropStatuses);
    }

    [HttpPut("approve")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<List<WaterAccountCoverCropStatusDto>>> Approve([FromRoute] int geographyID, [FromBody] List<int> coverCropStatusIDs)
    {
        var errors = await WaterAccountCoverCropStatuses.ValidateApproveAsync(_dbContext, geographyID, coverCropStatusIDs);
        errors.ForEach(x => ModelState.AddModelError(x.Type, x.Message));
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await WaterAccountCoverCropStatuses.ApproveAsync(dbContext, geographyID, coverCropStatusIDs, callingUser);

        var waterAccountCoverCropStatuses = await WaterAccountCoverCropStatuses.ListByGeographyIDAsync(_dbContext, geographyID);
        var updatedCoverCropStatuses = waterAccountCoverCropStatuses.Where(x => coverCropStatusIDs.Contains(x.WaterAccountCoverCropStatusID!.Value)).ToList();
        return Ok(updatedCoverCropStatuses);
    }

    [HttpPut("return")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<List<WaterAccountCoverCropStatusDto>>> Return([FromRoute] int geographyID, [FromBody] List<int> coverCropStatusIDs)
    {
        var errors = await WaterAccountCoverCropStatuses.ValidateReturnAsync(_dbContext, geographyID, coverCropStatusIDs);
        errors.ForEach(x => ModelState.AddModelError(x.Type, x.Message));
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await WaterAccountCoverCropStatuses.ReturnAsync(dbContext, geographyID, coverCropStatusIDs, callingUser);

        var waterAccountCoverCropStatuses = await WaterAccountCoverCropStatuses.ListByGeographyIDAsync(_dbContext, geographyID);
        var updatedCoverCropStatuses = waterAccountCoverCropStatuses.Where(x => coverCropStatusIDs.Contains(x.WaterAccountCoverCropStatusID!.Value)).ToList();
        return Ok(updatedCoverCropStatuses);
    }
}