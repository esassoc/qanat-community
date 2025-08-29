using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class MonitoringWellExtensionMethods
{
    public static MonitoringWellSimpleDto AsSimpleDto(this MonitoringWell monitoringWell)
    {
        var dto = new MonitoringWellSimpleDto()
        {
            MonitoringWellID = monitoringWell.MonitoringWellID,
            GeographyID = monitoringWell.GeographyID,
            SiteCode = monitoringWell.SiteCode,
            MonitoringWellName = monitoringWell.MonitoringWellName,
            MonitoringWellSourceTypeID = monitoringWell.MonitoringWellSourceTypeID
        };
        return dto;
    }

    public static MonitoringWellDataDto AsMonitoringWellDataDto(
        this MonitoringWell monitoringWell)
    {
        return new MonitoringWellDataDto()
        {
            GeographyID = monitoringWell.GeographyID,
            MonitoringWellID = monitoringWell.MonitoringWellID,
            MonitoringWellName = monitoringWell.MonitoringWellName,
            SiteCode = monitoringWell.SiteCode,
            MonitoringWellSourceTypeDisplayName = monitoringWell.MonitoringWellSourceType.MonitoringWellSourceTypeDisplayName,
            Latitude = monitoringWell.Geometry.InteriorPoint.X,
            Longitude = monitoringWell.Geometry.InteriorPoint.Y,
            NumberOfMeasurements = monitoringWell.MonitoringWellMeasurements.Count,
            EarliestMeasurementDate = monitoringWell.MonitoringWellMeasurements.MinBy(x => x.MeasurementDate)?.MeasurementDate,
            LastMeasurementDate = monitoringWell.MonitoringWellMeasurements.MaxBy(x => x.MeasurementDate)?.MeasurementDate,
            EarliestMeasurement = monitoringWell.MonitoringWellMeasurements.MinBy(x => x.MeasurementDate)?.Measurement,
            LastMeasurement = monitoringWell.MonitoringWellMeasurements.MaxBy(x => x.MeasurementDate)?.Measurement
        };
    }
}