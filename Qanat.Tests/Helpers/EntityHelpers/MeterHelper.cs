using Qanat.Common.Util;
using Qanat.EFModels.Entities;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Qanat.Tests.Helpers.EntityHelpers;

public static class MeterHelper
{
    public static async Task<Meter> AddMeterAsync(int geographyID, string? serialNumber = null)
    {
        if (string.IsNullOrEmpty(serialNumber))
        {
            serialNumber = Guid.NewGuid().ToString().Substring(0, 25);
        }

        var addMeterResult = await AssemblySteps.QanatDbContext.Meters.AddAsync(new Meter()
        {
            GeographyID = geographyID,
            SerialNumber = serialNumber,
            MeterStatusID = MeterStatus.Active.MeterStatusID
        });

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        var meter = addMeterResult.Entity;
        await AssemblySteps.QanatDbContext.Entry(meter).ReloadAsync();
        return meter;
    }

    public static async Task<WellMeter> AssociateMeterToWellAsync(int wellID, int meterID, DateTime startDate)
    {
        var addWellMeterResult = await AssemblySteps.QanatDbContext.WellMeters.AddAsync(new WellMeter()
        {
            WellID = wellID,
            MeterID = meterID,
            StartDate = startDate
        });

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        var wellMeter = addWellMeterResult.Entity;
        await AssemblySteps.QanatDbContext.Entry(wellMeter).ReloadAsync();
        return wellMeter;
    }
}

public static class MeterReadingHelper
{
    public static async Task<MeterReading> AddMeterReadingAsync(int geographyID, int wellID, int meterID, DateTime readingDate, string readingTime, int meterReadingUnitTypeID, decimal previousReading, decimal currentReading)
    {
        var time = DateTime.ParseExact(readingTime, "HH:mm", CultureInfo.InvariantCulture);
        var date = readingDate.Date.Add(time.TimeOfDay);

        var volume = currentReading - previousReading;
        var volumeInAcreFeet = meterReadingUnitTypeID == MeterReadingUnitType.Gallons.MeterReadingUnitTypeID
            ? UnitConversionHelper.ConvertGallonsToAcreFeet(volume)
            : volume;

        var addMeterReadingResult = await AssemblySteps.QanatDbContext.MeterReadings.AddAsync(new MeterReading()
        {
            GeographyID = geographyID,
            WellID = wellID,
            MeterID = meterID,
            MeterReadingUnitTypeID = meterReadingUnitTypeID,

            ReadingDate = date,
            PreviousReading = previousReading,
            CurrentReading = currentReading,

            Volume = volume,
            VolumeInAcreFeet = volumeInAcreFeet
        });

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        var meterReading = addMeterReadingResult.Entity;
        await AssemblySteps.QanatDbContext.Entry(meterReading).ReloadAsync();
        return meterReading;
    }
}