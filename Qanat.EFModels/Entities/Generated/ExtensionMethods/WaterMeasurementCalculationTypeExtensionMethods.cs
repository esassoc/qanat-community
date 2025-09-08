//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurementCalculationType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WaterMeasurementCalculationTypeExtensionMethods
    {
        public static WaterMeasurementCalculationTypeSimpleDto AsSimpleDto(this WaterMeasurementCalculationType waterMeasurementCalculationType)
        {
            var dto = new WaterMeasurementCalculationTypeSimpleDto()
            {
                WaterMeasurementCalculationTypeID = waterMeasurementCalculationType.WaterMeasurementCalculationTypeID,
                WaterMeasurementCalculationTypeName = waterMeasurementCalculationType.WaterMeasurementCalculationTypeName,
                WaterMeasurementCalculationTypeDisplayName = waterMeasurementCalculationType.WaterMeasurementCalculationTypeDisplayName
            };
            return dto;
        }
    }
}