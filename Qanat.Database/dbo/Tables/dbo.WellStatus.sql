CREATE TABLE [dbo].[WellStatus]
(
	[WellStatusID] [int] NOT NULL CONSTRAINT [PK_WellStatus_WellStatusID] PRIMARY KEY,
	[WellStatusName] [varchar](20) NOT NULL CONSTRAINT [AK_WellStatus_WellStatusName] UNIQUE,
	[WellStatusDisplayName] [varchar](20) NOT NULL CONSTRAINT [AK_WellStatus_WellStatusDisplayName] UNIQUE
) 
