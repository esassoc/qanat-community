using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class StatementBatches
{
    public static List<ErrorMessage> ValidateStatementBatch(QanatDbContext dbContext, int geographyID, StatementBatchUpsertDto statementBatchUpsertDto)
    {
        var errors = new List<ErrorMessage>();

        var conflictingGeographyName = dbContext.StatementBatches.SingleOrDefault(x =>
            x.GeographyID == geographyID && x.StatementBatchName ==
            statementBatchUpsertDto.StatementBatchName);

        if (conflictingGeographyName != null)
        {
            errors.Add(new ErrorMessage() { Type = "Name", Message = "A batch with this name already exists within this geography." });
        }

        if (statementBatchUpsertDto.WaterAccountIDs == null || !statementBatchUpsertDto.WaterAccountIDs.Any())
        {
            errors.Add(new ErrorMessage() { Type = "Water Accounts", Message = "Please select the water accounts to include in the Usage Statement batch." });
        }

        return errors;
    }

    public static async Task<StatementBatchDto> Create(QanatDbContext dbContext, int geographyID, int currentUserID, StatementBatchUpsertDto statementBatchUpsertDto)
    {
        var newStatementBatch = new StatementBatch()
        {
            GeographyID = geographyID,
            StatementBatchName = statementBatchUpsertDto.StatementBatchName,
            StatementTemplateID = statementBatchUpsertDto.StatementTemplateID.Value,
            ReportingPeriodID = statementBatchUpsertDto.ReportingPeriodID.Value,
            LastUpdated = DateTime.UtcNow,
            UpdateUserID = currentUserID,
            StatementsGenerated = false
        };

        await dbContext.StatementBatches.AddAsync(newStatementBatch);
        await dbContext.SaveChangesAsync();
        await dbContext.Entry(newStatementBatch).ReloadAsync();

        var newStatementBatchWaterAccounts = dbContext.WaterAccounts.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && statementBatchUpsertDto.WaterAccountIDs.Contains(x.WaterAccountID))
            .Select(x => new StatementBatchWaterAccount()
            {
                WaterAccountID = x.WaterAccountID,
                StatementBatchID = newStatementBatch.StatementBatchID,
            });

        await dbContext.StatementBatchWaterAccounts.AddRangeAsync(newStatementBatchWaterAccounts);
        await dbContext.SaveChangesAsync();

        var statementBatchDto = await GetByIDAsDto(dbContext, newStatementBatch.StatementBatchID);
        return statementBatchDto;
    }

    public static async Task<List<StatementBatchDto>> ListByGeographyIDAsDto(QanatDbContext dbContext, int geographyID)
    {
        var statementBatchDtos = await dbContext.StatementBatches.AsNoTracking()
            .Include(x => x.ReportingPeriod)
            .Include(x => x.StatementTemplate)
            .Include(x => x.UpdateUser)
            .Include(x => x.StatementBatchWaterAccounts)
            .Where(x => x.GeographyID == geographyID)
            .Select(x => x.AsDto()).ToListAsync();

        return statementBatchDtos;
    }

    public static async Task<StatementBatchDto> GetByIDAsDto(QanatDbContext dbContext, int statementBatchID)
    {
        var statementBatch = await dbContext.StatementBatches.AsNoTracking()
            .Include(x => x.ReportingPeriod)
            .Include(x => x.StatementTemplate)
            .Include(x => x.UpdateUser)
            .Include(x => x.StatementBatchWaterAccounts)
            .SingleOrDefaultAsync(x => x.StatementBatchID == statementBatchID);

        return statementBatch?.AsDto();
    }

    public static async Task DeleteByID(QanatDbContext dbContext, int statementBatchID)
    {
        // linked FileResources and blob storage files should be deleted before this function is called
        var statementBatchWaterAccountToRemove = await dbContext.StatementBatchWaterAccounts
            .Where(x => x.StatementBatchID == statementBatchID).ToListAsync();

        dbContext.RemoveRange(statementBatchWaterAccountToRemove);

        var statementBatchToDelete = dbContext.StatementBatches
            .Single(x => x.StatementBatchID == statementBatchID);

        dbContext.StatementBatches.Remove(statementBatchToDelete);
        await dbContext.SaveChangesAsync();
    }
}