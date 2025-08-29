using CsvHelper;
using CsvHelper.TypeConversion;
using Microsoft.EntityFrameworkCore;
using Qanat.Common.Util;
using Qanat.Models.DataTransferObjects;
using System.Globalization;
using MissingFieldException = CsvHelper.MissingFieldException;

namespace Qanat.EFModels.Entities;

public static class MeterReadings
{
    public static List<ErrorMessage> ValidateMeterReadingUpsertAsync(QanatDbContext dbContext, int geographyID, int wellID, int meterID, MeterReadingUpsertDto meterReadingUpsertDto, int? meterReadingID = null)
    {
        var errors = new List<ErrorMessage>();

        var wellMeter = dbContext.WellMeters
            .AsNoTracking()
            .Include(x => x.Well)
            .Include(x => x.Meter)
            .SingleOrDefault(x => x.Well.GeographyID == geographyID && x.Meter.GeographyID == geographyID && x.WellID == wellID && x.MeterID == meterID && !x.EndDate.HasValue);

        if (wellMeter == null)
        {
            errors.Add(new ErrorMessage() { Type = "Meter", Message = "The selected meter is not currently assigned to the selected well." });
        }
        else if (meterReadingUpsertDto.ReadingDate!.Value.Date < wellMeter.StartDate.Date)
        {
            errors.Add(new ErrorMessage() { Type = "Invalid Date", Message = $"Reading date cannot be earlier than the date meter was assigned to well: {wellMeter.StartDate.ToShortDateString()}." });
        }

        var conflictingMeterReading = dbContext.MeterReadings.AsNoTracking()
            .SingleOrDefault(x => x.MeterID == meterID && x.WellID == wellID && x.ReadingDate.Date == meterReadingUpsertDto.ReadingDate.Value.Date && x.MeterReadingID != meterReadingID);

        if (conflictingMeterReading != null)
        {
            errors.Add(new ErrorMessage() { Type = "Duplicate Date", Message = "A meter reading already exists for this date and well." });
        }

        return errors;
    }

    public static async Task<MeterReadingDto> CreateAsync(QanatDbContext dbContext, int geographyID, int wellID, int meterID, MeterReadingUpsertDto meterReadingUpsertDto)
    {
        var newMeterReading = GetMeterReadingFromUpsertDto(geographyID, wellID, meterID, meterReadingUpsertDto);
        await dbContext.MeterReadings.AddAsync(newMeterReading);
        await dbContext.SaveChangesAsync();

        await dbContext.Entry(newMeterReading).ReloadAsync();

        await MeterReadingMonthlyInterpolations.RebuildMonthlyInterpolationsAsync(dbContext, meterID);

        var meterReadingDto = await GetByIDAsDtoAsync(dbContext, geographyID, wellID, meterID, newMeterReading.MeterReadingID);
        return meterReadingDto;
    }

    public static async Task<List<MeterReadingGridDto>> ListByGeographyIDAsync(QanatDbContext dbContext, int geographyID)
    {
        var meterReadings = await dbContext.vMeterReadings.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .Select(x => new MeterReadingGridDto()
            {
                GeographyID = x.GeographyID,
                GeographyDisplayName = x.GeographyDisplayName,
                WellID = x.WellID,
                WellName = x.WellName,
                MeterID = x.MeterID,
                SerialNumber = x.SerialNumber,
                MeterReadingUnitTypeID = x.MeterReadingUnitTypeID,
                MeterReadingUnitTypeDisplayName = x.MeterReadingUnitTypeDisplayName,
                ReadingDate = x.ReadingDate,
                PreviousReading = x.PreviousReading,
                CurrentReading = x.CurrentReading,
                Volume = x.Volume,
                VolumeInAcreFeet = x.VolumeInAcreFeet,
                ReaderInitials = x.ReaderInitials,
                Comment = x.Comment
            })
            .ToListAsync();

        return meterReadings;
    }

    public static async Task<List<MeterReadingDto>> ListByWellIDAsDtoAsync(QanatDbContext dbContext, int geographyID, int wellID)
    {
        var meterReadingDtos = await dbContext.MeterReadings.AsNoTracking()
            .Include(x => x.Meter)
            .Include(x => x.Well)
            .Where(x => x.GeographyID == geographyID && x.WellID == wellID)
            .Select(x => x.AsDto()).ToListAsync();

        return meterReadingDtos.OrderByDescending(x => x.ReadingDate).ToList();
    }

