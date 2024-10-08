using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static partial class WaterMeasurementExtensionMethods
{
    public static WaterMeasurementDto AsWaterMeasurementDto(this WaterMeasurement waterMeasurement)
    {
        var dto = new WaterMeasurementDto()
        {
            WaterMeasurementID = waterMeasurement.WaterMeasurementID,
            GeographyID = waterMeasurement.GeographyID,
            WaterMeasurementTypeID = waterMeasurement.WaterMeasurementTypeID,
            UnitTypeID = waterMeasurement.UnitTypeID,
            UsageEntityName = waterMeasurement.UsageEntityName,
            ReportedDate = waterMeasurement.ReportedDate,
            ReportedValue = waterMeasurement.ReportedValue,
            ReportedValueInAcreFeet = waterMeasurement.ReportedValueInAcreFeet,
            UsageEntityArea = waterMeasurement.UsageEntityArea,
            LastUpdateDate = waterMeasurement.LastUpdateDate,
            FromManualUpload = waterMeasurement.FromManualUpload,
            UnitTypeName = waterMeasurement.UnitType?.UnitTypeDisplayName,
            WaterMeasurementTypeName = waterMeasurement.WaterMeasurementType?.WaterMeasurementTypeName,
            WaterMeasurementCategoryTypeName = waterMeasurement.WaterMeasurementType?.WaterMeasurementCategoryType?.WaterMeasurementCategoryTypeName
        };
        return dto;
    }
}