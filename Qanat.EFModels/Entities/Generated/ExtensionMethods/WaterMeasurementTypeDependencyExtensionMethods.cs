//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterMeasurementTypeDependency]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WaterMeasurementTypeDependencyExtensionMethods
    {
        public static WaterMeasurementTypeDependencySimpleDto AsSimpleDto(this WaterMeasurementTypeDependency waterMeasurementTypeDependency)
        {
            var dto = new WaterMeasurementTypeDependencySimpleDto()
            {
                WaterMeasurementTypeDependencyID = waterMeasurementTypeDependency.WaterMeasurementTypeDependencyID,
                GeographyID = waterMeasurementTypeDependency.GeographyID,
                WaterMeasurementTypeID = waterMeasurementTypeDependency.WaterMeasurementTypeID,
                DependsOnWaterMeasurementTypeID = waterMeasurementTypeDependency.DependsOnWaterMeasurementTypeID
            };
            return dto;
        }
    }
}