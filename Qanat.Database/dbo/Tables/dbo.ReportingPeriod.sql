CREATE TABLE [dbo].[ReportingPeriod](
	[ReportingPeriodID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ReportingPeriod_ReportingPeriodID] PRIMARY KEY,
	[GeographyID] int not null constraint [FK_ReportingPeriod_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[ReportingPeriodName] [varchar](50) NOT NULL,
	[StartMonth] [int] NOT NULL,
	[Interval] [varchar](20) NOT NULL,
	CONSTRAINT [AK_GeographyID] unique ([GeographyID])
)