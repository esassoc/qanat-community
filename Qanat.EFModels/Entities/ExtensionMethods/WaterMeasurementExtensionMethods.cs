using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WaterMeasurementExtensionMethods
{
    public static WaterMeasurementSimpleDto AsSimpleDto(this WaterMeasurement waterMeasurement)
    {
        var dto = new WaterMeasurementSimpleDto()
        {
            WaterMeasurementID = waterMeasurement.WaterMeasurementID,
            GeographyID = waterMeasurement.GeographyID,
            UsageLocationID = waterMeasurement.UsageLocationID,
            WaterMeasurementTypeID = waterMeasurement.WaterMeasurementTypeID,
            UnitTypeID = waterMeasurement.UnitTypeID,
            ReportedDate = waterMeasurement.ReportedDate,
            ReportedValueInNativeUnits = waterMeasurement.ReportedValueInNativeUnits,
            ReportedValueInAcreFeet = waterMeasurement.ReportedValueInAcreFeet,
            ReportedValueInFeet = waterMeasurement.ReportedValueInFeet,
            LastUpdateDate = waterMeasurement.LastUpdateDate,
            FromManualUpload = waterMeasurement.FromManualUpload,
            Comment = waterMeasurement.Comment
        };
        return dto;
    }

    public static WaterMeasurementDto AsWaterMeasurementDto(this WaterMeasurement waterMeasurement)
    {
        var dto = new WaterMeasurementDto()
        {
            WaterMeasurementID = waterMeasurement.WaterMeasurementID,
            GeographyID = waterMeasurement.GeographyID,

            WaterMeasurementTypeID = waterMeasurement.WaterMeasurementTypeID,
            WaterMeasurementTypeName = waterMeasurement.WaterMeasurementType?.WaterMeasurementTypeName,
            WaterMeasurementCategoryTypeName = waterMeasurement.WaterMeasurementType?.WaterMeasurementCategoryType?.WaterMeasurementCategoryTypeName,

            UnitTypeID = waterMeasurement.UnitTypeID,
            UnitTypeName = waterMeasurement.UnitType?.UnitTypeDisplayName,

            UsageLocationID = waterMeasurement.UsageLocationID,
            UsageLocationName = waterMeasurement.UsageLocation.Name,
            UsageLocationArea = (decimal)waterMeasurement.UsageLocation.Area,

            ReportedDate = waterMeasurement.ReportedDate,
            ReportedValueInNativeUnits = waterMeasurement.ReportedValueInNativeUnits,
            ReportedValueInFeet = waterMeasurement.ReportedValueInFeet,
            ReportedValueInAcreFeet = waterMeasurement.ReportedValueInAcreFeet,

            LastUpdateDate = waterMeasurement.LastUpdateDate,
            FromManualUpload = waterMeasurement.FromManualUpload,
        };
        return dto;
    }
}