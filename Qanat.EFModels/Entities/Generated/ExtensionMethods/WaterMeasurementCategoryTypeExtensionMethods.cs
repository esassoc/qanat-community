//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurementCategoryType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WaterMeasurementCategoryTypeExtensionMethods
    {
        public static WaterMeasurementCategoryTypeSimpleDto AsSimpleDto(this WaterMeasurementCategoryType waterMeasurementCategoryType)
        {
            var dto = new WaterMeasurementCategoryTypeSimpleDto()
            {
                WaterMeasurementCategoryTypeID = waterMeasurementCategoryType.WaterMeasurementCategoryTypeID,
                WaterMeasurementCategoryTypeName = waterMeasurementCategoryType.WaterMeasurementCategoryTypeName,
                WaterMeasurementCategoryTypeDisplayName = waterMeasurementCategoryType.WaterMeasurementCategoryTypeDisplayName
            };
            return dto;
        }
    }
}