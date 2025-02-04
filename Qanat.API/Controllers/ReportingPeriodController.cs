using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.API.Services.Attributes;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
[Route("geographies/{geographyID}/reporting-periods")]
public class ReportingPeriodController(QanatDbContext dbContext, ILogger<ReportingPeriodController> logger, IOptions<QanatConfiguration> qanatConfiguration, [FromServices] UserDto callingUser)
    : SitkaController<ReportingPeriodController>(dbContext, logger, qanatConfiguration)

{
    [HttpPost]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithGeographyRolePermission(PermissionEnum.ReportingPeriodRights, RightsEnum.Create)]
    public async Task<ActionResult<ReportingPeriodDto>> Create(int geographyID, ReportingPeriodUpsertDto reportingPeriodUpsertDto)
    {
        var validationErrors = await ReportingPeriods.ValidateCreateAsync(_dbContext, geographyID, reportingPeriodUpsertDto);
        if (validationErrors.Any() || !ModelState.IsValid)
        {
            validationErrors.ForEach(ve => ModelState.AddModelError(ve.Type, ve.Message));
            return BadRequest(ModelState);
        }

        var reportingPeriod = await ReportingPeriods.CreateAsync(_dbContext, geographyID, reportingPeriodUpsertDto, callingUser);
        return Ok(reportingPeriod);
    }

    [HttpGet]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ReportingPeriodRights, RightsEnum.Read)]
    public async Task<ActionResult<List<ReportingPeriodDto>>> ListByGeographyID(int geographyID)
    {
        var reportingPeriods = await ReportingPeriods.ListByGeographyIDAsync(_dbContext, geographyID, callingUser);
        return Ok(reportingPeriods);
    }

    [HttpGet("{reportingPeriodID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [WithGeographyRolePermission(PermissionEnum.ReportingPeriodRights, RightsEnum.Read)]
    public async Task<ActionResult<ReportingPeriodDto>> Get(int geographyID, int reportingPeriodID)
    {
        var reportingPeriod = await ReportingPeriods.GetAsync(_dbContext, geographyID, reportingPeriodID, callingUser);
        if (reportingPeriod == null)
        {
            return NotFound();
        }

        return Ok(reportingPeriod);
    }

    [HttpPut("{reportingPeriodID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithGeographyRolePermission(PermissionEnum.ReportingPeriodRights, RightsEnum.Update)]
    public async Task<ActionResult<ReportingPeriodDto>> Update(int geographyID, int reportingPeriodID, ReportingPeriodUpsertDto reportingPeriodUpsertDto)
    {
        var validationErrors = await ReportingPeriods.ValidateUpdateAsync(_dbContext, geographyID, reportingPeriodUpsertDto, reportingPeriodID);
        if (validationErrors.Any() || !ModelState.IsValid)
        {
            validationErrors.ForEach(ve => ModelState.AddModelError(ve.Type, ve.Message));
            return BadRequest(ModelState);
        }

        var updatedReportingPeriod = await ReportingPeriods.UpdateAsync(_dbContext, geographyID, reportingPeriodID, reportingPeriodUpsertDto, callingUser);
        return Ok(updatedReportingPeriod);
    }
}