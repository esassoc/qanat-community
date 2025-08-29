using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public class WellMeters
{
    public static List<ErrorMessage> ValidateAddWellMeter(QanatDbContext dbContext, AddWellMeterRequestDto requestDto)
    {
        var errorMessages = new List<ErrorMessage>();

        var existingWellMeterForWell = dbContext.WellMeters.AsNoTracking().SingleOrDefault(x => x.WellID == requestDto.WellID && !x.EndDate.HasValue);
        if (existingWellMeterForWell != null)
        {
            errorMessages.Add(new ErrorMessage() { Type = "Well", Message = "The selected well is already assigned a meter."});
        }

        var existingWellMeterForMeter = dbContext.WellMeters.AsNoTracking().SingleOrDefault(x => x.MeterID == requestDto.MeterID && !x.EndDate.HasValue);
        if (existingWellMeterForMeter != null)
        {
            errorMessages.Add(new ErrorMessage() { Type = "Meter", Message = "The selected meter is already assigned to a well." });
        }

        return errorMessages;
    }

    public static void AddWellMeter(QanatDbContext dbContext, AddWellMeterRequestDto requestDto)
    {
        var wellMeter = new WellMeter()
        {
            WellID = requestDto.WellID,
            MeterID = requestDto.MeterID,
            StartDate = requestDto.StartDate
        };

        dbContext.WellMeters.Add(wellMeter);
        dbContext.SaveChanges();
    }

    public static List<ErrorMessage> ValidateRemoveWellMeter(WellMeter wellMeter, RemoveWellMeterRequestDto requestDto)
    {
        var errorMessages = new List<ErrorMessage>();

        if (wellMeter == null)
        {
            errorMessages.Add(new ErrorMessage() { Type= "Well Meter", Message = "The selected meter was not found assigned to the selected well."});
        }
        else if (wellMeter.StartDate >= requestDto.EndDate)
        {
            errorMessages.Add(new ErrorMessage() { Type = "End Date", Message = $"The end date must be later than the start date of {wellMeter.StartDate.ToShortDateString()}." });
        }

        return errorMessages;
    }

    public static void RemoveWellMeter(QanatDbContext dbContext, WellMeter wellMeter, RemoveWellMeterRequestDto requestDto)
    {
        wellMeter.EndDate = requestDto.EndDate;
        dbContext.SaveChanges();
    }
}