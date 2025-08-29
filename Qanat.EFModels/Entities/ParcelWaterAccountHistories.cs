using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class ParcelWaterAccountHistories
{
    public static async Task<List<ParcelWaterAccountHistorySimpleDto>> ListAsync(QanatDbContext dbContext, int parcelID)
    {
        var parcelWaterAccountHistories = await dbContext.ParcelWaterAccountHistories.AsNoTracking()
            .Include(x => x.Parcel)
            .Include(x => x.ReportingPeriod)
            .Include(x => x.CreateUser)
            .Where(x => x.ParcelID == parcelID)
            .OrderByDescending(x => x.ReportingPeriod.EndDate).ThenByDescending(x => x.CreateDate)
            .Select(x => x.AsSimpleWithExtras())
            .ToListAsync();

        return parcelWaterAccountHistories;
    }

    public static async Task<List<ParcelWaterAccountHistorySimpleDto>> ListByWaterAccountIDAsync(QanatDbContext dbContext, int waterAccountID)
    {
        var parcelWaterAccountHistories = await dbContext.ParcelWaterAccountHistories.AsNoTracking()
            .Include(x => x.Parcel)
            .Include(x => x.ReportingPeriod)
            .Include(x => x.CreateUser)
            .Where(x => x.ToWaterAccountID == waterAccountID || x.FromWaterAccountID == waterAccountID)
            .OrderByDescending(x => x.ReportingPeriod.EndDate).ThenByDescending(x => x.CreateDate)
            .Select(x => x.AsSimpleWithExtras())
            .ToListAsync();

        return parcelWaterAccountHistories;
    }

    public static ParcelWaterAccountHistorySimpleDto AsSimpleWithExtras(this ParcelWaterAccountHistory parcelWaterAccountHistory)
    {
        var simpleDto = parcelWaterAccountHistory.AsSimpleDto();
        simpleDto.ParcelNumber = parcelWaterAccountHistory.Parcel.ParcelNumber;
        simpleDto.ReportingPeriodName = parcelWaterAccountHistory.ReportingPeriod.Name;
        simpleDto.CreateUserFullName = parcelWaterAccountHistory.CreateUser.FullName;
        return simpleDto;
    }
}
