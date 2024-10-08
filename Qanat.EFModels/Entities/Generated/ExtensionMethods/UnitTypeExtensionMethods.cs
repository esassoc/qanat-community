//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[UnitType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class UnitTypeExtensionMethods
    {
        public static UnitTypeSimpleDto AsSimpleDto(this UnitType unitType)
        {
            var dto = new UnitTypeSimpleDto()
            {
                UnitTypeID = unitType.UnitTypeID,
                UnitTypeName = unitType.UnitTypeName,
                UnitTypeDisplayName = unitType.UnitTypeDisplayName,
                UnitTypeAbbreviation = unitType.UnitTypeAbbreviation
            };
            return dto;
        }
    }
}