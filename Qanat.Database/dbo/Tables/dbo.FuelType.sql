CREATE TABLE [dbo].[FuelType]
(
	[FuelTypeID] [int] NOT NULL CONSTRAINT [PK_FuelType_FuelTypeID] PRIMARY KEY,
	[FuelTypeName] [varchar](20) NOT NULL CONSTRAINT [AK_FuelType_FuelTypeName] UNIQUE,
	[FuelTypeDisplayName] [varchar](20) NOT NULL CONSTRAINT [AK_FuelType_FuelTypeDisplayName] UNIQUE,
)