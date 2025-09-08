CREATE TABLE [dbo].[WaterAccountFallowStatus]
(
	[WaterAccountFallowStatusID]	INT			NOT NULL IDENTITY(1, 1),
	[GeographyID]					INT			NOT NULL, 
	[WaterAccountID]				INT			NOT NULL,
	[ReportingPeriodID]				INT			NOT NULL, 
	[SelfReportStatusID]			INT			NOT NULL DEFAULT(1),

    [SubmittedDate]					DATETIME    NULL,
	[SubmittedByUserID]				INT         NULL,
    [ApprovedDate]					DATETIME    NULL,
	[ApprovedByUserID]				INT         NULL,
    [ReturnedDate]					DATETIME    NULL,
	[ReturnedByUserID]				INT         NULL,

    [CreateDate]					DATETIME    NOT NULL,       
    [CreateUserID]					INT         NOT NULL,   
    [UpdateDate]					DATETIME    NULL,
    [UpdateUserID]					INT         NULL,

	CONSTRAINT [PK_WaterAccountFallowStatus_WaterAccountFallowStatusID]				PRIMARY KEY ([WaterAccountFallowStatusID]),

	CONSTRAINT [FK_WaterAccountFallowStatus_Geography_GeographyID]					FOREIGN KEY ([GeographyID])			REFERENCES dbo.[Geography]([GeographyID]),
	CONSTRAINT [FK_WaterAccountFallowStatus_WaterAccount_WaterAccountID]			FOREIGN KEY ([WaterAccountID])		REFERENCES dbo.[WaterAccount]([WaterAccountID]),
	CONSTRAINT [FK_WaterAccountFallowStatus_ReportingPeriod_ReportingPeriodID]		FOREIGN KEY ([ReportingPeriodID])	REFERENCES dbo.[ReportingPeriod]([ReportingPeriodID]),
	CONSTRAINT [FK_WaterAccountFallowStatus_SelfReportStatus_SelReportStatusID]		FOREIGN KEY ([SelfReportStatusID])	REFERENCES dbo.[SelfReportStatus]([SelfReportStatusID]),
	CONSTRAINT [FK_WaterAccountFallowStatus_User_SubmittedByUserID]					FOREIGN KEY ([SubmittedByUserID])	REFERENCES dbo.[User]([UserID]),
	CONSTRAINT [FK_WaterAccountFallowStatus_User_ApprovedByUserID]					FOREIGN KEY ([ApprovedByUserID])	REFERENCES dbo.[User]([UserID]),
	CONSTRAINT [FK_WaterAccountFallowStatus_User_ReturnedByUserID]					FOREIGN KEY ([ReturnedByUserID])	REFERENCES dbo.[User]([UserID]),
	CONSTRAINT [FK_WaterAccountFallowStatus_User_CreateUserID]						FOREIGN KEY ([CreateUserID])		REFERENCES dbo.[User]([UserID]),
	CONSTRAINT [FK_WaterAccountFallowStatus_User_UpdateUserID]						FOREIGN KEY ([UpdateUserID])		REFERENCES dbo.[User]([UserID]),

	CONSTRAINT [AK_WaterAccountFallowStatus_GeographyID_WaterAccountID_ReportingPeriodID] UNIQUE ([GeographyID], [WaterAccountID], [ReportingPeriodID]),
)
GO