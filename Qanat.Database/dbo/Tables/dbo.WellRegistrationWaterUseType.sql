CREATE TABLE [dbo].[WellRegistrationWaterUseType]
(
	[WellRegistrationWaterUseTypeID] [int] NOT NULL CONSTRAINT [PK_WellRegistrationWaterUseType_WellRegistrationWaterUseTypeID] PRIMARY KEY,
	[WellRegistrationWaterUseTypeName] [varchar](20) NOT NULL CONSTRAINT [AK_WellRegistrationWaterUseType_WellRegistrationWaterUseTypeName] UNIQUE,
	[WellRegistrationWaterUseTypeDisplayName] [varchar](20) NOT NULL CONSTRAINT [AK_WellRegistrationWaterUseType_WellRegistrationWaterUseTypeDisplayName] UNIQUE
)