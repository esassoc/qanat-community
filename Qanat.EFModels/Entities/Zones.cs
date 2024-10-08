using Microsoft.EntityFrameworkCore;
using Qanat.Common.Util;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public class Zones
{
    public static List<ErrorMessage> ValidateZones(List<ZoneMinimalDto> zoneMinimalDtos)
    {
        var zoneNames = zoneMinimalDtos.Select(x => x.ZoneName).Distinct().ToList();
        var results = (from zoneName in zoneNames where string.IsNullOrEmpty(zoneName) select new ErrorMessage() { Type = "Zone Name", Message = "Zone Name is a required field. Please enter a valid zone name." }).ToList();
        if (zoneNames.Count != zoneMinimalDtos.Count)
        {
            results.Add(new ErrorMessage()
            {
                Type = "Zone Name",
                Message = "More than one zone with the same name. Please rename."
            });
        }

        return results;
    }

    public static void UpdateListOfZones(QanatDbContext dbContext, int zoneGroupID, List<ZoneMinimalDto> zoneMinimalDtos)
    {
        var existingZones = dbContext.Zones.Where(x => x.ZoneGroupID == zoneGroupID).ToList();
        foreach (var zone in zoneMinimalDtos)
        {
            if (zone.ZoneID > 0)
            {
                var existingZone = existingZones.SingleOrDefault(x => x.ZoneID == zone.ZoneID);
                if (existingZone == null)
                {
                    throw new NullReferenceException($"Trying to update Zone with ID {zone.ZoneID} but it does not exist!");
                }
                existingZone.ZoneName = zone.ZoneName;
                existingZone.ZoneSlug = StringUtilities.SlugifyString(zone.ZoneName);
                existingZone.ZoneDescription = zone.ZoneDescription;
                existingZone.ZoneColor = zone.ZoneColor;
                existingZone.ZoneAccentColor = zone.ZoneAccentColor;
                existingZone.PrecipMultiplier = zone.PrecipMultiplier;
                existingZone.SortOrder = zone.SortOrder;
            }
            else
            {
                var newZone = new Zone()
                {
                    ZoneGroupID = zoneGroupID,
                    ZoneName = zone.ZoneName,
                    ZoneSlug = StringUtilities.SlugifyString(zone.ZoneName),
                    ZoneDescription = zone.ZoneDescription,
                    ZoneColor = zone.ZoneColor,
                    ZoneAccentColor = zone.ZoneAccentColor,
                    PrecipMultiplier = zone.PrecipMultiplier,
                    SortOrder = zone.SortOrder
            };
                dbContext.Zones.Add(newZone);
            }
        }

        foreach (var zone in existingZones.Where(zone => zoneMinimalDtos.SingleOrDefault(x => x.ZoneID == zone.ZoneID) == null))
        {
            dbContext.Zones.Remove(zone);
        }

        dbContext.SaveChanges();

    }

    public static List<ZoneDisplayDto> ListAsDisplayDto(QanatDbContext dbContext)
    {
        return dbContext.Zones.AsNoTracking().Select(x => x.AsDisplayDto()).ToList();
    }

    public static List<ZoneDetailedDto> ListByGeographyIDAsZoneDetailedDto(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.Zones.Include(x => x.ZoneGroup)
            .Include(x => x.ParcelZones)
            .ThenInclude(x => x.Parcel).AsSplitQuery()
            .AsNoTracking().Where(x => x.ZoneGroup.GeographyID == geographyID).Select(x => x.AsDetailedDto()).ToList();
    }
}