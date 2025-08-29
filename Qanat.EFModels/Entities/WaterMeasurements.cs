using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Qanat.Common.Util;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WaterMeasurements
{
    public static async Task<List<WaterMeasurementSimpleDto>> CreateWaterMeasurements(QanatDbContext dbContext, List<WaterMeasurement> waterMeasurements)
    {
        dbContext.WaterMeasurements.AddRange(waterMeasurements);
        await dbContext.SaveChangesAsync();

        return waterMeasurements.Select(x => x.AsSimpleDto()).ToList();
    }

    public static async Task<WaterMeasurementCsvResponseDto> CreateFromCSV(QanatDbContext dbContext, List<ParcelTransactionCSV> records, DateTime effectiveDate, int waterMeasurementTypeID, int unitTypeID, int geographyID)
    {
        var unitTypeEnum = UnitType.AllLookupDictionary[unitTypeID].ToEnum;
        var transactionDate = DateTime.UtcNow;

        //MK 8/14/2024 -- This line below was causing issues for me... I think it's because the date is already in UTC. I'm commenting it out for now.
        //effectiveDate = effectiveDate.AddHours(8); // todo: convert effective date to utc date
        var waterMeasurements = new List<WaterMeasurement>();

        var usageLocationNames = new List<string>();
        var unmatchedLocationNames = new List<string>();

        var usageLocations = await UsageLocations.ListByGeographyAndReportedDate(dbContext, geographyID, effectiveDate);
        
        foreach (var record in records)
        {
            var usageLocation = usageLocations.FirstOrDefault(x => x.Name == record.UsageLocationName);
            if (usageLocation == null)
            {
                unmatchedLocationNames.Add(record.UsageLocationName);
                continue;
            }

            usageLocationNames.Add(record.UsageLocationName);
            var reportedValue = record.Quantity;
            var comment = record.Comment;
            var usageArea = (decimal)usageLocation.Area;
            var volume = ConvertReportedValueToAcreFeet(unitTypeEnum, reportedValue.GetValueOrDefault(0), usageArea);
            var depth = usageArea != 0 
                ? volume / usageArea 
                : 0;

            var waterMeasurement = new WaterMeasurement()
            {
                GeographyID = geographyID,
                UsageLocationID = usageLocation.UsageLocationID,
                WaterMeasurementTypeID = waterMeasurementTypeID,
                UnitTypeID = unitTypeID,
                LastUpdateDate = transactionDate,
                ReportedDate = effectiveDate,
                ReportedValueInNativeUnits = reportedValue,
                ReportedValueInAcreFeet = volume,
                ReportedValueInFeet = depth,
                FromManualUpload = true,
                Comment = comment
            };
            waterMeasurements.Add(waterMeasurement);
        }

        // overwrite existing records for specified effective date and water use type
        var parcelUsageRecordsToRemove = dbContext.WaterMeasurements
            .Include(x => x.UsageLocation)
            .Where(x => x.GeographyID == geographyID && x.ReportedDate == effectiveDate && x.WaterMeasurementTypeID == waterMeasurementTypeID && usageLocationNames.Contains(x.UsageLocation.Name));
        dbContext.RemoveRange(parcelUsageRecordsToRemove);

        dbContext.WaterMeasurements.AddRange(waterMeasurements);
        await dbContext.SaveChangesAsync();

        return new WaterMeasurementCsvResponseDto(waterMeasurements.Count, unmatchedLocationNames);
    }

    public static decimal ConvertReportedValueToAcreFeet(UnitTypeEnum unitTypeEnum, decimal reportedValue, decimal area)
    {
        return unitTypeEnum switch
        {
            UnitTypeEnum.Inches => UnitConversionHelper.ConvertInchesToAcreFeet(reportedValue, area),
            UnitTypeEnum.Millimeters => UnitConversionHelper.ConvertMillimetersToAcreFeet(reportedValue, area),
            UnitTypeEnum.AcreFeet => reportedValue,
            UnitTypeEnum.AcreFeetPerAcre => reportedValue * area,
            _ => throw new InvalidEnumArgumentException($"{unitTypeEnum.ToString()} cannot be uploaded via CSV.")
        };
    }

    public static List<ParcelWaterMeasurementChartDatumDto> ListAsParcelWaterMeasurementChartDatumDto(QanatDbContext dbContext, int geographyID, int parcelID, UserDto callingUser)
    {
        var usageLocationIDs = dbContext.UsageLocations.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.ParcelID == parcelID)
            .Select(x => x.UsageLocationID).ToList();

        var parcel = Parcels.GetByID(dbContext, parcelID);

        var filterToShowLandowner = false;
        if (parcel.WaterAccountID.HasValue)
        {
            var waterAccountUser = WaterAccountUsers.GetWaterAccountUserForUserIDAndWaterAccountID(dbContext, callingUser.UserID, parcel.WaterAccountID.Value);
            filterToShowLandowner = waterAccountUser != null; //MK 8/19/2024 -- Trying to match the logic in pWaterAccountMonthlyUsageSummary which just checks for the existence of a water account user for the calling user for the given water account.
        }

        var waterMeasurements = dbContext.WaterMeasurements
            .Include(x => x.WaterMeasurementType)
            .Where(x => usageLocationIDs.Contains(x.UsageLocationID));

        if (filterToShowLandowner)
        {
            waterMeasurements = waterMeasurements.Where(x => x.WaterMeasurementType.ShowToLandowner);
        }

        var parcelMeasurements = waterMeasurements
            .GroupBy(x => new { x.WaterMeasurementType.WaterMeasurementTypeName, x.ReportedDate }).AsEnumerable()
            .Select(x =>
            {
                var reportedValueSum = x.Sum(y => y.ReportedValueInAcreFeet);
                return new ParcelWaterMeasurementChartDatumDto()
                {
                    WaterMeasurementTypeName = x.Key.WaterMeasurementTypeName,
                    ReportedDate = x.Key.ReportedDate,
                    ReportedValueInAcreFeet = reportedValueSum > 0 ? reportedValueSum : 0
                };
            }).ToList();

        return parcelMeasurements;
    }
        
    public static async Task<List<WaterMeasurement>> ListByGeographyIDAndReportedDateAndOptionalWaterMeasurementTypeIDAndOptionalUsageLocationIDs(QanatDbContext dbContext, int geographyID, DateTime reportedDate, int? waterMeasurementTypeID, List<int>? usageLocationIDs = null)
    {
        var waterMeasurements = dbContext.WaterMeasurements.AsNoTracking()
            .Include(x => x.WaterMeasurementType)
            .Include(x => x.UsageLocation).ThenInclude(x => x.UsageLocationType).ThenInclude(x => x.WaterMeasurementType)
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == reportedDate.Date);

        if (waterMeasurementTypeID.HasValue)
        {
            waterMeasurements = waterMeasurements.Where(x => x.WaterMeasurementTypeID == waterMeasurementTypeID.Value);
        }

        if (usageLocationIDs != null && usageLocationIDs.Any())
        {
            waterMeasurements = waterMeasurements.Where(x => usageLocationIDs.Contains(x.UsageLocationID));
        }

        var result = await waterMeasurements.ToListAsync();
        return result;
    }

    public static async Task<List<WaterMeasurementDto>> ListCalculatedMeasurementsByGeographyIDAndDate(QanatDbContext dbContext, int geographyID, DateTime dateToCalculate)
    {
        var waterMeasurements = await dbContext.WaterMeasurements.AsNoTracking()
            .Include(x => x.WaterMeasurementType)
            .Include(x => x.UsageLocation)
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementType.WaterMeasurementCalculationTypeID.HasValue)
            .OrderBy(x => x.UsageLocation.Name).ThenBy(x => x.UsageLocation.Area).ThenBy(x => x.WaterMeasurementType.SortOrder)
            .ToListAsync();

        return waterMeasurements.Select(x => x.AsWaterMeasurementDto()).ToList();
    }

    public static async Task<WaterAccountBudgetStatDto> GetWaterMeasurementStatsForWaterBudget(QanatDbContext dbContext, int geographyID, int waterAccountID, int reportingYear, UserDto callingUser)
    {
        var reportingPeriodDto = await ReportingPeriods.GetByGeographyIDAndYearAsync(dbContext, geographyID, reportingYear, callingUser);
        if (reportingPeriodDto == null)
        {
            return new WaterAccountBudgetStatDto();
        }

        var usageLocationIDs = await dbContext.WaterAccountParcels.AsNoTracking()
            .Include(x => x.Parcel)
            .ThenInclude(x => x.UsageLocations)
            .Where(x => x.GeographyID == geographyID
                        && x.WaterAccountID == waterAccountID && x.ReportingPeriodID == reportingPeriodDto.ReportingPeriodID)
            .SelectMany(x =>
                x.Parcel.UsageLocations.Where(x => x.ReportingPeriodID == reportingPeriodDto.ReportingPeriodID)
                    .Select(y => y.UsageLocationID))
            .ToListAsync();

        var geography = Geographies.GetByID(dbContext, geographyID);
        var waterMeasurementTypeIDsToFilterTo = new List<int?>()
        {
            geography.WaterBudgetSlotAWaterMeasurementTypeID,
            geography.WaterBudgetSlotBWaterMeasurementTypeID,
            geography.WaterBudgetSlotCWaterMeasurementTypeID
        }.Where(x => x.HasValue);


        var reportingPeriodStart = reportingPeriodDto.StartDate;
        var reportingPeriodEnd = reportingPeriodDto.EndDate;

        var waterMeasurements = await dbContext.WaterMeasurements.AsNoTracking()
            .Include(x => x.WaterMeasurementType)
            .Where(x => x.GeographyID == geographyID)
            .Where(x => usageLocationIDs.Contains(x.UsageLocationID))
            .Where(x => waterMeasurementTypeIDsToFilterTo.Contains(x.WaterMeasurementTypeID))
            .Where(x => x.ReportedDate >= reportingPeriodStart && x.ReportedDate <= reportingPeriodEnd)
            .ToListAsync();

        var waterMeasurementsSlotA = waterMeasurements.Where(x => x.WaterMeasurementTypeID == geography.WaterBudgetSlotAWaterMeasurementTypeID);
        var slotAValueInAcreFeet = waterMeasurementsSlotA.Sum(x => x.ReportedValueInAcreFeet);

        var waterMeasurementsSlotB = waterMeasurements.Where(x => x.WaterMeasurementTypeID == geography.WaterBudgetSlotBWaterMeasurementTypeID);
        var slotBValueInAcreFeet = waterMeasurementsSlotB.Sum(x => x.ReportedValueInAcreFeet);

        var waterMeasurementsSlotC = waterMeasurements.Where(x => x.WaterMeasurementTypeID == geography.WaterBudgetSlotCWaterMeasurementTypeID);
        var slotCValueInAcreFeet = waterMeasurementsSlotC.Sum(x => x.ReportedValueInAcreFeet);

        var result = new WaterAccountBudgetStatDto()
        {
            SlotAValueInAcreFeet = slotAValueInAcreFeet,
            SlotBValueInAcreFeet = slotBValueInAcreFeet,
            SlotCValueInAcreFeet = slotCValueInAcreFeet
        };

        return result;
    }

    public static async Task DeleteWaterMeasurements(QanatDbContext dbContext, int geographyID, int waterMeasurementTypeID, DateTime dateToDelete, List<int> usageLocationIDs = null)
    {
        var waterMeasurements = dbContext.WaterMeasurements
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == waterMeasurementTypeID && x.ReportedDate.Date.Year == dateToDelete.Date.Year && x.ReportedDate.Date.Month == dateToDelete.Date.Month);

        if (usageLocationIDs != null)
        {
            waterMeasurements = waterMeasurements.Where(x => usageLocationIDs.Contains(x.UsageLocationID));
        }

        dbContext.WaterMeasurements.RemoveRange(waterMeasurements);
        await dbContext.SaveChangesAsync();
    }

    #region Bulk Set

    public static async Task<List<ErrorMessage>> ValidateBulkSetAsync(QanatDbContext dbContext, int geographyID, WaterMeasurementBulkSetDto waterMeasurementBulkSetDto)
    {
        var errors = new List<ErrorMessage>();

        var waterMeasurementType = await WaterMeasurementTypes.GetAsync(dbContext, geographyID, waterMeasurementBulkSetDto.WaterMeasurementTypeID!.Value);
        if (waterMeasurementType == null)
        {
            errors.Add(new ErrorMessage() { Type = "Water Measurement Type", Message = $"Could not find a Water Measurement Type with the ID {waterMeasurementBulkSetDto.WaterMeasurementTypeID}." });
        }
        else if (!waterMeasurementType.IsUserEditable)
        {
            errors.Add(new ErrorMessage() { Type = "Water Measurement Type", Message = $"Water Measurement Type {waterMeasurementType.WaterMeasurementTypeName} is not user editable." });
        }

        var reportingPeriods = await dbContext.ReportingPeriods.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        var validYears = reportingPeriods.Select(x => x.EndDate.Year);
        if (!validYears.Contains(waterMeasurementBulkSetDto.Year!.Value))
        {
            errors.Add(new ErrorMessage() { Type = "Year", Message = $"Year {waterMeasurementBulkSetDto.Year} is not a valid year for the selected geography." });
        }

        var validMonths = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        if (!validMonths.Contains(waterMeasurementBulkSetDto.Month!.Value))
        {
            errors.Add(new ErrorMessage() { Type = "Month", Message = $"Month {waterMeasurementBulkSetDto.Month} is not a valid month." });
        }

        return errors;
    }

    public static async Task BulkSetWaterMeasurementsAsync(QanatDbContext dbContext, int geographyID, DateTime date, WaterMeasurementBulkSetDto waterMeasurementBulkSetDto)
    {
        //Using a transaction to ensure that all water measurements are set or none are set if there is an error.
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(dbContext, geographyID, waterMeasurementBulkSetDto.Year!.Value);

            await dbContext.WaterMeasurements
                .Where(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == waterMeasurementBulkSetDto.WaterMeasurementTypeID && x.ReportedDate == date)
                .ExecuteDeleteAsync();

            var usageLocations = await dbContext.UsageLocations.AsNoTracking()
                .Where(x => x.GeographyID == geographyID && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID)
                .ToListAsync();

            var waterMeasurements = usageLocations.Select(x => new WaterMeasurement()
            {
                GeographyID = geographyID,
                UsageLocationID = x.UsageLocationID,
                WaterMeasurementTypeID = waterMeasurementBulkSetDto.WaterMeasurementTypeID,
                UnitTypeID = UnitType.AcreFeetPerAcre.UnitTypeID,
                ReportedDate = date,
                ReportedValueInNativeUnits = waterMeasurementBulkSetDto.ValueInAcreFeetPerAcre,
                ReportedValueInAcreFeet = Math.Round(waterMeasurementBulkSetDto.ValueInAcreFeetPerAcre * (decimal)x.Area, 4, MidpointRounding.ToEven),
                ReportedValueInFeet = waterMeasurementBulkSetDto.ValueInAcreFeetPerAcre,
                FromManualUpload = true,
                Comment = waterMeasurementBulkSetDto.Comment,
                LastUpdateDate = DateTime.UtcNow
            });

            await dbContext.WaterMeasurements.AddRangeAsync(waterMeasurements);

            await dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    #endregion
}