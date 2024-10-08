//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Parcel]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ParcelExtensionMethods
    {
        public static ParcelSimpleDto AsSimpleDto(this Parcel parcel)
        {
            var dto = new ParcelSimpleDto()
            {
                ParcelID = parcel.ParcelID,
                GeographyID = parcel.GeographyID,
                WaterAccountID = parcel.WaterAccountID,
                ParcelNumber = parcel.ParcelNumber,
                ParcelArea = parcel.ParcelArea,
                ParcelStatusID = parcel.ParcelStatusID,
                OwnerAddress = parcel.OwnerAddress,
                OwnerName = parcel.OwnerName
            };
            return dto;
        }
    }
}