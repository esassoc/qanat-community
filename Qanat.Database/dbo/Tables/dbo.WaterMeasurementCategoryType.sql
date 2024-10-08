CREATE TABLE [dbo].[WaterMeasurementCategoryType]
(
	[WaterMeasurementCategoryTypeID] [int] NOT NULL CONSTRAINT [PK_WaterMeasurementCategoryType_WaterMeasurementCategoryTypeID] PRIMARY KEY,
	[WaterMeasurementCategoryTypeName] [varchar](20) NOT NULL CONSTRAINT [AK_WaterMeasurementCategoryType_WaterMeasurementCategoryTypeName] UNIQUE,
	[WaterMeasurementCategoryTypeDisplayName] [varchar](20) NOT NULL CONSTRAINT [AK_WaterMeasurementCategoryType_WaterMeasurementCategoryTypeDisplayName] UNIQUE,
)