using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class Meters
{
    public static async Task<List<ErrorMessage>> ValidateMeterUpsertAsync(QanatDbContext dbContext, int geographyID, MeterGridDto meterGridDto, int? meterID = null)
    {
        var errors = new List<ErrorMessage>();

        var existingSerialNumber = await dbContext.Meters.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.SerialNumber == meterGridDto.SerialNumber && x.MeterID != meterID);

        if (existingSerialNumber != null)
        {
            errors.Add(new ErrorMessage() { Type = "Serial Number", Message = $"There is already a meter with the Serial Number ${meterGridDto.SerialNumber}." });
        }

        return errors;
    }

    public static MeterGridDto AddMeter(QanatDbContext dbContext, MeterGridDto meterGridDto)
    {
        var meter = new Meter()
        {
            GeographyID = meterGridDto.GeographyID,
            MeterStatusID = meterGridDto.MeterStatusID,
            SerialNumber = meterGridDto.SerialNumber,
            DeviceName = meterGridDto.DeviceName,
            Make = meterGridDto.Make,
            ModelNumber = meterGridDto.ModelNumber
        };

        dbContext.Meters.Add(meter);
        dbContext.SaveChanges();
        dbContext.Entry(meter).Reload();

        return GetByIDAsGridDto(dbContext, meter.MeterID);
    }

    public static List<MeterGridDto> GetGeographyIDAsGridDto(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.Meters
            .Include(x => x.WellMeters)
            .Where(x => x.GeographyID == geographyID)
            .Select(x => x.AsGridDto())
            .ToList();
    }
    public static List<MeterLinkDisplayDto> ListUnassignedAsLinkDisplayDtos(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.Meters.AsNoTracking()
            .Include(x => x.WellMeters)
            .Where(x => x.GeographyID == geographyID && x.WellMeters.All(x => x.EndDate.HasValue))
            .Select(x => x.AsLinkDisplayDto())
            .ToList();
    }

    public static MeterGridDto GetByIDAsGridDto(QanatDbContext dbContext, int meterID)
    {
        return dbContext.Meters.Single(x => x.MeterID == meterID).AsGridDto();
    }

    public static async Task<WellMeterDto> GetCurrentWellMeterByWellIDAsDtoAsync(QanatDbContext dbContext, int wellID)
    {
        var wellMeter = await dbContext.WellMeters.AsNoTracking()
            .Include(x => x.Well).ThenInclude(x => x.Parcel).ThenInclude(x => x.WaterAccount)
            .Include(x => x.Meter)
            .SingleOrDefaultAsync(x => x.WellID == wellID && !x.EndDate.HasValue);

        return wellMeter?.AsDto();
    }

    public static MeterGridDto UpdateMeter(QanatDbContext dbContext, int meterID, MeterGridDto meterGridDto)
    {
        var meter = dbContext.Meters.Single(x => x.MeterID == meterID);

        meter.MeterStatusID = meterGridDto.MeterStatusID;
        meter.SerialNumber = meterGridDto.SerialNumber;
        meter.DeviceName = meterGridDto.DeviceName;
        meter.Make = meterGridDto.Make;
        meter.ModelNumber = meterGridDto.ModelNumber;

        dbContext.SaveChanges();
        dbContext.Entry(meter).Reload();

        return GetByIDAsGridDto(dbContext, meterID);
    }
}