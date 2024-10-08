using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Qanat.Common.Util;
using Qanat.Models.DataTransferObjects;
using System.Globalization;

namespace Qanat.EFModels.Entities;

public class ZoneGroups
{
    private static IQueryable<ZoneGroup> GetImplWithTracking(QanatDbContext dbContext)
    {
        return dbContext.ZoneGroups.Include(x => x.Geography)
                                   .Include(x => x.Zones)
                                        .ThenInclude(x => x.ParcelZones)
                                        .ThenInclude(x => x.Parcel)
                                   .Include(x => x.GeographyAllocationPlanConfigurations)
                                   .AsSplitQuery();
    }

    public static List<ZoneGroupMinimalDto> GetZoneGroupsByGeography(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.ZoneGroups.Include(x => x.Geography)
            .Include(x => x.Zones)
            .Include(x => x.GeographyAllocationPlanConfigurations)
            .AsSplitQuery()
            .AsNoTracking().Where(x => x.GeographyID == geographyID)
            .OrderBy(x => x.SortOrder)
            .Select(x => x.AsZoneGroupMinimalDto()).ToList();
    }

    public static ZoneGroup GetByZoneGroupSlug(QanatDbContext dbContext, string zoneGroupSlug, int geographyID)
    {
        return GetImplWithTracking(dbContext).AsNoTracking()
            .SingleOrDefault(x => x.ZoneGroupSlug == zoneGroupSlug && x.GeographyID == geographyID);
    }

    public static ZoneGroupMinimalDto GetByZoneGroupSlugAsMinimalDto(QanatDbContext dbContext, string zoneGroupSlug, int geographyID)
    {
        return GetByZoneGroupSlug(dbContext, zoneGroupSlug, geographyID)?.AsZoneGroupMinimalDto();
    }

    public static ZoneGroup GetByID(QanatDbContext dbContext, int zoneGroupID)
    {
        return GetImplWithTracking(dbContext).SingleOrDefault(x => x.ZoneGroupID == zoneGroupID);
    }

    public static ZoneGroupMinimalDto CreateZoneGroup(QanatDbContext dbContext, ZoneGroupMinimalDto zoneGroupMinimalDto)
    {
        var zoneGroup = new ZoneGroup();
        zoneGroup.GeographyID = zoneGroupMinimalDto.GeographyID;
        zoneGroup.ZoneGroupName = zoneGroupMinimalDto.ZoneGroupName;
        zoneGroup.ZoneGroupDescription = zoneGroupMinimalDto.ZoneGroupDescription;
        zoneGroup.SortOrder = zoneGroupMinimalDto.SortOrder;
        zoneGroup.ZoneGroupSlug = StringUtilities.SlugifyString(zoneGroupMinimalDto.ZoneGroupName);


        dbContext.ZoneGroups.Add(zoneGroup);
        dbContext.SaveChanges();
        dbContext.Entry(zoneGroup).Reload();
        Zones.UpdateListOfZones(dbContext, zoneGroup.ZoneGroupID, zoneGroupMinimalDto.ZoneList);
        return zoneGroup.AsZoneGroupMinimalDto();
    }

    public static BoundingBoxDto GetBoundingBoxForZoneGroup(QanatDbContext dbContext, List<int> zoneGroupIDs)
    {
        var geographyBoundaries = dbContext.ZoneGroups.Include(x => x.Geography).ThenInclude(x => x.GeographyBoundary).AsNoTracking().Where(x => zoneGroupIDs.Contains(x.ZoneGroupID)).Where(x => x.Geography.GeographyBoundary != null)
            .Select(x => x.Geography.GeographyBoundary).ToList();
        return new BoundingBoxDto(geographyBoundaries.Select(x => x.BoundingBox));
    }

    public static List<ErrorMessage> ValidateZoneGroup(QanatDbContext dbContext, int geographyID, ZoneGroupMinimalDto zoneGroupMinimalDto)
    {
        var results = new List<ErrorMessage>();
        var currentZoneGroup = dbContext.ZoneGroups.AsNoTracking()
            .SingleOrDefault(x => x.GeographyID == zoneGroupMinimalDto.GeographyID && x.ZoneGroupName == zoneGroupMinimalDto.ZoneGroupName);

        if (string.IsNullOrEmpty(zoneGroupMinimalDto.ZoneGroupName))
        {
            results.Add(new ErrorMessage()
            {
                Type = "Zone Group Name",
                Message = "Zone Group Name is a required field. Please enter a name for the Zone Group."
            });
        }
        else if (currentZoneGroup != null && currentZoneGroup.ZoneGroupID != zoneGroupMinimalDto.ZoneGroupID)
        {
            results.Add(new ErrorMessage()
            {
                Type = "Zone Group Name",
                Message = $"The Zone Group Name \"{currentZoneGroup.ZoneGroupName}\" is already taken. Please choose a unique Zone Group Name."
            });
        }

        var geography = dbContext.Geographies
            .Include(x => x.GeographyConfiguration)
            .Include(x => x.GeographyAllocationPlanConfiguration).AsNoTracking()
            .Single(x => x.GeographyID == geographyID);

        if (geography.GeographyAllocationPlanConfiguration != null
            && zoneGroupMinimalDto.ZoneGroupID == geography.GeographyAllocationPlanConfiguration.ZoneGroupID
            && geography.GeographyConfiguration.ZonePrecipMultipliersEnabled
            && zoneGroupMinimalDto.ZoneList.Any(x => !x.PrecipMultiplier.HasValue))
        {
            results.Add(new ErrorMessage()
            {
                Type = "Precip Multiplier",
                Message = $"The Precip Multipler field is required for Zones within this geography's configured Allocation Zone Group."
            });
        }

        return results;
    }

