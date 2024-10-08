CREATE TABLE [dbo].[WaterMeasurement](
	[WaterMeasurementID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_WaterMeasurement_WaterMeasurementID] PRIMARY KEY,
	[GeographyID] int not null constraint [FK_WaterMeasurement_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[WaterMeasurementTypeID] int null constraint [FK_WaterMeasurement_WaterMeasurementType_WaterMeasurementTypeID] foreign key references dbo.[WaterMeasurementType]([WaterMeasurementTypeID]),
	[UnitTypeID] int null constraint [FK_WaterMeasurement_UnitType_UnitTypeID] foreign key references dbo.[UnitType]([UnitTypeID]),
	[UsageEntityName] [varchar](100) NOT NULL,
	[ReportedDate] datetime NOT NULL,
	[ReportedValue] [decimal](20, 4) NOT NULL,
	[ReportedValueInAcreFeet] [decimal](20, 4) NULL,
	[UsageEntityArea] [decimal](20, 4) NULL,
	[LastUpdateDate] datetime not null,
	[FromManualUpload] bit not null,
	[Comment] [varchar](500) NULL,
	--CONSTRAINT [AK_WaterMeasurement_GeographyID_WaterMeasurementTypeID_ReportedDate_UsageEntityName_UsageEntityArea] UNIQUE ([GeographyID], [WaterMeasurementTypeID], [ReportedDate], [UsageEntityName], [UsageEntityArea]),
);
GO

CREATE INDEX IX_WaterMeasurement_GeographyID_WaterMeasurementTypeID_ReportedDate ON dbo.WaterMeasurement(GeographyID, WaterMeasurementTypeID, ReportedDate)
GO

CREATE INDEX IX_WaterMeasurement_UsageEntityName ON dbo.WaterMeasurement(UsageEntityName)
GO