using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Hangfire;
using Qanat.API.Services;
using Qanat.API.Services.Authorization;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.Security;

namespace Qanat.API.Controllers;

[ApiController]
[RightsChecker]
public class MonitoringWellController : SitkaController<MonitoringWellController>
{
    private readonly YoloWellWRIDService _yoloWellWRIDService;
    public MonitoringWellController(QanatDbContext dbContext, ILogger<MonitoringWellController> logger, IOptions<QanatConfiguration> qanatConfiguration, YoloWellWRIDService yoloWellWRIDService) : base(dbContext, logger, qanatConfiguration)
    {
        _yoloWellWRIDService = yoloWellWRIDService;
    }

    [HttpGet("geographies/{geographyID}/monitoring-well-measurements")]
    [WithRolePermission(PermissionEnum.MonitoringWellRights, RightsEnum.Read)]
    public ActionResult<List<MonitoringWellMeasurementDataDto>> GetAllMonitoringWellsForGeography([FromRoute] int geographyID)
    {
        var monitoringWells = MonitoringWells.GetMonitoringWellsFromGeography(_dbContext, geographyID);
        return Ok(monitoringWells.Select(x => x.AsMonitoringWellMeasurementDataDto()));
    }

    [HttpGet("geographies/{geographyID}/monitoring-wells")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)] // todo: better permissions, need to add water account users to the geography users table as a normal user with basic read rights to geography entities
    public ActionResult<List<MonitoringWellDataDto>> GetAllMonitoringWellsForGeographyForGrid([FromRoute] int geographyID)
    {
        var monitoringWells = MonitoringWells.GetMonitoringWellsFromGeographyForGrid(_dbContext, geographyID);
        return Ok(monitoringWells.Select(x => x.AsMonitoringWellDataDto()));
    }

    [HttpGet("geographies/{geographyID}/monitoring-well/{siteCode}")]
    [WithRoleFlag(FlagEnum.CanClaimWaterAccounts)] // todo: better permissions, need to add water account users to the geography users table as a normal user with basic read rights to geography entities
    public ActionResult<List<MonitoringWellMeasurementDto>> GetMonitoringWellByGeographyAndSiteCode([FromRoute] int geographyID, string siteCode)
    {
        var monitoringWellMeasurements = MonitoringWells.GetMonitoringWell(_dbContext, geographyID, siteCode);
        return Ok(monitoringWellMeasurements);
    }

    [HttpPost("monitoring-well-measurements")]
    [WithRolePermission(PermissionEnum.MonitoringWellRights, RightsEnum.Update)]
    public ActionResult UpdateMonitoringWellData()
    {
        HangfireJobScheduler.EnqueueRecurringJob(MonitoringWellCNRAUpdateJob.JobName);
        return Ok();
    }

    [HttpGet("monitoring-wells/yolo-wrid/retrieve")]
    [WithRolePermission(PermissionEnum.MonitoringWellRights, RightsEnum.Update)]
    public ActionResult RetrieveYoloWRIDWellsAndMeasurements()
    {
        BackgroundJob.Enqueue(() => _yoloWellWRIDService.RetrieveScadaWellsAndMeasurements());
        return Ok();
    }
}