CREATE TABLE [dbo].[WellRegistrationStatus]
(
	[WellRegistrationStatusID] [int] NOT NULL CONSTRAINT [PK_WellRegistrationStatus_WellRegistrationStatusID] PRIMARY KEY,
	[WellRegistrationStatusName] [varchar](20) NOT NULL CONSTRAINT [AK_WellRegistrationStatus_WellRegistrationStatusName] UNIQUE,
	[WellRegistrationStatusDisplayName] [varchar](20) NOT NULL CONSTRAINT [AK_WellRegistrationStatus_WellRegistrationStatusDisplayName] UNIQUE
)