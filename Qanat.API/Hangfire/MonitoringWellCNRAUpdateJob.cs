using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.API.Services;
using Qanat.Common.GeoSpatial;
using Qanat.Common.Util;
using Qanat.EFModels.Entities;

namespace Qanat.API.Hangfire;

public class MonitoringWellCNRAUpdateJob : ScheduledBackgroundJobBase<MonitoringWellCNRAUpdateJob>
{
    private readonly MonitoringWellCNRAService _monitoringWellCNRAService;
    private readonly QanatDbContext _dbContext;

    public MonitoringWellCNRAUpdateJob(ILogger<MonitoringWellCNRAUpdateJob> logger,
        IWebHostEnvironment webHostEnvironment, QanatDbContext qanatDbContext,
        IOptions<QanatConfiguration> qanatConfiguration, MonitoringWellCNRAService monitoringWellCNRAService, SitkaSmtpClientService sitkaSmtpClientService) : base(JobName, logger, webHostEnvironment,
        qanatDbContext, qanatConfiguration, sitkaSmtpClientService)
    {
        _dbContext = qanatDbContext;
        _monitoringWellCNRAService = monitoringWellCNRAService;
    }

    public override List<RunEnvironment> RunEnvironments => new() { RunEnvironment.Development, RunEnvironment.Staging, RunEnvironment.Production };

    public const string JobName = "Monitoring Wells California Natural Resources Agency Update";

    protected override void RunJobImplementation()
    {
        var geographyBoundaries = _dbContext.GeographyBoundaries.ToList();
        foreach (var geographyBoundary in geographyBoundaries)
        {
            var geometry = geographyBoundary.GSABoundary;
            var resultsOffset = 0;
            var measurements = _monitoringWellCNRAService.RetrieveMeasurements(geometry, resultsOffset.ToString()).Result;
            var wellData = measurements.features.ToList();
            
            while (measurements.features.Count == 2000)
            {
                resultsOffset += 2000;
                measurements = _monitoringWellCNRAService.RetrieveMeasurements(geometry, resultsOffset.ToString()).Result;
                wellData.AddRange(measurements.features);
            }

            // filtering out null measurements
            wellData = wellData.Where(x => x.properties.WSE.HasValue).ToList();

            // grouping by site code and filtering out any wells with only one measurement
            var measurementsBySiteCode = wellData
                .GroupBy(x => x.properties.SITE_CODE)
                .Where(x => x.Count() > 1);

            var existingMonitorWells =
                _dbContext.MonitoringWells.Include(x => x.MonitoringWellMeasurements)
                    .Where(x => x.MonitoringWellSourceTypeID == (int)MonitoringWellSourceTypeEnum.CNRA && x.GeographyID == geographyBoundary.GeographyID).ToList();

            var updatedMonitoringWells = measurementsBySiteCode.Select(x => new MonitoringWell()
            {
                GeographyID = geographyBoundary.GeographyID,
                SiteCode = x.First().properties.SITE_CODE,
                MonitoringWellName = x.First().properties.WELL_NAME,
                MonitoringWellSourceTypeID = (int)MonitoringWellSourceTypeEnum.CNRA,
                Geometry = GeometryHelper.CreateLocationPoint4326FromLatLong(x.First().geometry.coordinates[1],
                    x.First().geometry.coordinates[0])
            }).ToList();

            existingMonitorWells.Merge(updatedMonitoringWells, _dbContext.MonitoringWells,
                (x, y) => x.SiteCode == y.SiteCode && x.GeographyID == y.GeographyID,
                (x, y) => { x.Geometry = y.Geometry; });
            _dbContext.SaveChanges();

            var monitorWellsDB = _dbContext.MonitoringWells
                .Include(x => x.Geography)
                .Where(x => x.MonitoringWellSourceTypeID == (int)MonitoringWellSourceTypeEnum.CNRA 
                            && x.GeographyID == geographyBoundary.GeographyID).ToList();

            var existingMonitorWellMeasurements = _dbContext.MonitoringWellMeasurements
                .Include(x => x.MonitoringWell)
                .Where(x => x.MonitoringWell.MonitoringWellSourceTypeID == (int)MonitoringWellSourceTypeEnum.CNRA 
                            && x.MonitoringWell.GeographyID == geographyBoundary.GeographyID)
                .ToList();

            var updatedMonitoringWellMeasurements = wellData
                .Select(x =>
                {
                    var monitoringWellID = monitorWellsDB
                        .SingleOrDefault(y => y.SiteCode == x.properties.SITE_CODE)?.MonitoringWellID;

                    if (!monitoringWellID.HasValue) return null;

                    return new MonitoringWellMeasurement()
                    {
                        Measurement = x.properties.WSE.Value,
                        MeasurementDate = x.properties.MeasurementDate,
                        MonitoringWellID = monitoringWellID.Value,
                        ExtenalUniqueID = x.properties.OBJECTID,
                        GeographyID = geographyBoundary.GeographyID
                    };
                })
                .Where(x => x != null)
                .ToList();

            existingMonitorWellMeasurements.Merge(updatedMonitoringWellMeasurements,
                _dbContext.MonitoringWellMeasurements,
                (x, y) => x.ExtenalUniqueID == y.ExtenalUniqueID && x.GeographyID == y.GeographyID,
                (x, y) =>
                {
                    x.Measurement = y.Measurement;
                    x.MeasurementDate = y.MeasurementDate;
                    x.MonitoringWellID = y.MonitoringWellID;
                }
            );

            _dbContext.SaveChanges();
        }
    }
}