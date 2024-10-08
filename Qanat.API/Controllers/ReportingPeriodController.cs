using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
public class ReportingPeriodController : SitkaController<ReportingPeriodController>
{
    public ReportingPeriodController(QanatDbContext dbContext, ILogger<ReportingPeriodController> logger, IOptions<QanatConfiguration> qanatConfiguration) : base(dbContext, logger, qanatConfiguration)
    {
    }

    [HttpGet("geographies/{geographyID}/reporting-period")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ReportingPeriodRights, RightsEnum.Read)]
    public ActionResult<ReportingPeriodSimpleDto> GetReportingPeriodForGeography([FromRoute] int geographyID)
    {
        var reportingPeriod = ReportingPeriods.GetReportingPeriodForGeographyAsSimpleDto(_dbContext, geographyID);
        return Ok(reportingPeriod);
    }

    [HttpGet("geographies/{geographyID}/reporting-period/years")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ReportingPeriodRights, RightsEnum.Read)]
    public ActionResult<List<int>> GetYearsForReportingPeriod([FromRoute] int geographyID)
    {
        var reportingPeriod = ReportingPeriods.GetByGeographyID(_dbContext, geographyID);
        if (reportingPeriod == null)
        {
            return BadRequest(
                ("A reporting period has not been configured for this geography. Please update settings on the Configure dashboard.")
            );
        }

        var startingYear = Geographies.GetByID(_dbContext, reportingPeriod.GeographyID).StartYear;
        var currentYear = DateTime.Now.Year;

        var allYearsInReportingPeriod = new List<int>();
        for (var i = currentYear; i >= startingYear; i--)
            allYearsInReportingPeriod.Add(i);

        return Ok(allYearsInReportingPeriod);
    }

    [HttpGet("reporting-periods/years")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)]
    public ActionResult<List<int>> GetYearsForAllReportingPeriods()
    {
        var startingYear = _dbContext.Geographies.AsNoTracking().ToList()
            .MinBy(x => x.StartYear)?.StartYear;
        var currentYear = DateTime.Now.Year;

        var allYearsInAllReportingPeriods = new List<int>();
        for (var i = currentYear; i >= startingYear; i--)
            allYearsInAllReportingPeriods.Add(i);

        return Ok(allYearsInAllReportingPeriods);
    }

    [HttpPost("geographies/{geographyID}/reporting-period/update")]
    [EntityNotFound(typeof(Geography), "geographyID")]
    [WithGeographyRolePermission(PermissionEnum.ReportingPeriodRights, RightsEnum.Update)]
    public ActionResult<ReportingPeriodSimpleDto> UpdateReportingPeriod([FromBody] ReportingPeriodSimpleDto reportingPeriodSimpleDto, [FromRoute] int geographyID)
    {
        var existingReportingPeriod = _dbContext.ReportingPeriods.SingleOrDefault(x => x.GeographyID == geographyID);

        if (existingReportingPeriod != null)
        {
            existingReportingPeriod.ReportingPeriodName = reportingPeriodSimpleDto.ReportingPeriodName;
            existingReportingPeriod.StartMonth = reportingPeriodSimpleDto.StartMonth;
            existingReportingPeriod.Interval = reportingPeriodSimpleDto.Interval;
        }
        else
        {
            var newReportingPeriod = new ReportingPeriod()
            {
                GeographyID = geographyID,
                ReportingPeriodName = reportingPeriodSimpleDto.ReportingPeriodName,
                StartMonth = reportingPeriodSimpleDto.StartMonth,
                Interval = reportingPeriodSimpleDto.Interval
            };

            _dbContext.ReportingPeriods.Add(newReportingPeriod);
        }

        _dbContext.SaveChanges();

        return Ok(ReportingPeriods.GetByGeographyID(_dbContext, geographyID));
    }
}