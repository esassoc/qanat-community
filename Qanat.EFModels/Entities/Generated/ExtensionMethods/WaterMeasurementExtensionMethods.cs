//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurement]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WaterMeasurementExtensionMethods
    {
        public static WaterMeasurementSimpleDto AsSimpleDto(this WaterMeasurement waterMeasurement)
        {
            var dto = new WaterMeasurementSimpleDto()
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
                Comment = waterMeasurement.Comment
            };
            return dto;
        }
    }
}