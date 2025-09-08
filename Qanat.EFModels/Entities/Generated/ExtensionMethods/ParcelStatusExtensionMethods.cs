//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelStatus]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ParcelStatusExtensionMethods
    {
        public static ParcelStatusSimpleDto AsSimpleDto(this ParcelStatus parcelStatus)
        {
            var dto = new ParcelStatusSimpleDto()
            {
                ParcelStatusID = parcelStatus.ParcelStatusID,
                ParcelStatusName = parcelStatus.ParcelStatusName,
                ParcelStatusDisplayName = parcelStatus.ParcelStatusDisplayName
            };
            return dto;
        }
    }
}