    public static void CreateOrUpdateZoneGroup(QanatDbContext dbContext, ZoneGroupMinimalDto zoneGroupMinimalDto)
    {
        var zoneGroup = GetImplWithTracking(dbContext).SingleOrDefault(x => x.ZoneGroupID == zoneGroupMinimalDto.ZoneGroupID);
        if (zoneGroup == null)
        {
            CreateZoneGroup(dbContext, zoneGroupMinimalDto);
            return;
        }
        zoneGroup.ZoneGroupName = zoneGroupMinimalDto.ZoneGroupName;
        zoneGroup.ZoneGroupSlug = StringUtilities.SlugifyString(zoneGroupMinimalDto.ZoneGroupName);
        zoneGroup.ZoneGroupDescription = zoneGroupMinimalDto.ZoneGroupDescription;
        Zones.UpdateListOfZones(dbContext, zoneGroupMinimalDto.ZoneGroupID, zoneGroupMinimalDto.ZoneList);
        dbContext.SaveChanges();
    }

    public static void DeleteZoneGroup(QanatDbContext dbContext, int zoneGroupID)
    {
        var zoneGroup = GetImplWithTracking(dbContext).SingleOrDefault(x => x.ZoneGroupID == zoneGroupID);
        if (zoneGroup != null)
        {
            var zones = dbContext.Zones.Where(x => x.ZoneGroupID == zoneGroupID).ToList();
            dbContext.Zones.RemoveRange(zones);
            dbContext.ZoneGroups.Remove(zoneGroup);
            ClearZoneGroupData(dbContext, zoneGroupID);
            dbContext.SaveChanges();
        }
    }

    public static void ClearZoneGroupData(QanatDbContext dbContext, int zoneGroupID)
    {
        var zones = dbContext.Zones.Where(x => x.ZoneGroupID == zoneGroupID).Select(x => x.ZoneID).ToList();
        var parcelZones = dbContext.ParcelZones;
        var parcelZonesToRemove = parcelZones.Where(parcelZone => zones.Contains(parcelZone.ZoneID)).ToList();
        dbContext.ParcelZones.RemoveRange(parcelZonesToRemove);
        dbContext.SaveChanges();
    }

    public static void UpdateZoneGroupSortOrder(QanatDbContext dbContext, List<ZoneGroupMinimalDto> zoneGroupMinimalDtos)
    {
        var existingZoneGroups = dbContext.ZoneGroups.Where(x => x.GeographyID == zoneGroupMinimalDtos[0].GeographyID).ToList();
        var updatedZoneGroups = zoneGroupMinimalDtos.Select(x => new ZoneGroup()
        {
            ZoneGroupID = x.ZoneGroupID,
            GeographyID = x.GeographyID,
            ZoneGroupDescription = x.ZoneGroupDescription,
            SortOrder = x.SortOrder,
            ZoneGroupName = x.ZoneGroupName

        }).ToList();

        var allInDatabase = dbContext.ZoneGroups;

        existingZoneGroups.Merge(updatedZoneGroups, allInDatabase,
            (x, y) => x.ZoneGroupID == y.ZoneGroupID,
            (x, y) =>
            {
                x.SortOrder = y.SortOrder;
            });
        dbContext.SaveChanges();
    }

