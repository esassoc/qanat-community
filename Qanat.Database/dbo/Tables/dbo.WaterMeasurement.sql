CREATE TABLE [dbo].[WaterMeasurement](
	[WaterMeasurementID]			INT				NOT NULL IDENTITY(1,1)	CONSTRAINT [PK_WaterMeasurement_WaterMeasurementID] PRIMARY KEY,
	[GeographyID]					INT				NOT NULL				CONSTRAINT [FK_WaterMeasurement_Geography_GeographyID] FOREIGN KEY REFERENCES dbo.[Geography]([GeographyID]),
	[UsageLocationID]				INT				NOT NULL				CONSTRAINT [FK_WaterMeasurement_UsageLocation_UsageLocationID] FOREIGN KEY REFERENCES dbo.[UsageLocation]([UsageLocationID]),
	[WaterMeasurementTypeID]		INT				NULL					CONSTRAINT [FK_WaterMeasurement_WaterMeasurementType_WaterMeasurementTypeID] FOREIGN KEY REFERENCES dbo.[WaterMeasurementType]([WaterMeasurementTypeID]),
	[UnitTypeID]					INT				NULL					CONSTRAINT [FK_WaterMeasurement_UnitType_UnitTypeID] FOREIGN KEY REFERENCES dbo.[UnitType]([UnitTypeID]),	
	
	[ReportedDate]					DATETIME		NOT NULL,
	[ReportedValueInNativeUnits]	DECIMAL(20, 4)	NULL,
	[ReportedValueInAcreFeet]		DECIMAL(20, 4)	NOT NULL,
	[ReportedValueInFeet]			DECIMAL(20, 4)	NOT NULL,
	
	[LastUpdateDate]				DATETIME		NOT NULL,
	[FromManualUpload]				BIT				NOT NULL,
	[Comment]						VARCHAR(500)	NULL,
);
GO

CREATE INDEX IX_WaterMeasurement_GeographyID_WaterMeasurementTypeID_ReportedDate ON dbo.WaterMeasurement(GeographyID, WaterMeasurementTypeID, ReportedDate)
GO

CREATE INDEX IX_WaterMeasurement_UsageLocationID ON dbo.WaterMeasurement([UsageLocationID])
GO

CREATE INDEX IX_WaterMeasurement_UnitTypeID ON dbo.WaterMeasurement (UnitTypeID)
GO