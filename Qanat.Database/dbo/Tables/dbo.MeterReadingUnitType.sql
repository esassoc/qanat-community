CREATE TABLE [dbo].[MeterReadingUnitType] (
	[MeterReadingUnitTypeID] [int] NOT NULL CONSTRAINT [PK_MeterReadingUnitType_MeterReadingUnitTypeID] PRIMARY KEY,
	[MeterReadingUnitTypeName] [varchar](50) NOT NULL CONSTRAINT [AK_MeterReadingUnitType_MeterReadingUnitTypeName] unique,
	[MeterReadingUnitTypeDisplayName] [varchar](50) NOT NULL CONSTRAINT [AK_MeterReadingUnitType_MeterReadingUnitTypeDisplayName] unique,
	[MeterReadingUnitTypeAbbreviation] [varchar](8) NOT NULL CONSTRAINT [AK_MeterReadingUnitType_MeterReadingUnitTypeAbbreviation] unique,
	[MeterReadingUnitTypeAlternateDisplayName] [varchar](50) NULL
)