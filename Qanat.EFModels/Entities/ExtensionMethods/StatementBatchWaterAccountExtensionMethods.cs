using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class StatementBatchWaterAccountExtensionMethods
{
    public static StatementBatchWaterAccountDto AsDto(this StatementBatchWaterAccount statementBatchWaterAccount)
    {
        return new StatementBatchWaterAccountDto()
        {
            WaterAccountID = statementBatchWaterAccount.WaterAccountID,
            WaterAccountNumber = statementBatchWaterAccount.WaterAccount.WaterAccountNumber,
            WaterAccountName = statementBatchWaterAccount.WaterAccount.WaterAccountName,
            ContactName = statementBatchWaterAccount.WaterAccount.WaterAccountContact?.ContactName,
            FullAddress = statementBatchWaterAccount.WaterAccount.WaterAccountContact?.FullAddress,
            FileResourceGuid = statementBatchWaterAccount.FileResource?.FileResourceGUID
        };
    }
}