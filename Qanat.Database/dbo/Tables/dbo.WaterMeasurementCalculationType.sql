CREATE TABLE [dbo].[WaterMeasurementCalculationType]
(
	[WaterMeasurementCalculationTypeID] [int] NOT NULL CONSTRAINT [PK_WaterMeasurementCalculationType_WaterMeasurementCalculationTypeID] PRIMARY KEY,
	[WaterMeasurementCalculationTypeName] [varchar](255) NOT NULL CONSTRAINT [AK_WaterMeasurementCalculationType_WaterMeasurementCalculationTypeName] UNIQUE,
	[WaterMeasurementCalculationTypeDisplayName] [varchar](255) NOT NULL CONSTRAINT [AK_WaterMeasurementCalculationType_WaterMeasurementCalculationTypeDisplayName] UNIQUE,
)