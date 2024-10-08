//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelZone]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ParcelZoneExtensionMethods
    {
        public static ParcelZoneSimpleDto AsSimpleDto(this ParcelZone parcelZone)
        {
            var dto = new ParcelZoneSimpleDto()
            {
                ParcelZoneID = parcelZone.ParcelZoneID,
                ZoneID = parcelZone.ZoneID,
                ParcelID = parcelZone.ParcelID
            };
            return dto;
        }
    }
}