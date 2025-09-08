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
using System.Linq;
using System.Threading.Tasks;

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
    public async Task<ActionResult<ReportingPeriodDto>> Create([FromRoute] int geographyID, ReportingPeriodUpsertDto reportingPeriodUpsertDto)
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
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public async Task<ActionResult<List<ReportingPeriodDto>>> ListByGeographyID([FromRoute] int geographyID)
    {
        var reportingPeriods = await ReportingPeriods.ListByGeographyIDAsync(_dbContext, geographyID, callingUser);
        return Ok(reportingPeriods);
    }

    [HttpPut("{reportingPeriodID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithGeographyRolePermission(PermissionEnum.ReportingPeriodRights, RightsEnum.Update)]
    public async Task<ActionResult<ReportingPeriodDto>> Update([FromRoute] int geographyID, [FromRoute] int reportingPeriodID, [FromBody] ReportingPeriodUpsertDto reportingPeriodUpsertDto)
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

    [HttpPut("{reportingPeriodID}/cover-crop-self-report")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithGeographyRolePermission(PermissionEnum.ReportingPeriodRights, RightsEnum.Update)]
    public async Task<ActionResult<ReportingPeriodDto>> UpdateCoverCropSelfReportMetadata([FromRoute] int geographyID, [FromRoute] int reportingPeriodID, [FromBody] ReportingPeriodCoverCropSelfReportMetadataUpdateDto coverCropSelfReportMetadataUpdateDto)
    {
        var validationErrors = await ReportingPeriods.ValidateUpdateCoverCropSelfReportMetadataAsync(_dbContext, geographyID, reportingPeriodID, coverCropSelfReportMetadataUpdateDto);
        if (validationErrors.Any() || !ModelState.IsValid)
        {
            validationErrors.ForEach(ve => ModelState.AddModelError(ve.Type, ve.Message));
            return BadRequest(ModelState);
        }

        var updatedReportingPeriod = await ReportingPeriods.UpdateCoverCropSelfReportMetadataAsync(_dbContext, geographyID, reportingPeriodID, coverCropSelfReportMetadataUpdateDto, callingUser);
        return Ok(updatedReportingPeriod);
    }

    [HttpPut("{reportingPeriodID}/fallow-self-report")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)]
    [WithGeographyRolePermission(PermissionEnum.ReportingPeriodRights, RightsEnum.Update)]
    public async Task<ActionResult<ReportingPeriodDto>> UpdateFallowSelfReportMetadata([FromRoute] int geographyID, [FromRoute] int reportingPeriodID, [FromBody] ReportingPeriodFallowSelfReportMetadataUpdateDto fallowSelfReportMetadataUpdateDto)
    {
        var validationErrors = await ReportingPeriods.ValidateUpdateFallowSelfReportMetadataAsync(_dbContext, geographyID, reportingPeriodID, fallowSelfReportMetadataUpdateDto);
        if (validationErrors.Any() || !ModelState.IsValid)
        {
            validationErrors.ForEach(ve => ModelState.AddModelError(ve.Type, ve.Message));
            return BadRequest(ModelState);
        }

        var updatedReportingPeriod = await ReportingPeriods.UpdateFallowSelfReportMetadataAsync(_dbContext, geographyID, reportingPeriodID, fallowSelfReportMetadataUpdateDto, callingUser);
        return Ok(updatedReportingPeriod);
    }
}