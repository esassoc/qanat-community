CREATE TABLE [dbo].[UsageLocation]
(
	[UsageLocationID]				INT				NOT NULL IDENTITY(1,1),
	[GeographyID]					INT				NOT NULL, 
	[UsageLocationTypeID]			INT				NULL, --MK 6/17/2025 TODO: Make this NOT NULL after a production release.
 	[ParcelID]						INT				NOT NULL,
	[ReportingPeriodID]				INT				NOT NULL, 

	[Name]							VARCHAR(100)	NOT NULL,
	[Area]							FLOAT			NOT NULL, 

	[CoverCropNote]					VARCHAR(MAX)	NULL,
	[FallowNote]					VARCHAR(MAX)	NULL,
															 
    [CreateDate]					DATETIME		NOT NULL DEFAULT(GETUTCDATE()),       
    [CreateUserID]					INT				NOT NULL DEFAULT(2), --2 == John Burns
    [UpdateDate]					DATETIME		NULL,
    [UpdateUserID]					INT				NULL,

	CONSTRAINT [PK_UsageLocation_UsageLocationID]									PRIMARY KEY ([UsageLocationID]),

	CONSTRAINT [FK_UsageLocation_Geography_GeographyID]								FOREIGN KEY ([GeographyID])					REFERENCES dbo.[Geography]([GeographyID]),
	CONSTRAINT [FK_UsageLocation_UsageLocationType_UsageLocationTypeID]				FOREIGN KEY ([UsageLocationTypeID])			REFERENCES dbo.[UsageLocationType]([UsageLocationTypeID]),
	CONSTRAINT [FK_UsageLocation_Parcel_ParcelID]									FOREIGN KEY ([ParcelID])					REFERENCES dbo.[Parcel]([ParcelID]),
	CONSTRAINT [FK_UsageLocation_ReportingPeriod_ReportingPeriodID]					FOREIGN KEY ([ReportingPeriodID])			REFERENCES dbo.[ReportingPeriod]([ReportingPeriodID]),
	CONSTRAINT [FK_UsageLocation_User_CreateUserID]									FOREIGN KEY ([CreateUserID])				REFERENCES dbo.[User]([UserID]),
	CONSTRAINT [FK_UsageLocation_User_UpdateUserID]									FOREIGN KEY ([UpdateUserID])				REFERENCES dbo.[User]([UserID]),

	CONSTRAINT [AK_UsageLocation_GeographyID_ReportingPeriodID_Name] UNIQUE ([GeographyID], [ReportingPeriodID], [Name]),
)
GO

CREATE INDEX IX_UsageLocation_GeographyID_ParcelID ON dbo.UsageLocation(GeographyID, ParcelID);
GO

CREATE INDEX IX_UsageLocation_UsageLocationTypeID ON dbo.UsageLocation(UsageLocationTypeID);
GO

CREATE INDEX IX_UsageLocation_ReportingPeriodID ON dbo.UsageLocation(ReportingPeriodID);
GO

CREATE INDEX IX_UsageLocation_CreateUserID ON dbo.UsageLocation(CreateUserID);
GO

CREATE INDEX IX_UsageLocation_UpdateUserID ON dbo.UsageLocation(UpdateUserID);
GO