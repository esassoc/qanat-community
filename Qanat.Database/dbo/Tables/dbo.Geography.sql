CREATE TABLE [dbo].[Geography]
(
    -- Keys (uncategorized)
	[GeographyID]												INT				NOT NULL,
	[GeographyConfigurationID]									INT				NOT NULL,
	
	-- Basic/Misc Data
	[GeographyName]												VARCHAR(50)		NOT NULL,
	[GeographyDisplayName]										VARCHAR(100)	NOT NULL,
	[GeographyDescription]										VARCHAR(500)	NULL,
	[APNRegexPattern]											VARCHAR(100)	NOT NULL,
	[APNRegexPatternDisplay]									VARCHAR(50)		NOT NULL,
	[GSACanonicalID]											INT				NULL,
	[Color]														VARCHAR(9)		NULL,
	[ContactEmail]												VARCHAR(100)	NULL,
	[ContactPhoneNumber]										VARCHAR (30)	NULL,
	[ContactAddressLine1]										VARCHAR (500)	NULL,
	[ContactAddressLine2]										VARCHAR (500)	NULL,
	[LandownerDashboardSupplyLabel]								VARCHAR(200)	NOT NULL DEFAULT('Supply'),
	[LandownerDashboardUsageLabel]								VARCHAR(200)	NOT NULL DEFAULT('Usage'),

	-- Spatial Data
	[CoordinateSystem]											INT				NOT NULL,
	[AreaToAcresConversionFactor]								INT				NOT NULL, 

	-- Open ET Configuration 
	[IsOpenETActive]											BIT				NOT NULL DEFAULT(0),
	[OpenETShapeFilePath]										VARCHAR(100)	NULL,
	[OpenETRasterTimeseriesMultipolygonColumnToUseAsIdentifier]	VARCHAR(50)		NULL,

	-- Source of Record
	[SourceOfRecordWaterMeasurementTypeID]						INT				NULL,
	[SourceOfRecordExplanation]									VARCHAR(500)	NULL,

	-- Water Budget
	[ShowSupplyOnWaterBudgetComponent]							BIT				NOT NULL DEFAULT(1),
	[WaterBudgetSlotAHeader]									VARCHAR(255)	NULL,
	[WaterBudgetSlotAWaterMeasurementTypeID]					INT				NULL,
	[WaterBudgetSlotBHeader]									VARCHAR(255)	NULL,
	[WaterBudgetSlotBWaterMeasurementTypeID]					INT				NULL,
	[WaterBudgetSlotCHeader]									VARCHAR(255)	NULL,
	[WaterBudgetSlotCWaterMeasurementTypeID]					INT				NULL,

	-- Misc Configuration Bits
	[FeeCalculatorEnabled]										BIT				NOT NULL DEFAULT(0),
	[AllowWaterMeasurementSelfReporting]						BIT				NOT NULL DEFAULT(0),
	[AllowFallowSelfReporting]									BIT				NOT NULL DEFAULT(0),
	[AllowCoverCropSelfReporting]								BIT				NOT NULL DEFAULT(0),
	[AllowLandownersToRequestAccountChanges]					BIT				NOT NULL DEFAULT(0),
    [IsDemoGeography]											BIT				NOT NULL DEFAULT(0),

	--Key Constraints
	CONSTRAINT [PK_Geography_GeographyID]																		PRIMARY KEY ([GeographyID]),

	CONSTRAINT [FK_Geography_GeographyConfiguration_GeographyConfigurationID]									FOREIGN KEY	([GeographyConfigurationID])				REFERENCES dbo.[GeographyConfiguration]([GeographyConfigurationID]),
	CONSTRAINT [FK_Geography_WaterMeasurementType_SourceOfRecordWaterMeasurementTypeID_WaterMeasurementTypeID]	FOREIGN KEY	([SourceOfRecordWaterMeasurementTypeID])	REFERENCES dbo.[WaterMeasurementType]([WaterMeasurementTypeID]),
	CONSTRAINT [FK_Geography_WaterMeasurementType_WaterBudgetSlotAWaterMeasurementTypeID]						FOREIGN KEY ([WaterBudgetSlotAWaterMeasurementTypeID])	REFERENCES dbo.[WaterMeasurementType]([WaterMeasurementTypeID]),
	CONSTRAINT [FK_Geography_WaterMeasurementType_WaterBudgetSlotBWaterMeasurementTypeID]						FOREIGN KEY ([WaterBudgetSlotBWaterMeasurementTypeID])	REFERENCES dbo.[WaterMeasurementType]([WaterMeasurementTypeID]),
	CONSTRAINT [FK_Geography_WaterMeasurementType_WaterBudgetSlotCWaterMeasurementTypeID]						FOREIGN KEY ([WaterBudgetSlotCWaterMeasurementTypeID])	REFERENCES dbo.[WaterMeasurementType]([WaterMeasurementTypeID]),

	--Unique Constraints
	CONSTRAINT [AK_Geography_GeographyConfigurationID]	UNIQUE([GeographyConfigurationID]),
	CONSTRAINT [AK_Geography_GeographyName]				UNIQUE([GeographyName]), 
	CONSTRAINT [AK_Geography_GeographyDisplayName]		UNIQUE([GeographyDisplayName])
)