using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class UsageLocationCropExtensionMethods
    {
        public static UsageLocationCropSimpleDto AsSimpleDto(this UsageLocationCrop usageLocationCrop)
        {
            var dto = new UsageLocationCropSimpleDto()
            {
                UsageLocationCropID = usageLocationCrop.UsageLocationCropID,
                UsageLocationID = usageLocationCrop.UsageLocationID,
                Name = usageLocationCrop.Name
            };
            return dto;
        }
    }
}