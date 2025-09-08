//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Zone]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class ZoneExtensionMethods
    {
        public static ZoneSimpleDto AsSimpleDto(this Zone zone)
        {
            var dto = new ZoneSimpleDto()
            {
                ZoneID = zone.ZoneID,
                ZoneGroupID = zone.ZoneGroupID,
                ZoneName = zone.ZoneName,
                ZoneSlug = zone.ZoneSlug,
                ZoneDescription = zone.ZoneDescription,
                ZoneColor = zone.ZoneColor,
                ZoneAccentColor = zone.ZoneAccentColor,
                PrecipMultiplier = zone.PrecipMultiplier,
                SortOrder = zone.SortOrder
            };
            return dto;
        }
    }
}