using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class MonitoringWells
{
    public static List<MonitoringWellMeasurement> GetMonitoringWellsFromGeography(QanatDbContext dbContext, int geographyID)
    {
        var monitoringWells = dbContext.MonitoringWellMeasurements.AsNoTracking()
            .Include(x => x.MonitoringWell)
            .Where(x => x.GeographyID == geographyID)
            .ToList();
           
        return monitoringWells;
    }

    public static List<MonitoringWell> GetMonitoringWellsFromGeographyForGrid(QanatDbContext dbContext, int geographyID)
    {
        var monitoringWells = dbContext.MonitoringWells.AsNoTracking()
            .Include(x => x.MonitoringWellMeasurements)
            .Where(x => x.GeographyID == geographyID)
            .ToList();

        return monitoringWells;
    }

    public static List<MonitoringWellMeasurementDto> GetMonitoringWell(QanatDbContext dbContext, int geographyID, string siteCode)
    {
        var monitoringWellID =
            dbContext.MonitoringWells.Single(x => x.GeographyID == geographyID && x.SiteCode == siteCode).MonitoringWellID;
        var monitoringWellMeasurements = dbContext.MonitoringWellMeasurements
            .Include(x => x.MonitoringWell)
                .ThenInclude(x => x.Geography)
            .Where(x => x.MonitoringWellID == monitoringWellID).Select(x => x.AsMonitoringWellMeasurementDto()).ToList();
        return monitoringWellMeasurements;
    }
}