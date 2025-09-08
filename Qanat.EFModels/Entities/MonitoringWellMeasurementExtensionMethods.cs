using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static partial class MonitoringWellMeasurementExtensionMethods
{
    public static MonitoringWellMeasurementDataDto AsMonitoringWellMeasurementDataDto(
        this MonitoringWellMeasurement monitoringWellMeasurement)
    {
        return new MonitoringWellMeasurementDataDto()
        {
            MonitoringWellMeasurementID = monitoringWellMeasurement.MonitoringWellMeasurementID,
            ExtenalUniqueID = monitoringWellMeasurement.ExtenalUniqueID,
            GeographyID = monitoringWellMeasurement.GeographyID,
            Measurement = monitoringWellMeasurement.Measurement,
            MeasurementDate = monitoringWellMeasurement.MeasurementDate,
            MonitoringWellID = monitoringWellMeasurement.MonitoringWellID,
            MonitoringWellName = monitoringWellMeasurement.MonitoringWell.MonitoringWellName,
            SiteCode = monitoringWellMeasurement.MonitoringWell.SiteCode
        };
    }

    public static MonitoringWellMeasurementDto AsMonitoringWellMeasurementDto(this MonitoringWellMeasurement monitoringWellMeasurement)
    {
        var monitoringWellMeasurementDto = new MonitoringWellMeasurementDto()
        {
            MonitoringWellMeasurementID = monitoringWellMeasurement.MonitoringWellMeasurementID,
            MonitoringWell = monitoringWellMeasurement.MonitoringWell.AsSimpleDto(),
            Geography = monitoringWellMeasurement.Geography.AsSimpleDto(),
            ExtenalUniqueID = monitoringWellMeasurement.ExtenalUniqueID,
            Measurement = monitoringWellMeasurement.Measurement,
            MeasurementDate = monitoringWellMeasurement.MeasurementDate
        };
        return monitoringWellMeasurementDto;
    }
}