//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[FuelType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class FuelTypeExtensionMethods
    {
        public static FuelTypeSimpleDto AsSimpleDto(this FuelType fuelType)
        {
            var dto = new FuelTypeSimpleDto()
            {
                FuelTypeID = fuelType.FuelTypeID,
                FuelTypeName = fuelType.FuelTypeName,
                FuelTypeDisplayName = fuelType.FuelTypeDisplayName
            };
            return dto;
        }
    }
}