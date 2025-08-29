using Microsoft.EntityFrameworkCore;
using Qanat.Common.Util;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class MeterReadingMonthlyInterpolations
{
    // MK 3/26/2025: It's very possible that in the future rebuilding all interpolations for a meter will be too slow. However trying to scope it correctly proved to be difficult. Revisit this if we get complaints... (or stick it in a hangfire task...)
    public static async Task<List<MeterReadingMonthlyInterpolationSimpleDto>> RebuildMonthlyInterpolationsAsync(QanatDbContext dbContext, int meterID)
    {
        var result = new List<MeterReadingMonthlyInterpolationSimpleDto>();

        // Load all meter readings for the given meter, ordered chronologically.
        var meterReadings = await dbContext.MeterReadings.AsNoTracking()
            .Where(x => x.MeterID == meterID)
            .OrderBy(x => x.ReadingDate)
            .ToListAsync();

        // Need at least two readings to form one interval
        if (meterReadings.Count <= 1)
        {
            return result;
        }

        var rawInterpolations = new List<MeterReadingMonthlyInterpolation>();

        // Handle the first reading separately, since it has no prior reading.
        // We treat it as a 1-day interval by passing it as both the start and end, ensuring its volume is included without polluting prior months.
        var firstReading = meterReadings[0];
        var initialInterpolation = HandleReadings(firstReading, firstReading, firstReading.MeterReadingUnitTypeID);
        rawInterpolations.AddRange(initialInterpolation);

        // Iterate over each consecutive pair of readings to calculate usage intervals.
        for (var i = 1; i < meterReadings.Count; i++)
        {
            var start = meterReadings[i - 1];
            var end = meterReadings[i];

            var interpolations = HandleReadings(start, end, firstReading.MeterReadingUnitTypeID);
            rawInterpolations.AddRange(interpolations);
        }

        // Group interpolations by unique (geography, well, meter, date) to merge partial-month contributions.
        var groupedInterpolations = rawInterpolations
            .GroupBy(x => new { x.GeographyID, x.WellID, x.MeterID, x.Date })
            .Select(g =>
            {
                return new MeterReadingMonthlyInterpolation
                {
                    GeographyID = g.Key.GeographyID,
                    WellID = g.Key.WellID,
                    MeterID = g.Key.MeterID,
                    Date = g.Key.Date,
                    MeterReadingUnitTypeID = firstReading.MeterReadingUnitTypeID, // The first reading "wins" for the unit type.
                    InterpolatedVolume = g.Sum(x => x.InterpolatedVolume),
                    InterpolatedVolumeInAcreFeet = g.Sum(x => x.InterpolatedVolumeInAcreFeet)
                };
            })
            .ToList();

        // Clear out old interpolations for this meter.
        var existing = dbContext.MeterReadingMonthlyInterpolations.Where(x => x.MeterID == meterID);
        await existing.ExecuteDeleteAsync();

        await dbContext.AddRangeAsync(groupedInterpolations);
        await dbContext.SaveChangesAsync();

        groupedInterpolations.ForEach(x => dbContext.Entry(x).Reload());

        var meter = await dbContext.Meters.AsNoTracking()
            .Include(x => x.WellMeters).ThenInclude(x => x.Well)
            .FirstOrDefaultAsync(x => x.MeterID == meterID);

        var meteredExtractionWaterMeasurementType = await dbContext.WaterMeasurementTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GeographyID == meter.GeographyID && x.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.Meter.WaterMeasurementCategoryTypeID);

        if (meteredExtractionWaterMeasurementType != null)
        {
            await RebuildWaterMeasurementsForMeter(dbContext, meter, meteredExtractionWaterMeasurementType, groupedInterpolations);
        }

        result = groupedInterpolations.Select(x => x.AsSimpleDto()).ToList();
        return result;
    }

    private static List<MeterReadingMonthlyInterpolation> HandleReadings(MeterReading previousReading, MeterReading currentReading, int firstUnitType)
    {
        var monthlyInterpolations = new List<MeterReadingMonthlyInterpolation>();

        var currentReadingDate = currentReading.ReadingDate.Date;
        var previousReadingDate = previousReading.ReadingDate.Date;

        var isSingleReading = previousReading.MeterReadingID == currentReading.MeterReadingID;

        // Determine the exclusive end of the interval (except for single fake reading)
        var intervalEndExclusive = isSingleReading
            ? currentReadingDate
            : currentReadingDate.AddDays(-1);

        // Interval length (inclusive start, exclusive end)
        var totalDays = isSingleReading
            ? 1
            : (decimal)(intervalEndExclusive - previousReadingDate).TotalDays + 1;

        // Unit conversion if needed
        var volume = currentReading.Volume;
        if (currentReading.MeterReadingUnitTypeID != firstUnitType)
        {
            switch (currentReading.MeterReadingUnitTypeID)
            {
                case (int)MeterReadingUnitTypeEnum.AcreFeet:
                    volume = UnitConversionHelper.ConvertAcreFeetToGallons(volume);
                    break;
                case (int)MeterReadingUnitTypeEnum.Gallons:
                    volume = UnitConversionHelper.ConvertGallonsToAcreFeet(volume);
                    break;
                default:
                    throw new Exception();
            }
        }

        var dailyVolumeRate = volume / totalDays;

        var currentMonth = new DateTime(previousReadingDate.Year, previousReadingDate.Month, 1);
        while (currentMonth <= currentReadingDate)
        {
            var firstDayOfMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month);
            var lastDayOfMonth = new DateTime(currentMonth.Year, currentMonth.Month, daysInMonth);

            var rangeStart = previousReadingDate > firstDayOfMonth
                ? previousReadingDate
                : firstDayOfMonth;

            var rangeEnd = intervalEndExclusive < lastDayOfMonth
                ? intervalEndExclusive
                : lastDayOfMonth;

            var daysInRange = (decimal)(rangeEnd - rangeStart).TotalDays + 1;
            if (daysInRange > 0)
            {
                var monthlyContribution = daysInRange * dailyVolumeRate;
                var volumeInAcreFeet = firstUnitType == MeterReadingUnitType.Gallons.MeterReadingUnitTypeID
                    ? UnitConversionHelper.ConvertGallonsToAcreFeet(monthlyContribution)
                    : monthlyContribution;

                monthlyInterpolations.Add(new MeterReadingMonthlyInterpolation
                {
                    GeographyID = previousReading.GeographyID,
                    WellID = previousReading.WellID,
                    MeterID = previousReading.MeterID,
                    MeterReadingUnitTypeID = firstUnitType,
                    Date = currentMonth,
                    InterpolatedVolume = monthlyContribution,
                    InterpolatedVolumeInAcreFeet = volumeInAcreFeet,
                });
            }

            currentMonth = currentMonth.AddMonths(1);
        }

        return monthlyInterpolations;
    }

    private static async Task RebuildWaterMeasurementsForMeter(QanatDbContext dbContext, Meter meter, WaterMeasurementType waterMeasurementType, List<MeterReadingMonthlyInterpolation> monthlyInterpolations)
    {
        var well = meter.WellMeters.FirstOrDefault(x => !x.EndDate.HasValue)?.Well;
        if (well == null)
        {
            return;
        }

        // Find all irrigated parcels for the meter's well.
        var wellIrrigatedParcels = await dbContext.WellIrrigatedParcels.AsNoTracking()
            .Include(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.ReportingPeriod)
            .Where(x => x.WellID == well.WellID)
            .ToListAsync();

        var minDate = monthlyInterpolations.Min(x => x.Date);
        var maxDate = monthlyInterpolations.Max(x => x.Date);

        // Filter out usage locations that are not within the date range of the interpolations.
        var usageLocations = wellIrrigatedParcels.SelectMany(x => x.Parcel.UsageLocations)
            .Where(x => x.ReportingPeriod.StartDate.Date <= maxDate.Date && x.ReportingPeriod.EndDate.Date >= minDate.Date)
            .ToList();

        var rawWaterMeasurements = new List<WaterMeasurement>();
        foreach (var meterReadingMonthlyInterpolation in monthlyInterpolations)
        {
            var rawWaterMeasurementsForMonth = await RebuildExtractedMeterWaterMeasurementForUsageLocations(dbContext, meter.GeographyID, waterMeasurementType, usageLocations, meterReadingMonthlyInterpolation.Date);
            rawWaterMeasurements.AddRange(rawWaterMeasurementsForMonth);
        }

        // Group the water measurements and sum the reported values.
        var groupedWaterMeasurements = rawWaterMeasurements.GroupBy(x => new { x.GeographyID, x.UsageLocationID, x.WaterMeasurementTypeID, x.UnitTypeID, x.ReportedDate }).ToList();
        var waterMeasurements = groupedWaterMeasurements
            .Select(g =>
            {
                var commentCSV = string.Join(", ", g.Select(x => x.Comment));

                var totalVolume = g.Sum(x => x.ReportedValueInAcreFeet);
                var usageLocation = usageLocations.Single(x => x.UsageLocationID == g.Key.UsageLocationID);
                var totalDepth = totalVolume / (decimal)usageLocation.Area;

                return new WaterMeasurement
                {
                    GeographyID = g.Key.GeographyID,
                    UsageLocationID = g.Key.UsageLocationID,
                    WaterMeasurementTypeID = g.Key.WaterMeasurementTypeID,
                    UnitTypeID = g.Key.UnitTypeID,
                    ReportedDate = g.Key.ReportedDate,
                    ReportedValueInAcreFeet = totalVolume,
                    ReportedValueInFeet = totalDepth,
                    LastUpdateDate = DateTime.UtcNow,
                    FromManualUpload = false,
                    Comment = $"Extracted from the following meters: {commentCSV}."
                };
            })
            .ToList();

        var usageLocationIDs = groupedWaterMeasurements.Select(x => x.Key.UsageLocationID).Distinct().ToList();
        var reportedDates = groupedWaterMeasurements.Select(x => x.Key.ReportedDate).Distinct().ToList();

        var existingWaterMeasurements = dbContext.WaterMeasurements
            .Where(x => x.WaterMeasurementTypeID == waterMeasurementType.WaterMeasurementTypeID && usageLocationIDs.Contains(x.UsageLocationID) && reportedDates.Contains(x.ReportedDate));

        await existingWaterMeasurements.ExecuteDeleteAsync();

        await dbContext.WaterMeasurements.AddRangeAsync(waterMeasurements);
        await dbContext.SaveChangesAsync();

        // Run calculations for the water measurement type for each month affected.
        var dates = groupedWaterMeasurements.Select(x => x.Key.ReportedDate).Distinct().ToList();
        foreach (var date in dates)
        {
            await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(dbContext, meter.GeographyID, waterMeasurementType.WaterMeasurementTypeID, date);
        }
    }

    public static async Task<List<WaterMeasurement>> RebuildExtractedMeterWaterMeasurementForUsageLocations(QanatDbContext dbContext, int geographyID, WaterMeasurementType waterMeasurementType, List<UsageLocation> usageLocations, DateTime date)
    {
        var rawWaterMeasurements = new List<WaterMeasurement>();
        foreach (var usageLocation in usageLocations)
        {
            var wellsIrrigatingThisUsageLocation = await dbContext.WellIrrigatedParcels.AsNoTracking()
                .Include(x => x.Well).ThenInclude(x => x.WellMeters).ThenInclude(x => x.Meter)
                .Include(x => x.Parcel).ThenInclude(x => x.UsageLocations).ThenInclude(x => x.ReportingPeriod)
                .Where(x => x.ParcelID == usageLocation.ParcelID)
                .ToListAsync();

            foreach (var irrigatedWell in wellsIrrigatingThisUsageLocation)
            {
                var activeMeters = irrigatedWell.Well.WellMeters
                    .Where(x => x.WellID == irrigatedWell.WellID
                             && (x.StartDate.Year < date.Year || x.StartDate.Year == date.Year && x.StartDate.Month <= date.Month)
                             && (!x.EndDate.HasValue || x.EndDate.Value.Year > date.Year || x.EndDate.Value.Year == date.Year && x.EndDate.Value.Month >= date.Month));


                var parcelIDsForWell = await dbContext.WellIrrigatedParcels.AsNoTracking()
                    .Where(x => x.WellID == irrigatedWell.WellID)
                    .Select(x => x.ParcelID)
                    .Distinct()
                    .ToListAsync();

                var usageLocationsForWell = await dbContext.UsageLocations.AsNoTracking()
                    .Include(x => x.ReportingPeriod)
                    .Where(ul => parcelIDsForWell.Contains(ul.ParcelID)
                                && (ul.ReportingPeriod.StartDate.Year < date.Year || ul.ReportingPeriod.StartDate.Year == date.Year && ul.ReportingPeriod.StartDate.Month <= date.Month)
                                && (ul.ReportingPeriod.EndDate.Year > date.Year || ul.ReportingPeriod.EndDate.Year == date.Year && ul.ReportingPeriod.EndDate.Month >= date.Month)
                    )
                    .ToListAsync();

                var totalArea = usageLocationsForWell.Sum(x => x.Area);
                var areaApportion = totalArea != 0 ? (decimal)(usageLocation.Area / totalArea) : 0m;

                var interpolations = await dbContext.MeterReadingMonthlyInterpolations.AsNoTracking()
                    .Where(x => activeMeters.Select(m => m.MeterID).Contains(x.MeterID) && x.Date.Date == date.Date)
                    .Include(meterReadingMonthlyInterpolation => meterReadingMonthlyInterpolation.Meter)
                    .ToListAsync();

                foreach (var interpolation in interpolations)
                {
                    var daysInMonth = DateTime.DaysInMonth(interpolation.Date.Year, interpolation.Date.Month);
                    var endOfMonth = new DateTime(interpolation.Date.Year, interpolation.Date.Month, daysInMonth);

                    var interpolationInInches = interpolation.MeterReadingUnitTypeID == MeterReadingUnitType.Gallons.MeterReadingUnitTypeID
                        ? UnitConversionHelper.ConvertGallonsToInches(interpolation.InterpolatedVolume, (decimal)usageLocation.Area)
                        : UnitConversionHelper.ConvertAcreFeetToInches(interpolation.InterpolatedVolumeInAcreFeet, (decimal)usageLocation.Area);

                    var apportionOfVolume = interpolationInInches * areaApportion;
                    var apportionOfVolumeInAcreFeet = interpolation.InterpolatedVolumeInAcreFeet * areaApportion;

                    var apportionOfDepth = apportionOfVolume / (decimal)usageLocation.Area;

                    var waterMeasurement = new WaterMeasurement()
                    {
                        GeographyID = interpolation.GeographyID,
                        UsageLocationID = usageLocation.UsageLocationID,
                        WaterMeasurementTypeID = waterMeasurementType.WaterMeasurementTypeID,
                        ReportedDate = endOfMonth,
                        ReportedValueInAcreFeet = apportionOfVolumeInAcreFeet,
                        ReportedValueInFeet = apportionOfDepth,
                        LastUpdateDate = DateTime.UtcNow,
                        FromManualUpload = false,
                        Comment = $"{interpolation.Meter.SerialNumber}"
                    };

                    rawWaterMeasurements.Add(waterMeasurement);
                }
            }
        }

        return rawWaterMeasurements;
    }

    public static async Task<List<MeterReadingMonthlyInterpolationSimpleDto>> ListMonthlyInterpolationsByWellIDAsDtoAsync(QanatDbContext dbContext, int geographyID, int wellID, int meterID)
    {
        var monthlyInterpolations = await dbContext.MeterReadingMonthlyInterpolations.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WellID == wellID && x.MeterID == meterID)
            .Select(x => x.AsSimpleDto())
            .ToListAsync();

        return monthlyInterpolations;
    }
}