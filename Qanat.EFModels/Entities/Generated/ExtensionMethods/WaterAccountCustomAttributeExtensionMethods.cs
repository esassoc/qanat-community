//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterAccountCustomAttribute]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class WaterAccountCustomAttributeExtensionMethods
    {
        public static WaterAccountCustomAttributeSimpleDto AsSimpleDto(this WaterAccountCustomAttribute waterAccountCustomAttribute)
        {
            var dto = new WaterAccountCustomAttributeSimpleDto()
            {
                WaterAccountCustomAttributeID = waterAccountCustomAttribute.WaterAccountCustomAttributeID,
                WaterAccountID = waterAccountCustomAttribute.WaterAccountID,
                CustomAttributes = waterAccountCustomAttribute.CustomAttributes
            };
            return dto;
        }
    }
}