CREATE TABLE [dbo].[SelfReportStatus]
(
	[SelfReportStatusID]			INT			NOT NULL	CONSTRAINT [PK_SelfReportStatus_SelfReportStatusID] PRIMARY KEY,
	[SelfReportStatusName]			VARCHAR(20) NOT NULL	CONSTRAINT [AK_SelfReportStatus_SelfReportStatusName] UNIQUE,
	[SelfReportStatusDisplayName]	VARCHAR(20) NOT NULL	CONSTRAINT [AK_SelfReportStatus_SelfReportStatusDisplayName] UNIQUE
) 
