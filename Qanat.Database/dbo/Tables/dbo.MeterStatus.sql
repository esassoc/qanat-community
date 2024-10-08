CREATE TABLE [dbo].[MeterStatus]
(
	[MeterStatusID] [int] NOT NULL CONSTRAINT [PK_MeterStatus_MeterStatusID] PRIMARY KEY,
	[MeterStatusName] [varchar](20) NOT NULL CONSTRAINT [AK_MeterStatus_MeterStatusName] UNIQUE,
	[MeterStatusDisplayName] [varchar](20) NOT NULL CONSTRAINT [AK_MeterStatus_MeterStatusDisplayName] UNIQUE
)