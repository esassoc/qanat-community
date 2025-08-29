using System;
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
[Route("geographies/{geographyID}/water-accounts/{waterAccountID}/water-measurement-self-reports")]
public class WaterMeasurementSelfReportController(QanatDbContext dbContext, ILogger<WaterMeasurementSelfReportController> logger, IOptions<QanatConfiguration> qanatConfiguration, UserDto callingUser)
    : SitkaController<WaterMeasurementSelfReportController>(dbContext, logger, qanatConfiguration)
{

    #region CreateAsync, Read, UpdateAsync

    [HttpPost]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)] //Admin
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)] //Geography Manager
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Create)] //Water Account Holder
    public async Task<ActionResult<WaterMeasurementSelfReportSimpleDto>> Create([FromRoute] int geographyID, [FromRoute] int waterAccountID, [FromBody] WaterMeasurementSelfReportCreateDto waterMeasurementSelfReportCreateDto)
    {
        var validationErrors = await WaterMeasurementSelfReports.ValidateCreateAsync(_dbContext, geographyID, waterAccountID, waterMeasurementSelfReportCreateDto);
        if (validationErrors.Any() || !ModelState.IsValid)
        {
            validationErrors.ForEach(ve => ModelState.AddModelError(ve.Type, ve.Message));
            return BadRequest(ModelState);
        }

        var newSelfReportAsSimpleDto = await WaterMeasurementSelfReports.CreateAsync(_dbContext, geographyID, waterAccountID, waterMeasurementSelfReportCreateDto, callingUser);
        return Ok(newSelfReportAsSimpleDto);
    }

    [HttpGet("reporting-periods/{reportingPeriodID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [EntityNotFound(typeof(ReportingPeriod), "reportingPeriodID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)] //Admin
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)] //Geography Manager
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)] //Water Account Viewer/Holder
    public async Task<ActionResult<List<WaterMeasurementSelfReportSimpleDto>>> ReadListByYear([FromRoute] int geographyID, [FromRoute] int waterAccountID, [FromRoute] int reportingPeriodID)
    {
        var selfReportSimpleDtos = await WaterMeasurementSelfReports.ListAsSimpleDtoForWaterAccountAsync(_dbContext, geographyID, waterAccountID, reportingPeriodID);
        return Ok(selfReportSimpleDtos);
    }

    [HttpGet("{waterMeasurementSelfReportID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [EntityNotFound(typeof(WaterMeasurementSelfReport), "waterMeasurementSelfReportID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)] //Admin
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)] //Geography Manager
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Read)] //Water Account Viewer/Holder
    public async Task<ActionResult<WaterMeasurementSelfReportDto>> ReadSingle([FromRoute] int geographyID, [FromRoute] int waterAccountID, [FromRoute] int waterMeasurementSelfReportID)
    {
        var waterMeasurementSelfReportDto = await WaterMeasurementSelfReports.GetSingleAsDtoAsync(_dbContext, geographyID, waterAccountID, waterMeasurementSelfReportID);
        return Ok(waterMeasurementSelfReportDto);
    }

    [HttpPut("{waterMeasurementSelfReportID}")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [EntityNotFound(typeof(WaterMeasurementSelfReport), "waterMeasurementSelfReportID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)] //Admin
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)] //Geography Manager
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Update)] //Water Account Holder
    public async Task<ActionResult<WaterMeasurementSelfReportDto>> Update([FromRoute] int geographyID, [FromRoute] int waterAccountID, [FromRoute] int waterMeasurementSelfReportID, [FromBody] WaterMeasurementSelfReportUpdateDto waterMeasurementSelfReportUpdateDto)
    {
        var validationErrors = await WaterMeasurementSelfReports.ValidateUpdateAsync(_dbContext, geographyID, waterAccountID, waterMeasurementSelfReportID, waterMeasurementSelfReportUpdateDto, callingUser);
        if (validationErrors.Any() || !ModelState.IsValid)
        {
            validationErrors.ForEach(ve => ModelState.AddModelError(ve.Type, ve.Message));
            return BadRequest(ModelState);
        }

        var updatedSelfReportAsDto = await WaterMeasurementSelfReports.UpdateAsync(_dbContext, geographyID, waterAccountID, waterMeasurementSelfReportID, waterMeasurementSelfReportUpdateDto, callingUser);
        return Ok(updatedSelfReportAsDto);
    }

    #endregion

    #region Workflow (Submit, Approve, Return)

    [HttpPut("{waterMeasurementSelfReportID}/submit")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [EntityNotFound(typeof(WaterMeasurementSelfReport), "waterMeasurementSelfReportID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)] //Admin
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)] //Geography Manager
    [WithWaterAccountRolePermission(PermissionEnum.WaterAccountRights, RightsEnum.Update)] //Water Account Holder
    public async Task<ActionResult<WaterMeasurementSelfReportDto>> Submit([FromRoute] int geographyID, [FromRoute] int waterAccountID, [FromRoute] int waterMeasurementSelfReportID)
    {
        var validationErrors = await WaterMeasurementSelfReports.ValidateSubmitAsync(_dbContext, geographyID, waterAccountID, waterMeasurementSelfReportID);
        if (validationErrors.Any() || !ModelState.IsValid)
        {
            validationErrors.ForEach(ve => ModelState.AddModelError(ve.Type, ve.Message));
            return BadRequest(ModelState);
        }

        var updatedSelfReportAsDto = await WaterMeasurementSelfReports.SubmitAsync(_dbContext, geographyID, waterAccountID, waterMeasurementSelfReportID, callingUser);
        return Ok(updatedSelfReportAsDto);
    }

    [HttpPut("{waterMeasurementSelfReportID}/approve")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [EntityNotFound(typeof(WaterMeasurementSelfReport), "waterMeasurementSelfReportID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)] //Admin
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)] //Geography Manager
    public async Task<ActionResult<WaterMeasurementSelfReportDto>> Approve([FromRoute] int geographyID, [FromRoute] int waterAccountID, [FromRoute] int waterMeasurementSelfReportID)
    {
        var validationErrors = await WaterMeasurementSelfReports.ValidateApproveAsync(_dbContext, geographyID, waterAccountID, waterMeasurementSelfReportID);
        if (validationErrors.Any() || !ModelState.IsValid)
        {
            validationErrors.ForEach(ve => ModelState.AddModelError(ve.Type, ve.Message));
            return BadRequest(ModelState);
        }

        ApproveSelfReportResult calculationsToRun = null;
        try
        { 
            calculationsToRun = await WaterMeasurementSelfReports.ApproveAsync(_dbContext, geographyID, waterAccountID, waterMeasurementSelfReportID, callingUser);
        }
        catch (Exception ex)
        {
            //MK 1/17/2025 -- If we had an error approving, we should set the status back to submitted. Probably could improve the unit of work pattern to handle this better w/o a try/catch and reset.
            //Right now it will overwrite the previous submitter/submit date which is not ideal, but without we could get in a state where the users are stuck in approved and we haven't calculated the dependant water measurements.
            await WaterMeasurementSelfReports.SubmitAsync(_dbContext, geographyID, waterAccountID, waterMeasurementSelfReportID, callingUser);
            return BadRequest(ex.Message);
        }

        foreach (var waterMeasurementTypeID in calculationsToRun.WaterMeasurementTypeIDs)
        {
            foreach (var reportedDate in calculationsToRun.ReportedDates)
            {
                await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(_dbContext, geographyID, waterMeasurementTypeID, reportedDate);
            }
        }

        var updatedSelfReportAsDto = await WaterMeasurementSelfReports.GetSingleAsDtoAsync(_dbContext, geographyID, waterAccountID, waterMeasurementSelfReportID);
        return Ok(updatedSelfReportAsDto);
    }

    [HttpPut("{waterMeasurementSelfReportID}/return")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [EntityNotFound(typeof(WaterAccount), "waterAccountID")]
    [EntityNotFound(typeof(WaterMeasurementSelfReport), "waterMeasurementSelfReportID")]
    [WithRoleFlag(FlagEnum.IsSystemAdmin)] //Admin
    [WithGeographyRoleFlag(FlagEnum.HasManagerDashboard)] //Geography Manager
    public async Task<ActionResult<WaterMeasurementSelfReportDto>> Return([FromRoute] int geographyID, [FromRoute] int waterAccountID, [FromRoute] int waterMeasurementSelfReportID)
    {
        var validationErrors = await WaterMeasurementSelfReports.ValidateReturnAsync(_dbContext, geographyID, waterAccountID, waterMeasurementSelfReportID);
        if (validationErrors.Any() || !ModelState.IsValid)
        {
            validationErrors.ForEach(ve => ModelState.AddModelError(ve.Type, ve.Message));
            return BadRequest(ModelState);
        }

        var updatedSelfReportAsDto = await WaterMeasurementSelfReports.ReturnAsync(_dbContext, geographyID, waterAccountID, waterMeasurementSelfReportID, callingUser);
        return Ok(updatedSelfReportAsDto);
    }

    #endregion
}