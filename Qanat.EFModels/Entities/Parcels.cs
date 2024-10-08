using Microsoft.EntityFrameworkCore;
using Qanat.Common.Util;
using Qanat.Models.DataTransferObjects;
using System.Text.RegularExpressions;

namespace Qanat.EFModels.Entities;

public class Parcels
{
    private static IQueryable<Parcel> GetImpl(QanatDbContext dbContext)
    {
        return dbContext.Parcels.AsNoTracking()
            .Include(x => x.Geography)
            .Include(x => x.WaterAccount)
            .Include(x => x.ParcelZones)
                .ThenInclude(x => x.Zone)
                    .ThenInclude(z => z.ZoneGroup)
            .Include(x => x.WellIrrigatedParcels).ThenInclude(x => x.Well)
            .Include(x => x.Wells);
    }

    public static IEnumerable<Parcel> List(QanatDbContext dbContext, int geographyID)
    {
        return GetImpl(dbContext).Where(x => x.GeographyID == geographyID);
    }

    public static List<ParcelDisplayDto> ListAsDisplayDto(QanatDbContext dbContext, int geographyID)
    {
        return List(dbContext, geographyID).Select(x => x.AsDisplayDto()).ToList();
    }

    public static List<Parcel> ListByIDs(QanatDbContext dbContext, List<int> parcelIDs)
    {
        return GetImpl(dbContext).Where(x => parcelIDs.Contains(x.ParcelID)).ToList();
    }

    public static List<ParcelIndexGridDto> ListByGeographyIDAsIndexGridDtos(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.vParcelDetaileds.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .Select(x => x.AsIndexGridDto()).ToList();
    }
    
    public static List<ParcelIndexGridDto> ListByGeographyIDAndUserIDAsIndexGridDtos(QanatDbContext dbContext, int geographyID, int userID)
    {
        var waterAccountIDs = dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccountUsers)
            .Where(x => x.WaterAccountUsers.Any(y => y.UserID == userID))
            .Select(x => x.WaterAccountID).ToList();

