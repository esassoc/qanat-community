using Microsoft.EntityFrameworkCore;
using Qanat.EFModels.Entities.ExtensionMethods;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public class UsageLocationHistories
{
    public static async Task<List<UsageLocationHistoryDto>> ListByGeographyIDAndWaterAccountIDAsync(QanatDbContext dbContext, int geographyID, int waterAccountID)
    {
        var usageLocationHistories = await dbContext.UsageLocationHistories.AsNoTracking()
            .Include(x => x.UsageLocation)
            .Include(x => x.UsageLocation).ThenInclude(x => x.ReportingPeriod)
            .Include(x => x.UsageLocation).ThenInclude(x => x.Parcel).ThenInclude(x => x.WaterAccountParcels)
            .Include(x => x.UsageLocationType)
            .Include(x => x.CreateUser)
            .Where(x => x.GeographyID == geographyID && x.UsageLocation.Parcel.WaterAccountParcels.Any(z => z.WaterAccountID == waterAccountID))
            .ToListAsync();

        var usageLocationHistoryDtos = usageLocationHistories.Select(x => x.AsDto()).ToList();
        return usageLocationHistoryDtos;
    }


    public static async Task<List<UsageLocationHistoryDto>> ListByGeographyIDAndParcelIDAsync(QanatDbContext dbContext, int geographyID, int parcelID)
    {
        var usageLocationHistories = await dbContext.UsageLocationHistories.AsNoTracking()
            .Include(x => x.UsageLocation)
            .Include(x => x.UsageLocation).ThenInclude(x => x.ReportingPeriod)
            .Include(x => x.UsageLocationType)
            .Include(x => x.CreateUser)
            .Where(x => x.GeographyID == geographyID && x.UsageLocation.ParcelID == parcelID)
            .ToListAsync();

        var usageLocationHistoryDtos = usageLocationHistories.Select(x => x.AsDto()).ToList();
        return usageLocationHistoryDtos;
    }
}