using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class ZoneExtensionMethods
{
    public static ZoneDetailedDto AsDetailedDto(this Zone zone)
    {
        var zoneDetailedDto = new ZoneDetailedDto()
        {
            ZoneID = zone.ZoneID,
            ZoneGroupID = zone.ZoneGroupID,
            ZoneName = zone.ZoneName,
            ZoneSlug = zone.ZoneSlug,
            ZoneDescription = zone.ZoneDescription,
            ZoneColor = zone.ZoneColor,
            ZoneAccentColor = zone.ZoneAccentColor,
            SortOrder = zone.SortOrder,
            TotalParcels = zone.ParcelZones?.Count ?? 0,
            TotalArea = zone.ParcelZones?.Sum(x => x.Parcel.ParcelArea) ?? 0,
            ZoneGroupName = zone.ZoneGroup?.ZoneGroupName
        };
        return zoneDetailedDto;
    }

    public static ZoneDisplayDto AsDisplayDto(this Zone zone)
    {
        var zoneDisplayDto = new ZoneDisplayDto()
        {
            ZoneID = zone.ZoneID,
            ZoneGroupID = zone.ZoneGroupID,
            ZoneName = zone.ZoneName,
            ZoneColor = zone.ZoneColor,
            ZoneAccentColor = zone.ZoneAccentColor,
            ZoneGroupDisplayToAccountHolders = zone.ZoneGroup.DisplayToAccountHolders,
        };
        return zoneDisplayDto;
    }

    public static ZoneMinimalDto AsZoneMinimalDto(this Zone zone)
    {
        var zoneDetailedDto = new ZoneMinimalDto()
        {
            ZoneID = zone.ZoneID,
            ZoneGroupID = zone.ZoneGroupID,
            ZoneName = zone.ZoneName,
            ZoneSlug = zone.ZoneSlug,
            ZoneDescription = zone.ZoneDescription,
            ZoneColor = zone.ZoneColor,
            ZoneAccentColor = zone.ZoneAccentColor,
            PrecipMultiplier = zone.PrecipMultiplier,
            SortOrder = zone.SortOrder,
            ZoneGroupName = zone.ZoneGroup?.ZoneGroupName,
            ZoneGroupDisplayToAccountHolders = zone.ZoneGroup?.DisplayToAccountHolders ?? false
        };
        return zoneDetailedDto;
    }
}