using Microsoft.EntityFrameworkCore;
using Qanat.Common.Util;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class WaterMeasurements
{
    public static async Task<WaterMeasurementCsvResponseDto> CreateFromCSV(QanatDbContext dbContext, List<ParcelTransactionCSV> records, DateTime effectiveDate, int waterMeasurementTypeID, int unitTypeID, int geographyID)
    {
        var unitTypeEnum = UnitType.AllLookupDictionary[unitTypeID].ToEnum;
        var transactionDate = DateTime.UtcNow;

        //MK 8/14/2024 -- This line below was causing issues for me... I think it's because the date is already in UTC. I'm commenting it out for now.
        //effectiveDate = effectiveDate.AddHours(8); // todo: convert effective date to utc date
        var waterMeasurements = new List<WaterMeasurement>();

        var usageEntityNames = new List<string>();
        var unmatchedUsageEntityNames = new List<string>();

        var usageEntityAreaDictionary = dbContext.UsageEntities.AsNoTracking()
            .Where(x => x.GeographyID == geographyID).ToDictionary(x => x.UsageEntityName, y => y.UsageEntityArea);

        
        foreach (var record in records)
        {
            if (!usageEntityAreaDictionary.ContainsKey(record.UsageEntityName))
            {
                unmatchedUsageEntityNames.Add(record.UsageEntityName);
                continue;
            }
            usageEntityNames.Add(record.UsageEntityName);
            var reportedValue = record.Quantity;
            var comment = record.Comment;
            var waterMeasurement = new WaterMeasurement()
            {
                UsageEntityName = record.UsageEntityName,
                UsageEntityArea = (decimal)usageEntityAreaDictionary[record.UsageEntityName],
                LastUpdateDate = transactionDate,
                ReportedDate = effectiveDate,
                ReportedValue = (decimal)reportedValue,
                ReportedValueInAcreFeet = ConvertReportedValueToAcreFeet(unitTypeEnum, (decimal)reportedValue, (decimal)usageEntityAreaDictionary[record.UsageEntityName]),
                WaterMeasurementTypeID = waterMeasurementTypeID,
                UnitTypeID = unitTypeID,
                GeographyID = geographyID,
                FromManualUpload = true,
                Comment = comment
            };
            waterMeasurements.Add(waterMeasurement);
        }

        // overwrite existing records for specified effective date and water use type
        var parcelUsageRecordsToRemove = dbContext.WaterMeasurements
            .Where(x => x.GeographyID == geographyID && x.ReportedDate == effectiveDate && 
                        x.WaterMeasurementTypeID == waterMeasurementTypeID && usageEntityNames.Contains(x.UsageEntityName));
        dbContext.RemoveRange(parcelUsageRecordsToRemove);

        dbContext.WaterMeasurements.AddRange(waterMeasurements);
        await dbContext.SaveChangesAsync();

        return new WaterMeasurementCsvResponseDto(waterMeasurements.Count, unmatchedUsageEntityNames);
    }

    public static decimal ConvertReportedValueToAcreFeet(UnitTypeEnum unitTypeEnum, decimal reportedValue, decimal usageEntityArea)
    {
        return unitTypeEnum switch
        {
            UnitTypeEnum.Inches => UnitConversionHelper.ConvertInchesToAcreFeet(reportedValue, usageEntityArea),
            UnitTypeEnum.Millimeters => UnitConversionHelper.ConvertMillimetersToAcreFeet(reportedValue, usageEntityArea),
            _ => reportedValue
        };
    }

    public static List<ParcelWaterMeasurementChartDatumDto> ListAsParcelWaterMeasurementChartDatumDto(QanatDbContext dbContext, int geographyID, int parcelID, UserDto callingUser)
    {
        var usageEntityNames = dbContext.UsageEntities.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.ParcelID == parcelID)
            .Select(x => x.UsageEntityName).ToList();

        var parcel = Parcels.GetByID(dbContext, parcelID);

        var filterToShowLandowner = false;
        if (parcel.WaterAccountID.HasValue)
        {
            var waterAccountUser = WaterAccountUsers.GetWaterAccountUserForUserIDAndWaterAccountID(dbContext, callingUser.UserID, parcel.WaterAccountID.Value);
            filterToShowLandowner = waterAccountUser != null; //MK 8/19/2024 -- Trying to match the logic in pWaterAccountMonthlyUsageSummary which just checks for the existence of a water account user for the calling user for the given water account.
        }

        var waterMeasurements = dbContext.WaterMeasurements
            .Include(x => x.WaterMeasurementType)
            .AsNoTracking()
            .Where(x => usageEntityNames.Contains(x.UsageEntityName));

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

    public static async Task<List<WaterMeasurementDto>> ListCalculatedMeasurementsByGeographyIDAndDate(QanatDbContext dbContext, int geographyID, DateTime dateToCalculate)
    {
        var waterMeasurements = await dbContext.WaterMeasurements.AsNoTracking()
            .Include(x => x.WaterMeasurementType)
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementType.WaterMeasurementCalculationTypeID.HasValue)
            .OrderBy(x => x.UsageEntityName).ThenBy(x => x.UsageEntityArea).ThenBy(x => x.WaterMeasurementType.SortOrder)
            .ToListAsync();

        return waterMeasurements.Select(x => x.AsWaterMeasurementDto()).ToList();
    }

    public static async Task<List<WaterMeasurementDto>> CreateWaterMeasurements(QanatDbContext dbContext, List<WaterMeasurement> waterMeasurements)
    {
        dbContext.WaterMeasurements.AddRange(waterMeasurements);
        await dbContext.SaveChangesAsync();

        return waterMeasurements.Select(x => x.AsWaterMeasurementDto()).ToList();
    }

    public static async Task DeleteWaterMeasurements(QanatDbContext dbContext, int geographyID, int waterMeasurementTypeID, DateTime dateToDelete)
    {
        var waterMeasurements = dbContext.WaterMeasurements
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == waterMeasurementTypeID && x.ReportedDate.Date.Year == dateToDelete.Date.Year && x.ReportedDate.Date.Month == dateToDelete.Date.Month);

        dbContext.WaterMeasurements.RemoveRange(waterMeasurements);
        await dbContext.SaveChangesAsync();
    }

    public static async Task<WaterAccountBudgetStatDto> GetWaterMeasurementStatsForWaterBudget(QanatDbContext dbContext, int geographyID, int waterAccountID, int reportedYear, UserDto callingUser)
    {
        var usageEntitiesForWaterAccount = await dbContext.UsageEntities.AsNoTracking()
            .Include(x => x.Parcel)
            .Where(x => x.GeographyID == geographyID)
            .Where(x => x.Parcel.WaterAccountID == waterAccountID)
            .Select(x => x.UsageEntityName)
            .ToListAsync();

        var geography = Geographies.GetByID(dbContext, geographyID);
        var waterMeasurementTypeIDsToFilterTo = new List<int?>()
        {
            geography.WaterBudgetSlotAWaterMeasurementTypeID,
            geography.WaterBudgetSlotBWaterMeasurementTypeID,
            geography.WaterBudgetSlotCWaterMeasurementTypeID
        }.Where(x => x.HasValue);

        var reportingPeriodDto = await ReportingPeriods.GetByGeographyIDAndYearAsync(dbContext, geographyID, reportedYear, callingUser);
        if (reportingPeriodDto == null)
        {
            return new WaterAccountBudgetStatDto();
        }

        var reportingPeriodStart = reportingPeriodDto.StartDate;
        var reportingPeriodEnd = reportingPeriodDto.EndDate;

        var waterMeasurements = await dbContext.WaterMeasurements.AsNoTracking()
            .Include(x => x.WaterMeasurementType)
            .Where(x => x.GeographyID == geographyID )
            .Where(x => usageEntitiesForWaterAccount.Contains(x.UsageEntityName))
            .Where(x => waterMeasurementTypeIDsToFilterTo.Contains(x.WaterMeasurementTypeID))
            .Where(x => x.ReportedDate >= reportingPeriodStart && x.ReportedDate <= reportingPeriodEnd)
            .ToListAsync();

        var waterMeasurementsSlotA = waterMeasurements.Where(x => x.WaterMeasurementTypeID == geography.WaterBudgetSlotAWaterMeasurementTypeID);
        var slotAValueInAcreFeet = waterMeasurementsSlotA.Sum(x => x.ReportedValueInAcreFeet);

        var waterMeasurementsSlotB = waterMeasurements .Where(x => x.WaterMeasurementTypeID == geography.WaterBudgetSlotBWaterMeasurementTypeID);
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
}