        return dbContext.vParcelDetaileds.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterAccountID.HasValue && waterAccountIDs.Contains(x.WaterAccountID.Value))
            .Select(x => x.AsIndexGridDto()).ToList();
    }

    public static ParcelPopupDto GetParcelPopupDtoByID(QanatDbContext dbContext, int parcelID) 
    { 
        var parcel = dbContext.Parcels.AsNoTracking()
            .Include(x => x.Geography)
                .ThenInclude(x => x.GeographyAllocationPlanConfiguration)
                    .ThenInclude(x => x.ZoneGroup)
                        .ThenInclude(x => x.Zones)
            .Include(x => x.WaterAccount)
            .Include(x => x.ParcelZones)
                .ThenInclude(x => x.Zone)
                    .ThenInclude(z => z.ZoneGroup)
            .SingleOrDefault(x => x.ParcelID == parcelID);

        return parcel.AsPopupDto();
    }

    public static List<Parcel> ListByWaterAccountID(QanatDbContext dbContext, int waterAccountID)
    {
        var parcels = dbContext.Parcels
            .Include(x => x.WaterAccount)
            .AsNoTracking()
            .Where(x => x.WaterAccountID == waterAccountID).ToList();
        
        return parcels;
    }

    public static Parcel GetByID(QanatDbContext dbContext, int parcelID)
    {
        return GetImpl(dbContext).SingleOrDefault(x => x.ParcelID == parcelID);
    }

    public static List<ParcelDetailDto> GetParcelDetailDtoListByWaterAccountID(QanatDbContext dbContext, int waterAccountID)
    {
        return GetImpl(dbContext).Where(x => x.WaterAccountID == waterAccountID).Select(x => x.AsDetailDto()).ToList();
    }

    public static ParcelDisplayDto GetByIDAsDisplayDto(QanatDbContext dbContext, int parcelID)
    {
        var parcel = GetByID(dbContext, parcelID);
        return parcel?.AsDisplayDto();
    }

    public static List<ParcelWithGeometryDto> GetByGeographyAndParcelNumberAsFeatureCollection(QanatDbContext dbContext, int geographyID, string parcelNumber)
    {
        return dbContext.Parcels
            .Include(x => x.ParcelGeometry)
            .Include(x => x.Geography)
            .Include(x => x.WaterAccount)
            .AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.ParcelNumber.StartsWith(parcelNumber))
            .Select(x => x.AsParcelWithGeometryDto()).ToList();
    }

    public static bool IsValidParcelNumber(string regexPattern, string parcelNumber)
    {
        return Regex.IsMatch(parcelNumber, regexPattern);
    }

    public static List<Parcel> ListByGeographyIDAndParcelNumbers(QanatDbContext dbContext, int geographyID, List<string> parcelNumbers)
    {
        return GetImpl(dbContext).Where(x => x.GeographyID == geographyID && parcelNumbers.Contains(x.ParcelNumber)).ToList();
    }

    public static List<ParcelDisplayDto> SearchParcelNumberWithinGeography(QanatDbContext dbContext,int geographyID, string searchString)
    {
        return dbContext.Parcels.AsNoTracking()
            .Include(x => x.WaterAccount)
            .Where(x => x.ParcelNumber.StartsWith(searchString) && x.GeographyID == geographyID)
            .Select(x => x.AsDisplayDto())
            .ToList();
    }

    public static List<ParcelWithGeoJSONDto> GetParcelsWithGeoJSONDtos(QanatDbContext dbContext, List<int> parcelIDs)
    {
        var parcelsWithGeoJson = dbContext.Parcels
            .Include(x => x.ParcelGeometry)
            .Include(x => x.WaterAccount)
            .AsNoTracking()
            .Where(x => parcelIDs.Contains(x.ParcelID))
            .Select(x => x.AsParcelWithGeoJSONDto())
            .ToList();

        return parcelsWithGeoJson;
    }

    public static List<ParcelDisplayDto> SearchParcelNumber(QanatDbContext dbContext,  string searchString)
    {
        var parcelDisplayDtos = dbContext.Parcels.AsNoTracking()
            .Where(x => x.ParcelNumber.StartsWith(searchString))
            .Select(x => x.AsDisplayDto())
            .ToList();

        return parcelDisplayDtos;
    }

    /// <summary>
    /// Note, we do not want to do this with lots and lots of parcels
    /// </summary>
    /// <param name="dbContext"></param>
    /// <param name="parcelIDs"></param>
    /// <returns></returns>
    public static BoundingBoxDto GetBoundingBoxByParcelIDs(QanatDbContext dbContext, List<int> parcelIDs)
    {
        var geometries = dbContext.ParcelGeometries.Where(x => parcelIDs.Contains(x.ParcelID)).Select(x => x.Geometry4326).ToList();
        return new BoundingBoxDto(geometries);
    }

    public static List<ParcelMinimalDto> ListParcelsFromAccountIDAndEndDate(QanatDbContext dbContext, int waterAccountID, DateTime endDate)
    {
        int geographyID = dbContext.WaterAccounts.AsNoTracking().Single(x => x.WaterAccountID == waterAccountID).GeographyID;

        // todo: start year?
        var parcels = dbContext.WaterAccountParcels
            .Include(x => x.Parcel)
            .ThenInclude(x => x.Geography)
            .Where(x => x.GeographyID == geographyID && x.WaterAccountID == waterAccountID && x.EffectiveYear <= endDate.Year)
            .Select(x => x.Parcel.AsParcelMinimalDto()).ToList();

        return parcels;
    }

    public static List<ErrorMessage> ValidateUpdate(QanatDbContext dbContext, ParcelUpdateOwnershipRequestDto ownershipRequestDto)
    {
        var results = new List<ErrorMessage>();
        if (string.IsNullOrWhiteSpace(ownershipRequestDto.OwnerName))
        {
            results.Add(new ErrorMessage() { Type = "Owner Name", Message = "The Owner Name field is required."});
        }
        if (string.IsNullOrWhiteSpace(ownershipRequestDto.OwnerAddress))
        {
            results.Add(new ErrorMessage() { Type = "Owner Address", Message = "The Owner Address field is required."});
        }

        return results;
    }

    public static async Task UpdateOwnership(QanatDbContext dbContext, int parcelID,
        ParcelUpdateOwnershipRequestDto requestDto, int userID)
    {
        var parcel = dbContext.Parcels.Single(x => x.ParcelID == parcelID);

        parcel.OwnerName = requestDto.OwnerName;
        parcel.OwnerAddress = requestDto.OwnerAddress;

        var mostRecentParcelHistory = dbContext.ParcelHistories.AsNoTracking()
            .Where(x => x.ParcelID == parcelID).ToList()
            .MaxBy(x => x.UpdateDate);

        var effectiveYear = mostRecentParcelHistory.EffectiveYear;

        var parcelHistory = ParcelHistories.CreateNew(parcel, userID, effectiveYear);
        await dbContext.ParcelHistories.AddAsync(parcelHistory);
        await dbContext.SaveChangesAsync();
    }

    public static void UpdateParcelStatus(QanatDbContext dbContext, ParcelBulkUpdateParcelStatusDto parcelBulkUpdateParcelStatusDto, int userID)
    {
        var parcels = dbContext.Parcels.Where(parcel => parcelBulkUpdateParcelStatusDto.ParcelIDs.Contains(parcel.ParcelID)).ToList();
        var waterAccountParcels = dbContext.WaterAccountParcels.Where(parcel => parcelBulkUpdateParcelStatusDto.ParcelIDs.Contains(parcel.ParcelID)).ToList();
        foreach (var parcel in parcels)
        {
            // relationships where the effective year is greater than or equal to the EndYear selected
            var waterAccountParcelsToRemove = waterAccountParcels.Where(x =>
                x.ParcelID == parcel.ParcelID && x.EffectiveYear >= parcelBulkUpdateParcelStatusDto.EndYear);
            dbContext.WaterAccountParcels.RemoveRange(waterAccountParcelsToRemove);

            var waterAccountParcelsToUpdateEndDate = waterAccountParcels.Where(x =>
                x.ParcelID == parcel.ParcelID && (x.EndYear == null || x.EndYear > parcelBulkUpdateParcelStatusDto.EndYear));
            foreach (var waterAccountParcel in waterAccountParcelsToUpdateEndDate)
            {
                waterAccountParcel.EndYear = parcelBulkUpdateParcelStatusDto.EndYear;
            }

            parcel.WaterAccountID = null;
            parcel.ParcelStatusID = parcelBulkUpdateParcelStatusDto.ParcelStatusID;
            // create a ParcelHistory record when status changes
            var parcelHistory = ParcelHistories.CreateNew(parcel, userID, parcelBulkUpdateParcelStatusDto.EndYear);
            dbContext.ParcelHistories.Add(parcelHistory);
        }

        dbContext.SaveChanges();
    }

    public static List<int> ListWaterAccountParcelEffectiveYearsByParcelIDs(QanatDbContext dbContext, int geographyID, List<int> parcelIDs)
    {
        var latestStartYear = dbContext.WaterAccountParcels
            .Include(x => x.Parcel)
            .Where(x => x.GeographyID == geographyID 
                        && parcelIDs.Contains(x.ParcelID) && x.EndYear == null 
                        && x.Parcel.ParcelStatusID == (int)ParcelStatusEnum.Assigned)
            .OrderByDescending(x => x.EffectiveYear).FirstOrDefault()?.EffectiveYear;
        
        return latestStartYear != null ? Enumerable.Range((int)latestStartYear, DateTime.Today.Year - (int)latestStartYear + 1).ToList() : null;
    }

    public static async Task UpdateParcelZoneAssignments(QanatDbContext dbContext,
        ParcelZoneAssignmentFormDto zoneAssignmentFormDto)
    {
        var existingParcelZones = await dbContext.ParcelZones.Where(x => x.ParcelID == zoneAssignmentFormDto.ParcelID).ToListAsync();
        
        var updatedParcelZones = zoneAssignmentFormDto.ParcelZoneAssignments
            .Where(x => x.ZoneID != null).Select(x => new ParcelZone(){ParcelID = zoneAssignmentFormDto.ParcelID, ZoneID = (int)x.ZoneID}).ToList();

        existingParcelZones.Merge(updatedParcelZones, 
            dbContext.ParcelZones, 
            (x, y) => x.ParcelID == y.ParcelID && x.ZoneID == y.ZoneID);
        await dbContext.SaveChangesAsync();
    }
}