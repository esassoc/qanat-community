CREATE TABLE [dbo].[ReportingPeriod]
(
	--Keys
	[ReportingPeriodID]							INT				NOT NULL IDENTITY(1,1),
	[GeographyID]								INT				NOT NULL,

	--Data
	[Name]										VARCHAR(255)	NOT NULL,	
	[StartDate]									DATETIME		NOT NULL,
	[EndDate]									DATETIME		NOT NULL,
	[ReadyForAccountHolders]					BIT				NOT NULL DEFAULT(0),
	[IsDefault]									BIT				NOT NULL DEFAULT(0),

	--Self Reporting
	[CoverCropSelfReportStartDate]				DATETIME		NULL,
	[CoverCropSelfReportEndDate]				DATETIME		NULL,
	[CoverCropSelfReportReadyForAccountHolders] BIT				NOT NULL DEFAULT(0),

	[FallowSelfReportStartDate]					DATETIME		NULL,	
	[FallowSelfReportEndDate]					DATETIME		NULL,
	[FallowSelfReportReadyForAccountHolders]	BIT				NOT NULL DEFAULT(0),

	--Basic Audit
    [CreateDate]								DATETIME		NOT NULL,       
    [CreateUserID]								INT				NOT NULL,   
    [UpdateDate]								DATETIME		NULL,
    [UpdateUserID]								INT				NULL,

	CONSTRAINT [PK_ReportingPeriod_ReportingPeriodID]		PRIMARY KEY ([ReportingPeriodID]),

	CONSTRAINT [FK_ReportingPeriod_Geography_GeographyID]	FOREIGN KEY ([GeographyID])			REFERENCES dbo.[Geography]([GeographyID]),
	CONSTRAINT [FK_ReportingPeriod_User_CreateUserID]		FOREIGN KEY ([CreateUserID])		REFERENCES dbo.[User]([UserID]),
	CONSTRAINT [FK_ReportingPeriod_User_UpdateUserID]		FOREIGN KEY ([UpdateUserID])		REFERENCES dbo.[User]([UserID]),

	CONSTRAINT [AK_ReportingPeroid_GeographyID_Name]		UNIQUE ([GeographyID], [Name]),
	CONSTRAINT [CK_ReportingPeriod_StartDateBeforeEndDate]	CHECK ([StartDate] < [EndDate]),
)