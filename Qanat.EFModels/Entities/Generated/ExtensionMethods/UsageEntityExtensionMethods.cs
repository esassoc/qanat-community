//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[UsageEntity]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class UsageEntityExtensionMethods
    {
        public static UsageEntitySimpleDto AsSimpleDto(this UsageEntity usageEntity)
        {
            var dto = new UsageEntitySimpleDto()
            {
                UsageEntityID = usageEntity.UsageEntityID,
                ParcelID = usageEntity.ParcelID,
                GeographyID = usageEntity.GeographyID,
                UsageEntityName = usageEntity.UsageEntityName,
                UsageEntityArea = usageEntity.UsageEntityArea
            };
            return dto;
        }
    }
}