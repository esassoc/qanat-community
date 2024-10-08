//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[MonitoringWellMeasurement]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class MonitoringWellMeasurementExtensionMethods
    {
        public static MonitoringWellMeasurementSimpleDto AsSimpleDto(this MonitoringWellMeasurement monitoringWellMeasurement)
        {
            var dto = new MonitoringWellMeasurementSimpleDto()
            {
                MonitoringWellMeasurementID = monitoringWellMeasurement.MonitoringWellMeasurementID,
                MonitoringWellID = monitoringWellMeasurement.MonitoringWellID,
                GeographyID = monitoringWellMeasurement.GeographyID,
                ExtenalUniqueID = monitoringWellMeasurement.ExtenalUniqueID,
                Measurement = monitoringWellMeasurement.Measurement,
                MeasurementDate = monitoringWellMeasurement.MeasurementDate
            };
            return dto;
        }
    }
}