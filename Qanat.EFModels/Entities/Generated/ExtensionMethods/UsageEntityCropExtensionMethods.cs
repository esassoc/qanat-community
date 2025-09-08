//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[UsageEntityCrop]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class UsageEntityCropExtensionMethods
    {
        public static UsageEntityCropSimpleDto AsSimpleDto(this UsageEntityCrop usageEntityCrop)
        {
            var dto = new UsageEntityCropSimpleDto()
            {
                UsageEntityCropID = usageEntityCrop.UsageEntityCropID,
                UsageEntityID = usageEntityCrop.UsageEntityID,
                UsageEntityCropName = usageEntityCrop.UsageEntityCropName
            };
            return dto;
        }
    }
}