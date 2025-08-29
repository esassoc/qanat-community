using Microsoft.EntityFrameworkCore;
using Qanat.Common.Util;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public class Zones
{
    public static List<ErrorMessage> ValidateZones(QanatDbContext dbContext, int zoneGroupID, List<ZoneMinimalDto> zoneMinimalDtos)
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

        var existingZones = dbContext.Zones.Where(x => x.ZoneGroupID == zoneGroupID).ToList();
        var removedZones = existingZones.Where(zone => zoneMinimalDtos.SingleOrDefault(x => x.ZoneID == zone.ZoneID) == null);

        foreach (var removedZone in removedZones)
        {
            var allocationPlan = dbContext.AllocationPlans.FirstOrDefault(x => x.ZoneID == removedZone.ZoneID);
            if (allocationPlan != null)
            {
                results.Add(new ErrorMessage()
                {
                    Type = removedZone.ZoneName,
                    Message = $"Could not delete the zone {removedZone.ZoneName} because it is currently associated with an Allocation Plan."
                });
            }
        }

        return results;
    }

    //MK 5/1/2025: When swapping zone names, EF/SQL can't figure out how to update the rows without breaking the unique index on ZoneName/ZoneSlug. So it throws a weird error about circular references. Updating in two passes gets around this issue. 
    public static void UpdateListOfZones(QanatDbContext dbContext, int zoneGroupID, List<ZoneMinimalDto> zoneMinimalDtos)
    {
        using var transaction = dbContext.Database.BeginTransaction();

        var renameBack = new Dictionary<int, string>();

        var existingZones = dbContext.Zones.Where(x => x.ZoneGroupID == zoneGroupID).ToList();
        foreach (var zoneDto in zoneMinimalDtos)
        {
            if (zoneDto.ZoneID > 0)
            {
                var existingZone = existingZones.SingleOrDefault(x => x.ZoneID == zoneDto.ZoneID);
                if (existingZone == null)
                {
                    throw new NullReferenceException($"Trying to update Zone with ID {zoneDto.ZoneID} but it does not exist!");
                }

                if (zoneDto.ZoneName != existingZone.ZoneName)
                {
                    var temp = $"__tmp_{Guid.NewGuid()}";
                    existingZone.ZoneName = temp;
                    existingZone.ZoneSlug = temp;
                    renameBack[zoneDto.ZoneID] = zoneDto.ZoneName;
                }

                existingZone.ZoneDescription = zoneDto.ZoneDescription;
                existingZone.ZoneColor = zoneDto.ZoneColor;
                existingZone.ZoneAccentColor = zoneDto.ZoneAccentColor;
                existingZone.PrecipMultiplier = zoneDto.PrecipMultiplier;
                existingZone.SortOrder = zoneDto.SortOrder;
            }
            else
            {
                var newZone = new Zone()
                {
                    ZoneGroupID = zoneGroupID,
                    ZoneName = zoneDto.ZoneName,
                    ZoneSlug = StringUtilities.SlugifyString(zoneDto.ZoneName),
                    ZoneDescription = zoneDto.ZoneDescription,
                    ZoneColor = zoneDto.ZoneColor,
                    ZoneAccentColor = zoneDto.ZoneAccentColor,
                    PrecipMultiplier = zoneDto.PrecipMultiplier,
                    SortOrder = zoneDto.SortOrder
                };
                dbContext.Zones.Add(newZone);
            }
        }

        foreach (var zone in existingZones.Where(zone => zoneMinimalDtos.SingleOrDefault(x => x.ZoneID == zone.ZoneID) == null))
        {
            var parcelZones = dbContext.ParcelZones.Where(x => x.ZoneID == zone.ZoneID);
            dbContext.ParcelZones.RemoveRange(parcelZones);
            dbContext.Zones.Remove(zone);
        }

        dbContext.SaveChanges();

        foreach (var kvp in renameBack)
        {
            var zone = dbContext.Zones.FirstOrDefault(x => x.ZoneID == kvp.Key);
            if (zone != null)
            {
                zone.ZoneName = kvp.Value;
                zone.ZoneSlug = StringUtilities.SlugifyString(kvp.Value);
            }
        }

        dbContext.SaveChanges();
        transaction.Commit();
    }

    public static List<ZoneDisplayDto> ListAsDisplayDto(QanatDbContext dbContext)
    {
        var zoneDisplayDtos = dbContext.Zones.AsNoTracking()
            .Include(x => x.ZoneGroup)
            .Select(x => x.AsDisplayDto()).ToList();

        return zoneDisplayDtos;
    }

    public static List<ZoneDetailedDto> ListByGeographyIDAsZoneDetailedDto(QanatDbContext dbContext, int geographyID, bool callingUserIsAdminOrWaterManager)
    {
        var zones = dbContext.Zones
            .AsNoTracking()
            .Include(x => x.ZoneGroup)
            .Include(x => x.ParcelZones)
            .ThenInclude(x => x.Parcel).AsSplitQuery()
            .Where(x => x.ZoneGroup.GeographyID == geographyID);

        if (!callingUserIsAdminOrWaterManager)
        {
            zones = zones.Where(x => x.ZoneGroup.DisplayToAccountHolders);
        }

        var zoneDtos = zones.Select(x => x.AsDetailedDto()).ToList();
        return zoneDtos;
    }
}