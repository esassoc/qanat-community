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
[Route("geographies/{geographyID}/reporting-periods/{reportingPeriodID}/water-account-cover-crop-statuses")]
public class WaterAccountCoverCropStatusController(QanatDbContext dbContext, ILogger<WaterAccountCoverCropStatusController> logger, IOptions<QanatConfiguration> qanatConfiguration, UserDto callingUser)
    : SitkaController<WaterAccountCoverCropStatusController>(dbContext, logger, qanatConfiguration)
{
    [HttpPost("water-accounts/{waterAccountID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Update)]
    public async Task<ActionResult<WaterAccountCoverCropStatusDto>> Create([FromRoute] int geographyID, [FromRoute] int reportingPeriodID, [FromRoute] int waterAccountID)
    {
        var errors = await WaterAccountCoverCropStatuses.ValidateCreateAsync(_dbContext, geographyID, reportingPeriodID, waterAccountID, callingUser);
        errors.ForEach(x => ModelState.AddModelError(x.Type, x.Message));
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var waterAccountCoverCropStatus = await WaterAccountCoverCropStatuses.CreateAsync(_dbContext, geographyID, reportingPeriodID, waterAccountID, callingUser);
        return waterAccountCoverCropStatus;
    }

    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<List<WaterAccountCoverCropStatusDto>>> List([FromRoute] int geographyID, [FromRoute] int reportingPeriodID)
    {
        var waterAccountCoverCropStatuses = await WaterAccountCoverCropStatuses.ListByGeographyIDAndReportingPeriodIDForUserAsync(_dbContext, geographyID, reportingPeriodID, callingUser);
        return Ok(waterAccountCoverCropStatuses);
    }

    [HttpGet("{coverCropStatusID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [EntityNotFound(typeof(WaterAccountCoverCropStatus), "coverCropStatusID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public async Task<ActionResult<WaterAccountCoverCropStatusDto>> Get([FromRoute] int geographyID, [FromRoute] int reportingPeriodID, [FromRoute] int coverCropStatusID)
    {
        var waterAccountCoverCropStatus = await WaterAccountCoverCropStatuses.GetByIDAsync(_dbContext, geographyID, reportingPeriodID, coverCropStatusID);
        return Ok(waterAccountCoverCropStatus);
    }

    [HttpPut("{coverCropStatusID}/submit")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [EntityNotFound(typeof(WaterAccountCoverCropStatus), "coverCropStatusID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Update)]
    public async Task<ActionResult<WaterAccountCoverCropStatusDto>> Submit([FromRoute] int geographyID, [FromRoute] int reportingPeriodID, [FromRoute] int coverCropStatusID)
    {
        var errors = await WaterAccountCoverCropStatuses.ValidateSubmitAsync(_dbContext, geographyID, reportingPeriodID, coverCropStatusID);
        errors.ForEach(x => ModelState.AddModelError(x.Type, x.Message));
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var waterAccountCoverCropStatus = await WaterAccountCoverCropStatuses.SubmitAsync(_dbContext, geographyID, reportingPeriodID, coverCropStatusID, callingUser);
        return Ok(waterAccountCoverCropStatus);
    }
}