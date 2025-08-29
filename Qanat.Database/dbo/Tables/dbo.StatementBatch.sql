CREATE TABLE [dbo].[StatementBatch]
(
	[StatementBatchID] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_StatementBatch_StatementBatchID] PRIMARY KEY,
	[GeographyID] INT NOT NULL CONSTRAINT [FK_StatementBatch_Geography_GeographyID] FOREIGN KEY REFERENCES [dbo].[Geography]([GeographyID]),
	[StatementBatchName] VARCHAR(100) NOT NULL,
	[StatementTemplateID] INT NOT NULL CONSTRAINT [FK_StatementBatch_StatementTemplate_StatementTemplateID] FOREIGN KEY REFERENCES [dbo].[StatementTemplate]([StatementTemplateID]),
    [ReportingPeriodID] INT NOT NULL CONSTRAINT [FK_StatementBatch_ReportingPeriod_ReportingPeriodID] FOREIGN KEY REFERENCES [dbo].[ReportingPeriod]([ReportingPeriodID]),
	[LastUpdated] DATETIME NOT NULL,
	[UpdateUserID] INT NOT NULL CONSTRAINT [FK_StatementBatch_User_UpdateUserID_UserID] FOREIGN KEY REFERENCES [dbo].[User]([UserID]),
	[StatementsGenerated] BIT NOT NULL,
	CONSTRAINT [AK_StatementBatch_GeographyID_StatementBatchName] UNIQUE ([GeographyID], [StatementBatchName])
)