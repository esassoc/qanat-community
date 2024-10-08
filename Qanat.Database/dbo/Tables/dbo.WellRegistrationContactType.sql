CREATE TABLE [dbo].[WellRegistrationContactType]
(
	[WellRegistrationContactTypeID] [int] NOT NULL CONSTRAINT [PK_WellRegistrationContactType_WellRegistrationContactTypeID] PRIMARY KEY,
	[WellRegistrationContactTypeName] [varchar](20) NOT NULL CONSTRAINT [AK_WellRegistrationContactType_WellRegistrationContactTypeName] UNIQUE,
	[WellRegistrationContactTypeDisplayName] [varchar](20) NOT NULL CONSTRAINT [AK_WellRegistrationContactType_WellRegistrationContactTypeDisplayName] UNIQUE
)