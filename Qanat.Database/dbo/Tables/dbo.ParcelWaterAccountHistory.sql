CREATE TABLE [dbo].[ParcelWaterAccountHistory]
(
	[ParcelWaterAccountHistoryID]	INT				NOT NULL IDENTITY(1,1),
	[GeographyID]					INT				NOT NULL,
	[ParcelID]						INT				NOT NULL,	
	[ReportingPeriodID]				INT				NOT NULL,	

	--MK 2/21/2025: These can't be foreign keys because we want to keep the history even if the water account is deleted.
	[FromWaterAccountID]			INT				NULL,
	[FromWaterAccountNumber]		INT				NULL,
	[FromWaterAccountName]			VARCHAR(255)	NULL,

	[ToWaterAccountID]				INT				NULL,
	[ToWaterAccountNumber]			INT				NULL,
	[ToWaterAccountName]			VARCHAR(255)	NULL,

	[Reason]						VARCHAR(1000)	NULL,
	[CreateUserID]					INT				NOT NULL,
	[CreateDate]					DATETIME		NOT NULL DEFAULT(GETUTCDATE()),

	CONSTRAINT [PK_ParcelWaterAccountHistory_WaterAccountParcelID]					PRIMARY KEY([ParcelWaterAccountHistoryID]),
	
	CONSTRAINT [FK_ParcelWaterAccountHistory_Geography_GeographyID]					FOREIGN KEY([GeographyID])			REFERENCES [dbo].[Geography] ([GeographyID]),
	CONSTRAINT [FK_ParcelWaterAccountHistory_Parcel_ParcelID]						FOREIGN KEY([ParcelID])				REFERENCES [dbo].[Parcel] ([ParcelID]),
	CONSTRAINT [FK_ParcelWaterAccountHistory_ReportingPeriod_ReportingPeriodID]		FOREIGN KEY([ReportingPeriodID])	REFERENCES [dbo].[ReportingPeriod] ([ReportingPeriodID]),
	CONSTRAINT [FK_ParcelWaterAccountHistory_User_CreateUserID]						FOREIGN KEY([CreateUserID])			REFERENCES [dbo].[User] ([UserID])
)
GO;

CREATE INDEX IX_ParcelWaterAccountHistory_GeographyID on dbo.ParcelWaterAccountHistory(GeographyID);
GO

CREATE INDEX IX_ParcelWaterAccountHistory_ParcelID on dbo.ParcelWaterAccountHistory(ParcelID);
GO

CREATE INDEX IX_ParcelWaterAccountHistory_ReportingPeriodID on dbo.ParcelWaterAccountHistory(ReportingPeriodID);
GO

CREATE INDEX IX_ParcelWaterAccountHistory_CreateUserID on dbo.ParcelWaterAccountHistory(CreateUserID);
GO