//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WaterTypeExtensionMethods
    {
        public static WaterTypeSimpleDto AsSimpleDto(this WaterType waterType)
        {
            var dto = new WaterTypeSimpleDto()
            {
                WaterTypeID = waterType.WaterTypeID,
                GeographyID = waterType.GeographyID,
                IsActive = waterType.IsActive,
                WaterTypeName = waterType.WaterTypeName,
                IsAppliedProportionally = waterType.IsAppliedProportionally,
                WaterTypeDefinition = waterType.WaterTypeDefinition,
                IsSourcedFromApi = waterType.IsSourcedFromApi,
                SortOrder = waterType.SortOrder,
                WaterTypeSlug = waterType.WaterTypeSlug,
                WaterTypeColor = waterType.WaterTypeColor
            };
            return dto;
        }
    }
}