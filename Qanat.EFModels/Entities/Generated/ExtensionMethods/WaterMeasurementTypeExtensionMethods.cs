//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurementType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WaterMeasurementTypeExtensionMethods
    {
        public static WaterMeasurementTypeSimpleDto AsSimpleDto(this WaterMeasurementType waterMeasurementType)
        {
            var dto = new WaterMeasurementTypeSimpleDto()
            {
                WaterMeasurementTypeID = waterMeasurementType.WaterMeasurementTypeID,
                GeographyID = waterMeasurementType.GeographyID,
                WaterMeasurementCategoryTypeID = waterMeasurementType.WaterMeasurementCategoryTypeID,
                IsActive = waterMeasurementType.IsActive,
                WaterMeasurementTypeName = waterMeasurementType.WaterMeasurementTypeName,
                SortOrder = waterMeasurementType.SortOrder,
                IsUserEditable = waterMeasurementType.IsUserEditable,
                IsSelfReportable = waterMeasurementType.IsSelfReportable,
                ShowToLandowner = waterMeasurementType.ShowToLandowner,
                WaterMeasurementCalculationTypeID = waterMeasurementType.WaterMeasurementCalculationTypeID,
                CalculationJSON = waterMeasurementType.CalculationJSON
            };
            return dto;
        }
    }
}