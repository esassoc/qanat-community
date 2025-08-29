CREATE TABLE [dbo].[StatementBatchWaterAccount]
(
	[StatementBatchWaterAccountID] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_StatementBatchWaterAccount_StatementBatchWaterAccountID] PRIMARY KEY,
    [StatementBatchID] INT NOT NULL CONSTRAINT [FK_StatementBatchWaterAccount_StatementBatch_StatementBatchID] FOREIGN KEY REFERENCES [dbo].[StatementBatch]([StatementBatchID]),
    [WaterAccountID] INT NOT NULL CONSTRAINT [FK_StatementBatchWaterAccount_WaterAccount_WaterAccountID] FOREIGN KEY REFERENCES [dbo].[WaterAccount]([WaterAccountID]),
    [FileResourceID] INT NULL CONSTRAINT [FK_StatementBatchWaterAccount_FileResource_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource]([FileResourceID])
)
