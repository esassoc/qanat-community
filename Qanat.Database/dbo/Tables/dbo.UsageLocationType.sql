CREATE TABLE [dbo].[UsageLocationType]
(
	[UsageLocationTypeID]			INT				NOT NULL IDENTITY(1,1),
	[GeographyID]					INT				NOT NULL, 
	[WaterMeasurementTypeID]		INT				NULL,

	[Name]							VARCHAR(100)	NOT NULL,
	[Definition]					VARCHAR(500)	NULL,
	[CanBeRemoteSensed]				BIT				NOT NULL,
	[IsIncludedInUsageCalculation]	BIT				NOT NULL,
	[IsDefault]						BIT				NOT NULL,
	[ColorHex]						VARCHAR(7)		NULL,
	[SortOrder]						INT				NOT NULL,

	[CanBeSelectedInCoverCropForm]	BIT				NOT NULL DEFAULT(0),
	[CountsAsCoverCropped]			BIT				NOT NULL DEFAULT(0),

	[CanBeSelectedInFallowForm]		BIT				NOT NULL DEFAULT(0),
	[CountsAsFallowed]				BIT				NOT NULL DEFAULT(0),
															 
    [CreateDate]					DATETIME		NOT NULL,       
    [CreateUserID]					INT				NOT NULL,
    [UpdateDate]					DATETIME		NULL,
    [UpdateUserID]					INT				NULL,

	CONSTRAINT [PK_UsageLocationType_UsageLocationTypeID]		PRIMARY KEY ([UsageLocationTypeID]),

	CONSTRAINT [FK_UsageLocationType_Geography_GeographyID]							FOREIGN KEY ([GeographyID])				REFERENCES dbo.[Geography]([GeographyID]),
	CONSTRAINT [FK_UsageLocationType_WaterMeasurementType_WaterMeasurementTypeID]	FOREIGN KEY ([WaterMeasurementTypeID])	REFERENCES dbo.[WaterMeasurementType]([WaterMeasurementTypeID]),
	CONSTRAINT [FK_UsageLocationType_User_CreateUserID]								FOREIGN KEY ([CreateUserID])			REFERENCES dbo.[User]([UserID]),
	CONSTRAINT [FK_UsageLocationType_User_UpdateUserID]								FOREIGN KEY ([UpdateUserID])			REFERENCES dbo.[User]([UserID]),

	CONSTRAINT [AK_UsageLocationType_GeographyID_Name]			UNIQUE ([GeographyID], [Name]),
)
GO;