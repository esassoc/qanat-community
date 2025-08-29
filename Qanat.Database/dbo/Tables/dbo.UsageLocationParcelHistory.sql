CREATE TABLE [dbo].[UsageLocationParcelHistory]
(
	[UsageLocationParcelHistoryID]	INT				NOT NULL IDENTITY(1,1),
	[GeographyID]					INT				NOT NULL,
	[UsageLocationID]				INT				NOT NULL,	
	[ReportingPeriodID]				INT				NOT NULL,	

	--MK 2/21/2025: These can't be foreign keys because we want to keep the history even if the parcel is deleted.
	[FromParcelID]					INT				NULL,
	[FromParcelNumber]				VARCHAR(64)		NULL,

	[ToParcelID]					INT				NULL,
	[ToParcelNumber]				VARCHAR(64)		NULL,

	[Reason]						VARCHAR(1000)	NULL,
	[CreateUserID]					INT				NOT NULL,
	[CreateDate]					DATETIME		NOT NULL DEFAULT(GETUTCDATE()),

	CONSTRAINT [PK_UsageLocationParcelHistory_UsageLocationParcelHistoryID]			PRIMARY KEY([UsageLocationParcelHistoryID]),
	
	CONSTRAINT [FK_UsageLocationParcelHistory_Geography_GeographyID]				FOREIGN KEY([GeographyID])			REFERENCES [dbo].[Geography] ([GeographyID]),
	CONSTRAINT [FK_UsageLocationParcelHistory_UsageLocation_UsageLocationID]		FOREIGN KEY([UsageLocationID])		REFERENCES [dbo].[UsageLocation] ([UsageLocationID]),
	CONSTRAINT [FK_UsageLocationParcelHistory_ReportingPeriod_ReportingPeriodID]	FOREIGN KEY([ReportingPeriodID])	REFERENCES [dbo].[ReportingPeriod] ([ReportingPeriodID]),
	CONSTRAINT [FK_UsageLocationParcelHistory_User_CreateUserID]					FOREIGN KEY([CreateUserID])			REFERENCES [dbo].[User] ([UserID])
)
GO;

CREATE INDEX IX_UsageLocationParcelHistory_GeographyID on dbo.[UsageLocationParcelHistory]([GeographyID]);
GO

CREATE INDEX IX_UsageLocationParcelHistory_UsageLocationID on dbo.[UsageLocationParcelHistory]([UsageLocationID]);
GO

CREATE INDEX IX_ParcelWaterAccountHistory_ReportingPeriodID on dbo.[UsageLocationParcelHistory]([ReportingPeriodID]);
GO

CREATE INDEX IX_ParcelWaterAccountHistory_FromParcelID on dbo.[UsageLocationParcelHistory]([FromParcelID]);
GO

CREATE INDEX IX_ParcelWaterAccountHistory_ToParcelID on dbo.[UsageLocationParcelHistory]([ToParcelID]);
GO

CREATE INDEX IX_ParcelWaterAccountHistory_CreateUserID on dbo.[UsageLocationParcelHistory]([CreateUserID]);
GO