    public static Dictionary<string, int> CreateFromCSV(QanatDbContext dbContext, List<ZoneGroupCSV> records,
        int geographyID, int zoneGroupID)
    {
        var unassigned = "unassigned";
        var parcelZones = new List<ParcelZone>();

        var parcelsDictionary = dbContext.Parcels.AsNoTracking().Where(x => x.GeographyID == geographyID)
            .ToDictionary(x => x.ParcelNumber, y => y.ParcelID);
        var zonesDictionary = dbContext.Zones.AsNoTracking().Where(x => x.ZoneGroupID == zoneGroupID)
            .ToDictionary(x => x.ZoneName, y => y.ZoneID);

        var zoneGroupResponse = zonesDictionary.ToDictionary(zone => zone.Key, zone => 0);
        zoneGroupResponse.Add(unassigned, 0);

        foreach (var record in records)
        {

            if (!string.IsNullOrEmpty(record.Zone))
            {
                var parcelZone = new ParcelZone()
                {
                    ParcelID = parcelsDictionary[record.APN],
                    ZoneID = zonesDictionary[record.Zone]
                };

                parcelZones.Add(parcelZone);
                zoneGroupResponse[record.Zone]++;
                continue;
            }

            zoneGroupResponse[unassigned]++;
        }

        var allParcelZones = dbContext.ParcelZones;
        var zones = dbContext.Zones.Where(x => x.ZoneGroupID == zoneGroupID).Select(x => x.ZoneID).ToList();
        var existingParcelZones =
            allParcelZones.ToList().Where(parcelZone => zones.Contains(parcelZone.ZoneID)).ToList();

        existingParcelZones.Merge(parcelZones, allParcelZones,
            (x, y) => x.ZoneID == y.ZoneID && x.ParcelID == y.ParcelID,
            (x, y) =>
            {
                x.ZoneID = y.ZoneID;
                x.ParcelID = y.ParcelID;
            }
        );
        dbContext.SaveChanges();

        return zoneGroupResponse;
    }

    public static List<ErrorMessage> ValidateCsv(QanatDbContext dbContext, List<ZoneGroupCSV> records, int geographyID,
        int zoneGroupID)
    {
        var zoneGroupName = dbContext.ZoneGroups.Single(x => x.ZoneGroupID == zoneGroupID).ZoneGroupName;
        var results = new List<ErrorMessage>();
        var parcelNumbers = new List<string>();
        var parcelsInGeography = dbContext.Parcels.Where(x => x.GeographyID == geographyID).Select(x => x.ParcelID).ToList();
        var parcelsDictionary = dbContext.Parcels.AsNoTracking().Where(x => x.GeographyID == geographyID).ToDictionary(x => x.ParcelNumber, y => y.ParcelID);
        var zones = dbContext.Zones.Where(x => x.ZoneGroupID == zoneGroupID).Select(x => x.ZoneName).ToList();

        foreach (var record in records)
        {
            if (!string.IsNullOrEmpty(record.Zone) && !zones.Contains(record.Zone))
            {
                results.Add(new ErrorMessage()
                {
                    Type = "Zone",
                    Message = $"{record.Zone} is not a Zone in the current {zoneGroupName}. Please enter a valid zone."
                });
            }
            else if (!parcelsInGeography.Contains(parcelsDictionary[record.APN]))
            {
                results.Add( new ErrorMessage()
                {
                    Type = "Parcel",
                    Message = $"{record.APN} is not in the current geography. Please enter a valid APN."
                });
            }
            else if (parcelNumbers.Contains(record.APN))
            {
                results.Add(new ErrorMessage()
                {
                    Type = "Parcel",
                    Message = $"{record.APN} appears in the csv more than once. Please correct."
                });
            }
            parcelNumbers.Add(record.APN);
        }

        return results;
    }

    public static byte[] ListZoneGroupDataAsCsvByteArray(QanatDbContext dbContext, int geographyID, int zoneGroupID)
    {
        var parcelZones = GetAllZoneGroupData(dbContext, zoneGroupID);
        return ListParcelZones(dbContext, parcelZones, geographyID, zoneGroupID);
    }

    public static List<ParcelZone> GetAllZoneGroupData(QanatDbContext dbContext, int zoneGroupID)
    {
        var zoneIDs = dbContext.Zones.Where(x => x.ZoneGroupID == zoneGroupID).Select(x => x.ZoneID);
        //var allParcelZones = dbContext.ParcelZones.Where(parcelZone => zoneIDs.Contains(parcelZone.ZoneID)).ToList();
        var parcelZones = dbContext.ParcelZones.Where(parcelZone => zoneIDs.Contains(parcelZone.ZoneID)).ToList();
        return parcelZones;
    }

    private static byte[] ListParcelZones(QanatDbContext dbContext, List<ParcelZone> parcelZones, int geographyID, int zoneGroupID)
    {

        var parcelsDictionary = dbContext.Parcels.AsNoTracking().Where(x => x.GeographyID == geographyID)
            .ToDictionary(x => x.ParcelID, y => y.ParcelNumber);
        var zonesDictionary = dbContext.Zones.AsNoTracking().Where(x => x.ZoneGroupID == zoneGroupID)
            .ToDictionary(x => x.ZoneID, y => y.ZoneName);

        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream);
        using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
        csvWriter.WriteField("APN");
        csvWriter.WriteField("Zones");

        csvWriter.NextRecord();

        foreach (var parcelZone in parcelZones)
        {
            csvWriter.WriteField(parcelsDictionary[parcelZone.ParcelID]);

            csvWriter.WriteField(zonesDictionary[parcelZone.ZoneID]);

            csvWriter.NextRecord();
        }

        streamWriter.Flush();
        return memoryStream.ToArray();
    }

}