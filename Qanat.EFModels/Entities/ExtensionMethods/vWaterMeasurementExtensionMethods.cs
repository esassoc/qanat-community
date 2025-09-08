using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class vWaterMeasurementExtensionMethods
{
    public static WaterMeasurementDto AsWaterMeasurementDto(this vWaterMeasurement vWaterMeasurement)
    {
        var dto = new WaterMeasurementDto()
        {
            WaterMeasurementID = vWaterMeasurement.WaterMeasurementID,
            GeographyID = vWaterMeasurement.GeographyID,

            WaterMeasurementTypeID = vWaterMeasurement.WaterMeasurementTypeID,
            WaterMeasurementTypeName = vWaterMeasurement.WaterMeasurementTypeName,

            UnitTypeID = vWaterMeasurement.UnitTypeID,
            UnitTypeName = vWaterMeasurement.UnitTypeDisplayName,

            UsageLocationID = vWaterMeasurement.UsageLocationID,
            UsageLocationName = vWaterMeasurement.UsageLocationName,
            UsageLocationArea = (decimal)vWaterMeasurement.UsageLocationArea,

            ParcelID = vWaterMeasurement.ParcelID,
            ParcelNumber = vWaterMeasurement.ParcelNumber,

            WaterAccountID = vWaterMeasurement.WaterAccountID,
            WaterAccountNumberAndName = vWaterMeasurement.WaterAccountName == null ? $"#{vWaterMeasurement.WaterAccountNumber}" : $"#{vWaterMeasurement.WaterAccountNumber} ({vWaterMeasurement.WaterAccountName})",

            ReportedDate = vWaterMeasurement.ReportedDate,
            ReportedValueInNativeUnits = vWaterMeasurement.ReportedValueInNativeUnits,
            ReportedValueInFeet = vWaterMeasurement.ReportedValueInFeet,
            ReportedValueInAcreFeet = vWaterMeasurement.ReportedValueInAcreFeet,

            LastUpdateDate = vWaterMeasurement.LastUpdateDate,
            FromManualUpload = vWaterMeasurement.FromManualUpload,
        };
        return dto;
    }
}