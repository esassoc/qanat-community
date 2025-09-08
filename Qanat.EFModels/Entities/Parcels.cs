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
            .Include(x => x.Wells)
            .Include(x => x.WaterAccountParcels).ThenInclude(x => x.WaterAccount);
    }

    public static IEnumerable<Parcel> List(QanatDbContext dbContext, int geographyID)
    {
        return GetImpl(dbContext).Where(x => x.GeographyID == geographyID);
    }

    public static List<ParcelDisplayDto> ListAsDisplayDto(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.Parcels.AsNoTracking()
            .Include(x => x.Geography)
            .Include(x => x.WaterAccount)
            .Where(x => x.GeographyID == geographyID).Select(x => x.AsDisplayDto()).ToList();
    }

    public static List<Parcel> ListByIDs(QanatDbContext dbContext, List<int> parcelIDs)
    {
        return GetImpl(dbContext).Where(x => parcelIDs.Contains(x.ParcelID)).ToList();
    }

    public static List<ParcelIndexGridDto> ListByWaterAccountIDAsIndexGridDtos(QanatDbContext dbContext, int waterAccountID)
    {
        var parcels = dbContext.vParcelDetaileds.AsNoTracking()
            .Where(x => x.CurrentWaterAccountID == waterAccountID).AsEnumerable()
            .OrderByDescending(x => x.ReportingPeriodEndDate) //MK 2/28/2025: This OrderByDescending should ensure that the DistinctBy will pick up the most recent reporting period for each parcel.
            .Select(x => x.AsIndexGridDto())
            .DistinctBy(x => x.ParcelID).ToList();

        return parcels;
    }

    public static List<ParcelIndexGridDto> ListByGeographyIDAsIndexGridDtos(QanatDbContext dbContext, int geographyID)
    {
        var parcels = dbContext.vParcelDetaileds.AsNoTracking()
            .Where(x => x.GeographyID == geographyID).AsEnumerable()
            .OrderByDescending(x => x.ReportingPeriodEndDate) //MK 2/28/2025: This OrderByDescending should ensure that the DistinctBy will pick up the most recent reporting period for each parcel.
            .Select(x => x.AsIndexGridDto())
            .DistinctBy(x => x.ParcelID).ToList();

        return parcels;
    }

    public static List<ParcelIndexGridDto> ListByGeographyIDAndUserIDAsIndexGridDtos(QanatDbContext dbContext, int geographyID, int userID)
    {
        var waterAccountIDs = dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccountUsers)
            .Where(x => x.WaterAccountUsers.Any(y => y.UserID == userID))
            .Select(x => x.WaterAccountID).ToList();

        var parcels = dbContext.vParcelDetaileds.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterAccountID.HasValue && waterAccountIDs.Contains(x.WaterAccountID.Value))
            .AsEnumerable()
            .OrderByDescending(x => x.ReportingPeriodEndDate) //MK 2/28/2025: This OrderByDescending should ensure that the DistinctBy will pick up the most recent reporting period for each parcel.
            .Select(x => x.AsIndexGridDto())
            .DistinctBy(x => x.ParcelID).ToList();

        return parcels;
    }

    public static List<ParcelIndexGridDto> ListByGeographyIDAndReportingPeriodAsIndexGridDtos(QanatDbContext dbContext, int geographyID, int reportingPeriodID)
    {
        var parcels = dbContext.vParcelDetaileds.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID)
            .Select(x => x.AsIndexGridDto()).ToList();

        return parcels;
    }

    public static List<ParcelIndexGridDto> ListByGeographyIDAndReportingPeriodIDAndUserIDAsIndexGridDtos(QanatDbContext dbContext, int geographyID, int reportingPeriodID, int userID)
    {
        var waterAccountIDs = dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccountUsers)
            .Where(x => x.WaterAccountUsers.Any(y => y.UserID == userID))
            .Select(x => x.WaterAccountID).ToList();

        var parcels = dbContext.vParcelDetaileds.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID && x.WaterAccountID.HasValue && waterAccountIDs.Contains(x.WaterAccountID.Value))
            .Select(x => x.AsIndexGridDto()).ToList();

        return parcels;
    }

    public static List<ParcelLinkDisplayDto> ListByGeographyIDAndReportingPeriodAsLinkDisplayDtos(QanatDbContext dbContext, int geographyID, int reportingPeriodID)
    {
        var parcels = dbContext.vParcelDetaileds.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID)
            .Select(x => new ParcelLinkDisplayDto()
            {
                ParcelID = x.ParcelID,
                LinkDisplay = x.ParcelNumber
            }).ToList();

        return parcels;
    }

    public static List<ParcelLinkDisplayDto> ListByGeographyIDAndReportingPeriodIDAndUserIDAsLinkDisplayDtos(QanatDbContext dbContext, int geographyID, int reportingPeriodID, int userID)
    {
        var waterAccountIDs = dbContext.WaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccountUsers)
            .Where(x => x.WaterAccountUsers.Any(y => y.UserID == userID))
            .Select(x => x.WaterAccountID)
            .ToList();

        var parcels = dbContext.vParcelDetaileds.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriodID && x.WaterAccountID.HasValue && waterAccountIDs.Contains(x.WaterAccountID.Value))
            .Select(x => new ParcelLinkDisplayDto()
            {
                ParcelID = x.ParcelID,
                LinkDisplay = x.ParcelNumber
            })
            .ToList();

        return parcels;
    }

    public static ParcelPopupDto GetParcelPopupDtoByID(QanatDbContext dbContext, int parcelID, int? reportingPeriodID = null)
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
            .Include(x => x.WaterAccountParcels).ThenInclude(x => x.WaterAccount)
            .SingleOrDefault(x => x.ParcelID == parcelID);

        return parcel.AsPopupDto(reportingPeriodID);
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

    public static ParcelDisplayDto GetByIDAsDisplayDto(QanatDbContext dbContext, int parcelID)
    {
        var parcel = dbContext.Parcels.AsNoTracking().SingleOrDefault(x => x.ParcelID == parcelID);
        return parcel?.AsDisplayDto();
    }

    public static List<ParcelWithGeometryDto> GetByGeographyAndParcelNumberAsParcelWithGeometryDto(QanatDbContext dbContext, int geographyID, string parcelNumber)
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

    public static List<ParcelDisplayDto> SearchParcelNumberWithinGeography(QanatDbContext dbContext, int geographyID, string searchString)
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

    public static List<ParcelDisplayDto> SearchParcelNumber(QanatDbContext dbContext, string searchString)
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
        var geographyID = dbContext.WaterAccounts.AsNoTracking().Single(x => x.WaterAccountID == waterAccountID).GeographyID;
        var parcels = dbContext.WaterAccountParcels
            .Include(x => x.Parcel).ThenInclude(x => x.Geography)
            .Where(x => x.GeographyID == geographyID && x.WaterAccountID == waterAccountID && x.ReportingPeriod.EndDate.Year <= endDate.Year)
            .Select(x => x.Parcel.AsParcelMinimalDto()).ToList();

        return parcels;
    }

    public static List<ErrorMessage> ValidateUpdate(QanatDbContext dbContext, ParcelUpdateOwnershipRequestDto ownershipRequestDto)
    {
        var results = new List<ErrorMessage>();
        if (string.IsNullOrWhiteSpace(ownershipRequestDto.OwnerName))
        {
            results.Add(new ErrorMessage() { Type = "Owner Name", Message = "The Owner Name field is required." });
        }

        if (string.IsNullOrWhiteSpace(ownershipRequestDto.OwnerAddress))
        {
            results.Add(new ErrorMessage() { Type = "Owner Address", Message = "The Owner Address field is required." });
        }

        return results;
    }

    public static async Task UpdateOwnership(QanatDbContext dbContext, int parcelID, ParcelUpdateOwnershipRequestDto requestDto, int userID)
    {
        var parcel = dbContext.Parcels.Single(x => x.ParcelID == parcelID);

        parcel.OwnerName = requestDto.OwnerName;
        parcel.OwnerAddress = requestDto.OwnerAddress;

        var parcelHistory = ParcelHistories.CreateNew(parcel, userID);
        await dbContext.ParcelHistories.AddAsync(parcelHistory);
        await dbContext.SaveChangesAsync();
        // we need to mark any existing parcel history records for the provided parcels as IsReviewed = true
        await ParcelHistories.MarkAsReviewedByParcelIDsAsync(dbContext, [parcelID]);
    }

    public static async Task UpdateParcelStatus(QanatDbContext dbContext, int geographyID, ParcelBulkUpdateParcelStatusDto parcelBulkUpdateParcelStatusDto, int userID)
    {
        var parcels = await dbContext.Parcels
            .Include(x => x.WaterAccount)
            .Where(parcel => parcelBulkUpdateParcelStatusDto.ParcelIDs.Contains(parcel.ParcelID))
            .ToListAsync();

        var currentReportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(dbContext, geographyID, DateTime.UtcNow.Year);
        var reportingPeriodIDsForCurrentAndFuture = dbContext.ReportingPeriods.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.EndDate >= currentReportingPeriod.EndDate)
            .Select(x => x.ReportingPeriodID).ToList();

        foreach (var parcel in parcels)
        {
            parcel.ParcelStatusID = parcelBulkUpdateParcelStatusDto.ParcelStatusID;

            if (parcelBulkUpdateParcelStatusDto.ParcelStatusID == ParcelStatus.Unassigned.ParcelStatusID || parcelBulkUpdateParcelStatusDto.ParcelStatusID == ParcelStatus.Excluded.ParcelStatusID || parcelBulkUpdateParcelStatusDto.ParcelStatusID == ParcelStatus.Inactive.ParcelStatusID)
            {
                var fromWaterAccountID = parcel.WaterAccountID;
                var fromWaterAccountNumber = parcel.WaterAccount?.WaterAccountNumber;
                var fromWaterAccountName = parcel.WaterAccount?.WaterAccountName;

                parcel.WaterAccountID = null;

                //Remove water account parcel for current reporting period and any reporting period after the current 
                await dbContext.WaterAccountParcels
                    .Include(x => x.ReportingPeriod)
                    .Where(x => x.ParcelID == parcel.ParcelID && x.ReportingPeriod.EndDate >= currentReportingPeriod.EndDate)
                    .ExecuteDeleteAsync();

                var parcelStatus = ParcelStatus.All.Single(x => x.ParcelStatusID == parcelBulkUpdateParcelStatusDto.ParcelStatusID);

                var parcelWaterAccountHistoriesToAdd = new List<ParcelWaterAccountHistory>();
                foreach (var reportingPeriodID in reportingPeriodIDsForCurrentAndFuture)
                {
                    var parcelWaterAccountHistory = new ParcelWaterAccountHistory()
                    {
                        GeographyID = geographyID,
                        ReportingPeriodID = reportingPeriodID,
                        ParcelID = parcel.ParcelID,
                        FromWaterAccountID = fromWaterAccountID,
                        FromWaterAccountName = fromWaterAccountName,
                        FromWaterAccountNumber = fromWaterAccountNumber,
                        Reason = $"Parcel status changed to {parcelStatus.ParcelStatusDisplayName}.",
                        CreateUserID = userID,
                        CreateDate = DateTime.UtcNow
                    };

                    parcelWaterAccountHistoriesToAdd.Add(parcelWaterAccountHistory);
                }

                await dbContext.ParcelWaterAccountHistories.AddRangeAsync(parcelWaterAccountHistoriesToAdd);
            }

            // create a ParcelHistory record when status changes
            var parcelHistory = ParcelHistories.CreateNew(parcel, userID);
            await dbContext.ParcelHistories.AddAsync(parcelHistory);
        }

        await dbContext.SaveChangesAsync();

        // we need to mark any existing parcel history records for the provided parcels as IsReviewed = true
        await ParcelHistories.MarkAsReviewedByParcelIDsAsync(dbContext, parcelBulkUpdateParcelStatusDto.ParcelIDs);
    }

    public static List<int> ListWaterAccountParcelEffectiveYearsByParcelIDs(QanatDbContext dbContext, int geographyID, List<int> parcelIDs)
    {
        var endYears = dbContext.WaterAccountParcels.AsNoTracking()
            .Include(x => x.ReportingPeriod)
            .Where(x => parcelIDs.Contains(x.ParcelID) && x.ReportingPeriod.GeographyID == geographyID)
            .Select(x => x.ReportingPeriod.EndDate.Year).Distinct().ToList();

        return endYears;
    }

    public static async Task UpdateParcelZoneAssignments(QanatDbContext dbContext, ParcelZoneAssignmentFormDto zoneAssignmentFormDto)
    {
        var existingParcelZones = await dbContext.ParcelZones.Where(x => x.ParcelID == zoneAssignmentFormDto.ParcelID).ToListAsync();

        var updatedParcelZones = zoneAssignmentFormDto.ParcelZoneAssignments
            .Where(x => x.ZoneID != null)
            .Select(x => new ParcelZone() { ParcelID = zoneAssignmentFormDto.ParcelID, ZoneID = (int)x.ZoneID })
            .ToList();

        existingParcelZones.Merge(updatedParcelZones, dbContext.ParcelZones, (x, y) => x.ParcelID == y.ParcelID && x.ZoneID == y.ZoneID);
        await dbContext.SaveChangesAsync();
    }

    public static ParcelSearchSummaryDto GetBySearchString(QanatDbContext dbContext, int? geographyID, string searchString, UserDto user)
    {
        const int searchResultLimit = 10;

        IQueryable<Parcel> parcels;

        if (user.RoleID == (int)RoleEnum.SystemAdmin)
        {
            parcels = dbContext.Parcels.AsNoTracking().Include(x => x.WaterAccount);
        }
        else
        {
            var managedGeographyIDs = dbContext.GeographyUsers.AsNoTracking()
                .Where(x => x.UserID == user.UserID && x.GeographyRoleID == GeographyRole.WaterManager.GeographyRoleID)
                .Select(x => x.GeographyID).ToList();

            parcels = dbContext.Parcels.AsNoTracking()
                .Include(x => x.WaterAccount)
                .Include(x => x.WaterAccountParcels)
                .ThenInclude(x => x.WaterAccount)
                .ThenInclude(x => x.WaterAccountUsers)
                .Where(x => managedGeographyIDs.Contains(x.GeographyID) || x.WaterAccount.WaterAccountUsers.Any(y => y.UserID == user.UserID));
        }

        var parcelsToFilter = geographyID.HasValue ? parcels.Where(x => x.GeographyID == geographyID.Value).ToList() : parcels.ToList();
        var matchedParcels =
            parcelsToFilter
            .Where(x =>
                (x.WaterAccount != null && (!string.IsNullOrEmpty(x.WaterAccount.WaterAccountName) && x.WaterAccount.WaterAccountName.ToUpper().Contains(searchString.ToUpper()))) ||
                (x.WaterAccount != null && x.WaterAccount.WaterAccountNumber.ToString().Contains(searchString)) ||
                x.OwnerName.Contains(searchString.ToUpper()) ||
                x.OwnerAddress.Contains(searchString.ToUpper()) ||
                x.ParcelNumber.Contains(searchString))
            .ToList()
            .Select(x => AsParcelWithMatchedFieldsDto(searchString, x))
            .ToList();

        return new ParcelSearchSummaryDto()
        {
            TotalResults = matchedParcels.Count,
            ParcelSearchResults = matchedParcels
                .Take(searchResultLimit).ToList()
        };
    }

    private static ParcelSearchResultWithMatchedFieldsDto AsParcelWithMatchedFieldsDto(string searchString, Parcel parcel)
    {
        return new ParcelSearchResultWithMatchedFieldsDto()
        {
            Parcel = parcel.AsSearchResultDto(),
            MatchedFields = new Dictionary<ParcelSearchMatchEnum, bool>()
            {
                {
                    ParcelSearchMatchEnum.APN,
                    parcel.ParcelNumber.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                },
                {
                    ParcelSearchMatchEnum.WaterAccountName,
                    parcel.WaterAccount != null &&
                    !string.IsNullOrEmpty(parcel.WaterAccount?.WaterAccountName) &&
                    parcel.WaterAccount.WaterAccountName.Contains(searchString, StringComparison.OrdinalIgnoreCase)
                },
                {
                    ParcelSearchMatchEnum.ContactAddress,
                    parcel.OwnerAddress?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false
                },
                {
                    ParcelSearchMatchEnum.ContactName,
                    parcel.OwnerName?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false
                },
                {
                    ParcelSearchMatchEnum.WaterAccountNumber,
                    parcel.WaterAccount != null &&
                    parcel.WaterAccount.WaterAccountNumber.ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase)
                }
            }
        };
    }

    #region Parcel Acres

    public static List<ErrorMessage> ValidateAcresUpdate(QanatDbContext dbContext, int parcelID, ParcelAcreUpdateDto parcelAcreUpdateDto)
    {
        var results = new List<ErrorMessage>();

        if (parcelAcreUpdateDto.Acres <= 0)
        {
            results.Add(new ErrorMessage()
            {
                Type = "Parcel Acres",
                Message = "Parcel acres must be greater than 0."
            });
        }

        return results;
    }

    public static async Task<ParcelMinimalDto> UpdateAcres(QanatDbContext dbContext, int parcelID, ParcelAcreUpdateDto parcelUpdateAcresDto, UserDto callingUser)
    {
        var parcel = dbContext.Parcels.Single(x => x.ParcelID == parcelID);

        var previousArea = Math.Round(parcel.ParcelArea, 4, MidpointRounding.ToEven);
        var updatedAcres = Math.Round(parcelUpdateAcresDto.Acres, 4, MidpointRounding.ToEven);
        if (previousArea == updatedAcres) //MK 1/3/2025 after the release script to round these to 4 decimal places runs in production, we should change the data type probably to a Decimal(10,4) to enforce the rule and reduce instances of floating point comparisons. The rounding above should help with avoiding floating point issues for now hopefully.
        {
            //NOOP skip putting in a bogus audit entry.
            return parcel.AsParcelMinimalDto();
        }

        parcel.ParcelArea = updatedAcres;

        await dbContext.SaveChangesAsync();

        var newAuditEntry = ParcelHistories.CreateNew(parcel, callingUser.UserID);
        await dbContext.ParcelHistories.AddAsync(newAuditEntry);

        await dbContext.SaveChangesAsync();

        // we need to mark any existing parcel history records for the parcel as IsReviewed = true
        await ParcelHistories.MarkAsReviewedByParcelIDsAsync(dbContext, [parcel.ParcelID]);

        await dbContext.Entry(parcel).ReloadAsync();
        await dbContext.Entry(newAuditEntry).ReloadAsync();

        return parcel.AsParcelMinimalDto();
    }

    #endregion
}