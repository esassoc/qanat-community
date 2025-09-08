CREATE TABLE [HangFire].[State](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[JobId] [bigint] NOT NULL CONSTRAINT [FK_HangFire_State_Job] FOREIGN KEY REFERENCES [HangFire].[Job] ([Id]) ON UPDATE CASCADE ON DELETE CASCADE,
	[Name] [nvarchar](20) NOT NULL,
	[Reason] [nvarchar](100) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[Data] [nvarchar](max) NULL,
	CONSTRAINT [PK_HangFire_State] PRIMARY KEY ([JobId], [Id])
)
