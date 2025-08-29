using Microsoft.EntityFrameworkCore;
using Qanat.EFModels.Entities.ExtensionMethods;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class UsageLocationParcelHistories
{
    public static async Task<List<UsageLocationParcelHistoryDto>> ListByParcelIDAsync(QanatDbContext dbContext, int geographyID, int parcelID)
    {
        var fromHistories = await dbContext.UsageLocationParcelHistories.AsNoTracking()
            .Include(x => x.UsageLocation)
            .Include(x => x.ReportingPeriod)
            .Include(x => x.CreateUser)
            .Where(x => x.GeographyID == geographyID && x.FromParcelID == parcelID)
            .Select(x => x.AsDto())
            .ToListAsync();

        var toHistories = await dbContext.UsageLocationParcelHistories.AsNoTracking()
            .Include(x => x.UsageLocation)
            .Include(x => x.ReportingPeriod)
            .Include(x => x.CreateUser)
            .Where(x => x.GeographyID == geographyID && x.ToParcelID == parcelID)
            .Select(x => x.AsDto())
            .ToListAsync();

        var allHistories = fromHistories.Concat(toHistories)
            .OrderBy(x => x.UsageLocationName).ThenByDescending(x => x.CreateDate)
            .ToList();

        return allHistories;
    }
}