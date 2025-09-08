CREATE TABLE [HangFire].[Server](
	[Id] [nvarchar](200) NOT NULL CONSTRAINT [PK_HangFire_Server] PRIMARY KEY,
	[Data] [nvarchar](max) NULL,
	[LastHeartbeat] [datetime] NOT NULL
)
GO

CREATE NONCLUSTERED INDEX [IX_HangFire_Server_LastHeartbeat] ON [HangFire].[Server]
(
	[LastHeartbeat] ASC
)