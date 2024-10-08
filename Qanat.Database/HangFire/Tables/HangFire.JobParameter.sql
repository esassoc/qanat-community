CREATE TABLE [HangFire].[JobParameter](
	[JobId] [bigint] NOT NULL CONSTRAINT [FK_HangFire_JobParameter_Job] FOREIGN KEY REFERENCES [HangFire].[Job] ([Id]) ON UPDATE CASCADE ON DELETE CASCADE,
	[Name] [nvarchar](40) NOT NULL,
	[Value] [nvarchar](max) NULL,
	CONSTRAINT [PK_HangFire_JobParameter] PRIMARY KEY ([JobId], [Name])
)