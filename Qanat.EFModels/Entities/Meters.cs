using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public class Meters
{
    public static List<MeterGridDto> GetGeographyIDAsGridDto(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.Meters
            .Include(x => x.WellMeters)
            .Where(x => x.GeographyID == geographyID)
            .Select(x => x.AsGridDto())
            .ToList();
    }

    public static MeterGridDto GetByIDAsGridDto(QanatDbContext dbContext, int meterID)
    {
        return dbContext.Meters.Single(x => x.MeterID == meterID).AsGridDto();
    }

    public static List<MeterLinkDisplayDto> ListAsLinkDisplayDtos(QanatDbContext dbContext, int geographyID)
    {
        return dbContext.Meters.AsNoTracking()
            .Include(x => x.WellMeters)
            .Where(x => x.GeographyID == geographyID)
            .Select(x => x.AsLinkDisplayDto())
            .ToList();
    }

    public static MeterGridDto AddMeter(QanatDbContext dbContext, MeterGridDto meterGridDto)
    {
        var meter = new Meter()
        {
            SerialNumber = meterGridDto.SerialNumber,
            DeviceName = meterGridDto.DeviceName,
            Make = meterGridDto.Make,
            GeographyID = meterGridDto.GeographyID,
            MeterStatusID = meterGridDto.MeterStatusID,
            ModelNumber = meterGridDto.ModelNumber
        };
        dbContext.Meters.Add(meter);
        dbContext.SaveChanges();
        dbContext.Entry(meter).Reload();
        return GetByIDAsGridDto(dbContext, meter.MeterID);
    }

    public static MeterGridDto UpdateMeter(QanatDbContext dbContext, int meterID, MeterGridDto meterGridDto)
    {
        var meter = dbContext.Meters.SingleOrDefault(x => x.MeterID == meterID);
        meter.SerialNumber = meterGridDto.SerialNumber;
        meter.DeviceName = meterGridDto.DeviceName;
        meter.Make = meterGridDto.Make;
        meter.MeterStatusID = meterGridDto.MeterStatusID;
        meter.ModelNumber = meterGridDto.ModelNumber;

        dbContext.SaveChanges();
        dbContext.Entry(meter).Reload();
        return GetByIDAsGridDto(dbContext, meterID);
    }
}