CREATE TABLE [HangFire].[AggregatedCounter](
	[Key] [nvarchar](100) NOT NULL CONSTRAINT [PK_HangFire_CounterAggregated] PRIMARY KEY,
	[Value] [bigint] NOT NULL,
	[ExpireAt] [datetime] NULL
)
GO

CREATE NONCLUSTERED INDEX [IX_HangFire_AggregatedCounter_ExpireAt] ON [HangFire].[AggregatedCounter]
(
	[ExpireAt] ASC
)
WHERE ([ExpireAt] IS NOT NULL)
