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
[Route("geographies/{geographyID}/reporting-periods/{reportingPeriodID}/water-account-fallow-statuses")]
public class WaterAccountFallowStatusController(QanatDbContext dbContext, ILogger<WaterAccountFallowStatusController> logger, IOptions<QanatConfiguration> qanatConfiguration, UserDto callingUser)
    : SitkaController<WaterAccountFallowStatusController>(dbContext, logger, qanatConfiguration)
{
    [HttpPost("water-accounts/{waterAccountID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Update)]
    public async Task<ActionResult<WaterAccountFallowStatusDto>> Create([FromRoute] int geographyID, [FromRoute] int reportingPeriodID, [FromRoute] int waterAccountID)
    {
        var errors = await WaterAccountFallowStatuses.ValidateCreateAsync(_dbContext, geographyID, reportingPeriodID, waterAccountID, callingUser);
        errors.ForEach(x => ModelState.AddModelError(x.Type, x.Message));
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var waterAccountFallowStatus = await WaterAccountFallowStatuses.CreateAsync(_dbContext, geographyID, reportingPeriodID, waterAccountID, callingUser);
        return waterAccountFallowStatus;
    }

    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    public async Task<ActionResult<List<WaterAccountFallowStatusDto>>> List([FromRoute] int geographyID, [FromRoute] int reportingPeriodID)
    {
        var waterAccountFallowStatuses = await WaterAccountFallowStatuses.ListByGeographyIDAndReportingPeriodIDForUserAsync(_dbContext, geographyID, reportingPeriodID, callingUser);
        return Ok(waterAccountFallowStatuses);
    }

    [HttpGet("{fallowStatusID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [EntityNotFound(typeof(WaterAccountFallowStatus), "fallowStatusID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)]
    public async Task<ActionResult<WaterAccountFallowStatusDto>> Get([FromRoute] int geographyID, [FromRoute] int reportingPeriodID, [FromRoute] int fallowStatusID)
    {
        var waterAccountFallowStatus = await WaterAccountFallowStatuses.GetByIDAsync(_dbContext, geographyID, reportingPeriodID, fallowStatusID);
        return Ok(waterAccountFallowStatus);
    }

    [HttpPut("{fallowStatusID}/submit")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [EntityNotFound(typeof(WaterAccountFallowStatus), "FallowStatusID")]
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)]
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Update)]
    public async Task<ActionResult<WaterAccountFallowStatusDto>> Submit([FromRoute] int geographyID, [FromRoute] int reportingPeriodID, [FromRoute] int fallowStatusID)
    {
        var errors = await WaterAccountFallowStatuses.ValidateSubmitAsync(_dbContext, geographyID, reportingPeriodID, fallowStatusID);
        errors.ForEach(x => ModelState.AddModelError(x.Type, x.Message));
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var waterAccountFallowStatus = await WaterAccountFallowStatuses.SubmitAsync(_dbContext, geographyID, reportingPeriodID, fallowStatusID, callingUser);
        return Ok(waterAccountFallowStatus);
    }
}