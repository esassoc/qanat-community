//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[CustomAttributeType]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class CustomAttributeTypeExtensionMethods
    {
        public static CustomAttributeTypeSimpleDto AsSimpleDto(this CustomAttributeType customAttributeType)
        {
            var dto = new CustomAttributeTypeSimpleDto()
            {
                CustomAttributeTypeID = customAttributeType.CustomAttributeTypeID,
                CustomAttributeTypeName = customAttributeType.CustomAttributeTypeName,
                CustomAttributeTypeDisplayName = customAttributeType.CustomAttributeTypeDisplayName
            };
            return dto;
        }
    }
}