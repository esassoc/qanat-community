//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelCustomAttribute]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ParcelCustomAttributeExtensionMethods
    {
        public static ParcelCustomAttributeSimpleDto AsSimpleDto(this ParcelCustomAttribute parcelCustomAttribute)
        {
            var dto = new ParcelCustomAttributeSimpleDto()
            {
                ParcelCustomAttributeID = parcelCustomAttribute.ParcelCustomAttributeID,
                ParcelID = parcelCustomAttribute.ParcelID,
                CustomAttributes = parcelCustomAttribute.CustomAttributes
            };
            return dto;
        }
    }
}