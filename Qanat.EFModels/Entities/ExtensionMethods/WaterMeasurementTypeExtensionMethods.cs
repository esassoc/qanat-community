using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class WaterMeasurementTypeExtensionMethods
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
                ShortName = waterMeasurementType.ShortName,
                SortOrder = waterMeasurementType.SortOrder,
                IsUserEditable = waterMeasurementType.IsUserEditable,
                IsSelfReportable = waterMeasurementType.IsSelfReportable,
                ShowToLandowner = waterMeasurementType.ShowToLandowner,
                WaterMeasurementCalculationTypeID = waterMeasurementType.WaterMeasurementCalculationTypeID,
                CalculationJSON = waterMeasurementType.CalculationJSON
            };
            return dto;
        }

        public static WaterMeasurementTypeSimpleDto AsSimpleDtoWithExtras(this WaterMeasurementType waterMeasurementType)
        {
            var dto = AsSimpleDto(waterMeasurementType);
            dto.WaterMeasurementCategoryName = waterMeasurementType.WaterMeasurementCategoryType?.WaterMeasurementCategoryTypeDisplayName;
            dto.WaterMeasurementCalculationName = waterMeasurementType.WaterMeasurementCalculationType?.WaterMeasurementCalculationTypeDisplayName;
            dto.IsSourceOfRecord = waterMeasurementType.WaterMeasurementTypeID == waterMeasurementType.Geography.SourceOfRecordWaterMeasurementTypeID;
            return dto;
        }
    }
}