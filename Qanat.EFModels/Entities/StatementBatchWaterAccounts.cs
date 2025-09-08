using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class StatementBatchWaterAccounts
{
    public static async Task<List<StatementBatchWaterAccountDto>> ListAsDto(QanatDbContext dbContext, int statementBatchID)
    {
        var statementBatchWaterAccounts = await dbContext.StatementBatchWaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountContact)
            .Include(x => x.FileResource)
            .Where(x => x.StatementBatchID == statementBatchID)
            .Select(x => x.AsDto()).ToListAsync();

        return statementBatchWaterAccounts;
    }

    public static async Task<StatementBatchWaterAccountDto> GetByIDAsDto(QanatDbContext dbContext, int statementBatchWaterAccountID)
    {
        var statementBatchWaterAccount = await dbContext.StatementBatchWaterAccounts.AsNoTracking()
            .Include(x => x.WaterAccount).ThenInclude(x => x.WaterAccountContact)
            .Include(x => x.FileResource)
            .SingleOrDefaultAsync(x => x.StatementBatchWaterAccountID == statementBatchWaterAccountID);

        return statementBatchWaterAccount?.AsDto();
    }
}