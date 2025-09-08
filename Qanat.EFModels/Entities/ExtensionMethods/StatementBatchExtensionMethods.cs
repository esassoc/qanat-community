using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class StatementBatchExtensionMethods
{
    public static StatementBatchDto AsDto(this StatementBatch statementBatch)
    {
        return new StatementBatchDto()
        {
            StatementBatchID = statementBatch.StatementBatchID,
            StatementBatchName = statementBatch.StatementBatchName,
            StatementTemplateName = statementBatch.StatementTemplate.TemplateTitle,
            ReportingPeriodYear = statementBatch.ReportingPeriod.EndDate.Year,
            LastUpdated = statementBatch.LastUpdated,
            UpdateUserFullName = statementBatch.UpdateUser.FullName,
            NumberOfWaterAccounts = statementBatch.StatementBatchWaterAccounts.Count,
            StatementsGenerated = statementBatch.StatementsGenerated
        };
    }
}