    public static async Task<MeterReadingDto> GetByIDAsDtoAsync(QanatDbContext dbContext, int geographyID, int wellID, int meterID, int meterReadingID)
    {
        var meterReading = await dbContext.MeterReadings.AsNoTracking()
            .Include(x => x.Meter)
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.WellID == wellID && x.MeterID == meterID && x.MeterReadingID == meterReadingID);

        return meterReading?.AsDto();
    }

    public static async Task<MeterReadingDto> GetLastReadingFromDateAsync(QanatDbContext dbContext, int geographyID, int wellID, int meterID, DateTime date)
    {
        //Find the last meter reading based on the passed in date.
        var meterReading = await dbContext.MeterReadings.AsNoTracking()
            .Include(x => x.Meter)
            .Include(x => x.Well)
            .Where(x => x.Well.GeographyID == geographyID && x.WellID == wellID && x.MeterID == meterID && x.ReadingDate.Date < date.Date)
            .OrderByDescending(x => x.ReadingDate)
            .FirstOrDefaultAsync();

        return meterReading?.AsDto();
    }

    public static async Task<MeterReadingDto> UpdateAsync(QanatDbContext dbContext, int geographyID, int wellID, int meterID, int meterReadingID, MeterReadingUpsertDto meterReadingUpsertDto)
    {
        var meterReading = dbContext.MeterReadings.Single(x => x.MeterReadingID == meterReadingID);
        var updatedMeterReading = GetMeterReadingFromUpsertDto(geographyID, meterReading.WellID, meterReading.MeterID, meterReadingUpsertDto);

        var rebuildMonthlyInterpolations = meterReading.Volume != updatedMeterReading.Volume || meterReading.ReadingDate != updatedMeterReading.ReadingDate;

        meterReading.MeterReadingUnitTypeID = updatedMeterReading.MeterReadingUnitTypeID;
        meterReading.ReadingDate = updatedMeterReading.ReadingDate;
        meterReading.PreviousReading = updatedMeterReading.PreviousReading;
        meterReading.CurrentReading = updatedMeterReading.CurrentReading;
        meterReading.Volume = updatedMeterReading.Volume;
        meterReading.VolumeInAcreFeet = updatedMeterReading.VolumeInAcreFeet;
        meterReading.ReaderInitials = updatedMeterReading.ReaderInitials;
        meterReading.Comment = updatedMeterReading.Comment;

        await dbContext.SaveChangesAsync();

        if (rebuildMonthlyInterpolations)
        {
            await MeterReadingMonthlyInterpolations.RebuildMonthlyInterpolationsAsync(dbContext, meterReading.MeterID);
        }

        var meterReadingDto = await GetByIDAsDtoAsync(dbContext, geographyID, wellID, meterID, meterReadingID);
        return meterReadingDto;
    }

    private static MeterReading GetMeterReadingFromUpsertDto(int geographyID, int wellID, int meterID, MeterReadingUpsertDto meterReadingUpsertDto)
    {
        var readingTime = DateTime.ParseExact(meterReadingUpsertDto.ReadingTime, "HH:mm", CultureInfo.InvariantCulture);
        var readingDate = meterReadingUpsertDto.ReadingDate!.Value.Date.Add(readingTime.TimeOfDay);

        var volume = meterReadingUpsertDto.CurrentReading!.Value - meterReadingUpsertDto.PreviousReading!.Value;
        var volumeInAcreFeet = meterReadingUpsertDto.MeterReadingUnitTypeID == MeterReadingUnitType.Gallons.MeterReadingUnitTypeID
            ? UnitConversionHelper.ConvertGallonsToAcreFeet(volume)
            : volume;

        var meterReading = new MeterReading()
        {
            GeographyID = geographyID,
            MeterID = meterID,
            WellID = wellID,
            MeterReadingUnitTypeID = meterReadingUpsertDto.MeterReadingUnitTypeID!.Value,
            ReadingDate = readingDate,
            PreviousReading = meterReadingUpsertDto.PreviousReading!.Value,
            CurrentReading = meterReadingUpsertDto.CurrentReading!.Value,
            Volume = volume,
            VolumeInAcreFeet = volumeInAcreFeet,
            ReaderInitials = meterReadingUpsertDto.ReaderInitials,
            Comment = meterReadingUpsertDto.Comment
        };

        return meterReading;
    }

    #region CSV

    public static async Task<MeterReadingCSVParseResult> ParseMeterReadingsFromCSVAsync(byte[] fileData)
    {
        using var memoryStream = new MemoryStream(fileData);
        using var reader = new StreamReader(memoryStream);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

        csvReader.Context.RegisterClassMap(new MeterReadingUpsertDtoCSVMap());

        var result = new MeterReadingCSVParseResult();
        try
        {
            await csvReader.ReadAsync();
            csvReader.ReadHeader();

            var headerNamesDuplicated = csvReader.HeaderRecord!.Where(x => !string.IsNullOrWhiteSpace(x)).GroupBy(x => x).Where(x => x.Count() > 1).ToList();
            if (headerNamesDuplicated.Any())
            {
                result.Errors.Add(new ErrorMessage()
                {
                    Type = "Headers",
                    Message = $"The following headers are duplicated: {string.Join(", ", headerNamesDuplicated.Select(x => x.Key))}. Please ensure all headers are unique."
                });

                return result;
            }

            var records = csvReader.GetRecords<MeterReadingCSVUpsertDto>().ToList();
            result.Records = records;
        }
        catch (MissingFieldException e)
        {
            var missingFieldMessage = e.Message.Split('.')[0];
            result.Errors.Add(new ErrorMessage()
            {
                Type = "Headers",
                Message = $"{missingFieldMessage}. Please check that the column name is not missing or misspelled."
            });

            return result;
        }
        catch (TypeConverterException e)
        {
            var typeConversionMessage = e.Message.Split('.')[0];
            result.Errors.Add(new ErrorMessage()
            {
                Type = "Type Conversion",
                Message = $"{typeConversionMessage}. Please check that the data types in the CSV match the expected types."
            });

            return result;
        }

        return result;
    }

    public static async Task<List<ErrorMessage>> ValidateCSVRecordsAsync(QanatDbContext dbContext, int geographyID, List<MeterReadingCSVUpsertDto> records)
    {
        var errors = new List<ErrorMessage>();
        var meters = await dbContext.Meters.AsNoTracking()
            .Include(x => x.WellMeters)
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        var unitTypeDisplayNames = MeterReadingUnitType.All.Select(x => x.MeterReadingUnitTypeDisplayName.ToLower());
        var unitTypeAlternateDisplayNames = MeterReadingUnitType.All
            .Select(x => x.MeterReadingUnitTypeAlternateDisplayName?.ToLower())
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList();

        var unitTypes = unitTypeDisplayNames.Union(unitTypeAlternateDisplayNames).ToList();

        var index = 0;
        foreach (var record in records)
        {
            if (string.IsNullOrEmpty(record.SerialNumber))
            {
                errors.Add(new ErrorMessage()
                {
                    Type = $"Row {index + 2}",
                    Message = "Serial Number is required."
                });
            }

            var meter = meters.SingleOrDefault(x => x.SerialNumber == record.SerialNumber);
            if (meter == null)
            {
                errors.Add(new ErrorMessage()
                {
                    Type = $"Row {index + 2}",
                    Message = $"Meter with Serial Number {record.SerialNumber} not found. Please check the serial number and try again.",
                });
            }
            else
            {
                var activeWellMeter = meter.WellMeters.SingleOrDefault(x => !x.EndDate.HasValue);
                if (activeWellMeter == null)
                {
                    errors.Add(new ErrorMessage()
                    {
                        Type = $"Row {index + 2}",
                        Message = $"Meter with Serial Number {record.SerialNumber} is not currently assigned to any well."
                    });
                }
            }

            if (!record.Date.HasValue)
            {
                errors.Add(new ErrorMessage()
                {
                    Type = $"Row {index + 2}",
                    Message = "Date is required."
                });
            }

            if (string.IsNullOrEmpty(record.Time))
            {
                errors.Add(new ErrorMessage()
                {
                    Type = $"Row {index + 2}",
                    Message = "Time is required."
                });
            }
            else
            {
                // Parse the time to ensure it is in the correct format. Support H:mm and HH:mm formats.
                var timeParsed = DateTime.TryParseExact(record.Time, ["H:mm", "HH:mm"], CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
                if (!timeParsed)
                {
                    errors.Add(new ErrorMessage()
                    {
                        Type = $"Row {index + 2}",
                        Message = "Time is not in the correct format. Please use HH:mm."
                    });
                }
            }

            if (!record.PreviousReading.HasValue)
            {
                errors.Add(new ErrorMessage()
                {
                    Type = $"Row {index + 2}",
                    Message = "Previous Reading is required."
                });
            }

            if (!record.CurrentReading.HasValue)
            {
                errors.Add(new ErrorMessage()
                {
                    Type = $"Row {index + 2}",
                    Message = "Current Reading is required."
                });
            }

            if (string.IsNullOrEmpty(record.UnitType))
            {
                errors.Add(new ErrorMessage()
                {
                    Type = $"Row {index + 2}",
                    Message = "Unit Type is required."
                });
            }
            else
            {
                var unitType = unitTypes.FirstOrDefault(x => x == record.UnitType.Trim().ToLower());
                if (unitType == null)
                {
                    errors.Add(new ErrorMessage()
                    {
                        Type = $"Row {index + 2}",
                        Message = $"Unit Type {record.UnitType} is not valid. Expected one of {string.Join(", ", unitTypes)}."
                    });
                }
            }

            var conflictingMeterReadingInRecords = records
                .Any(x => x != record && x.SerialNumber == record.SerialNumber && x.Date == record.Date && x.Time == record.Time);

            if (conflictingMeterReadingInRecords)
            {
                errors.Add(new ErrorMessage()
                {
                    Type = $"Row {index + 2}",
                    Message = record.Date.HasValue 
                        ? $"There are multiple readings uploaded for {record.SerialNumber} on {record.Date.Value:MM/dd/yyyy}." 
                        : $"There are multiple readings uploaded for {record.SerialNumber} with no date provided."
                });
            }

            var conflictingMeterReadingInDB = dbContext.MeterReadings.AsNoTracking()
                .Any(x => meter != null && x.MeterID == meter.MeterID && record.Date.HasValue && x.ReadingDate.Date == record.Date.Value.Date);

            if (conflictingMeterReadingInDB)
            {
                errors.Add(new ErrorMessage()
                {
                    Type = $"Row {index + 2}",
                    Message = $"There is already a meter reading for meter {meter?.SerialNumber} on {record.Date!.Value.Date:MM/dd/yyyy}."
                });
            }

            index++;
        }

        return errors;
    }

    public static async Task<List<MeterReadingSimpleDto>> BulkInsertAsync(QanatDbContext dbContext, int geographyID, List<MeterReadingCSVUpsertDto> records)
    {
        var serialNumbers = records.Select(x => x.SerialNumber).Distinct().ToList();
        var meters = await dbContext.Meters.AsNoTracking()
            .Include(x => x.WellMeters)
            .Where(x => x.GeographyID == geographyID && serialNumbers.Contains(x.SerialNumber))
            .ToListAsync();

        var meterReadings = new List<MeterReading>();

        foreach (var csvUpsertDto in records)
        {
            var meter = meters.Single(x => x.SerialNumber == csvUpsertDto.SerialNumber);
            var providedUnitType = csvUpsertDto.UnitType.Trim().ToLower();
            var currentWellMeter = meter.WellMeters.Single(x => !x.EndDate.HasValue);
            var unitType = MeterReadingUnitType.All.Single(x => x.MeterReadingUnitTypeDisplayName.ToLower() == providedUnitType || x.MeterReadingUnitTypeAlternateDisplayName?.ToLower() == providedUnitType);
            var date = csvUpsertDto.Date!.Value.Date;

            var time = DateTime.ParseExact(csvUpsertDto.Time, ["H:mm", "HH:mm"], CultureInfo.InvariantCulture);
            var readingDate = date.Add(time.TimeOfDay);

            var volume = csvUpsertDto.CurrentReading!.Value - csvUpsertDto.PreviousReading!.Value;
            var volumeInAcreFeet = unitType!.MeterReadingUnitTypeID == MeterReadingUnitType.Gallons.MeterReadingUnitTypeID
                ? UnitConversionHelper.ConvertGallonsToAcreFeet(volume)
                : volume;

            var meterReading = new MeterReading()
            {
                GeographyID = geographyID,
                MeterID = meter.MeterID,
                WellID = currentWellMeter.WellID,
                MeterReadingUnitTypeID = unitType.MeterReadingUnitTypeID,
                ReadingDate = readingDate,
                PreviousReading = csvUpsertDto.PreviousReading.Value,
                CurrentReading = csvUpsertDto.CurrentReading.Value,
                Volume = Math.Round(volume, 4, MidpointRounding.ToEven),
                VolumeInAcreFeet = Math.Round(volumeInAcreFeet, 4, MidpointRounding.ToEven),
                ReaderInitials = csvUpsertDto.ReaderInitials,
                Comment = csvUpsertDto.Comment
            };

            meterReadings.Add(meterReading);
        }

        await dbContext.MeterReadings.AddRangeAsync(meterReadings);
        await dbContext.SaveChangesAsync();

        foreach (var meter in meters)
        {
            await MeterReadingMonthlyInterpolations.RebuildMonthlyInterpolationsAsync(dbContext, meter.MeterID);
        }

        var result = meterReadings.Select(x => x.AsSimpleDto()).ToList();
        return result;
    }

    #endregion
}