CREATE TABLE [dbo].[UnitType](
	[UnitTypeID] [int] NOT NULL CONSTRAINT [PK_UnitType_UnitTypeID] PRIMARY KEY,
	[UnitTypeName] [varchar](50) NOT NULL CONSTRAINT [AK_UnitType_UnitTypeName] unique,
	[UnitTypeDisplayName] [varchar](50) NOT NULL CONSTRAINT [AK_UnitType_UnitTypeDisplayName] unique,
	[UnitTypeAbbreviation] [varchar](8) NOT NULL CONSTRAINT [AK_UnitType_UnitTypeAbbreviation] unique
)
