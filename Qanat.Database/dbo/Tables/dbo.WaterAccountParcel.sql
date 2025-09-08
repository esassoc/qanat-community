CREATE TABLE [dbo].[WaterAccountParcel]
(
	[WaterAccountParcelID]	INT NOT NULL IDENTITY(1,1),
	[GeographyID]			INT NOT NULL,
	[WaterAccountID]		INT NOT NULL,
	[ParcelID]				INT NOT NULL,
	[ReportingPeriodID]		INT NOT NULL,

	CONSTRAINT [PK_WaterAccountParcel_WaterAccountParcelID]						PRIMARY KEY([WaterAccountParcelID]),

	CONSTRAINT [FK_WaterAccountParcel_Geography_GeographyID]					FOREIGN KEY([GeographyID])			REFERENCES [dbo].[Geography]([GeographyID]),
	CONSTRAINT [FK_WaterAccountParcel_WaterAccount_WaterAccountID]				FOREIGN KEY([WaterAccountID])		REFERENCES [dbo].[WaterAccount] ([WaterAccountID]), 
	CONSTRAINT [FK_WaterAccountParcel_Parcel_ParcelID]							FOREIGN KEY([ParcelID])				REFERENCES [dbo].[Parcel] ([ParcelID]),
	CONSTRAINT [FK_WaterAccountParcel_ReportingPeriod_ReportingPeriodID]		FOREIGN KEY([ReportingPeriodID])	REFERENCES [dbo].[ReportingPeriod] ([ReportingPeriodID]),

	CONSTRAINT [AK_WaterAccountParcel_GeographyID_ParcelID_ReportingPeriodID]	UNIQUE ([GeographyID], [ParcelID], [ReportingPeriodID])
)
GO;

CREATE INDEX IX_WaterAccountParcel_WaterAccountID on dbo.WaterAccountParcel(WaterAccountID);
GO

CREATE INDEX IX_WaterAccountParcel_ParcelID on dbo.WaterAccountParcel(ParcelID);
GO

CREATE INDEX IX_WaterAccountParcel_GeographyID on dbo.WaterAccountParcel(GeographyID);
GO

CREATE INDEX IX_WaterAccountParcel_ReportingPeriodID on dbo.WaterAccountParcel(ReportingPeriodID);
GO