//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelStaging]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ParcelStagingExtensionMethods
    {
        public static ParcelStagingSimpleDto AsSimpleDto(this ParcelStaging parcelStaging)
        {
            var dto = new ParcelStagingSimpleDto()
            {
                ParcelStagingID = parcelStaging.ParcelStagingID,
                GeographyID = parcelStaging.GeographyID,
                ParcelNumber = parcelStaging.ParcelNumber,
                OwnerName = parcelStaging.OwnerName,
                OwnerAddress = parcelStaging.OwnerAddress,
                Acres = parcelStaging.Acres,
                HasConflict = parcelStaging.HasConflict
            };
            return dto;
        }
    }
}