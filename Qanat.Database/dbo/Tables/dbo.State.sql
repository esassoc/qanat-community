CREATE TABLE [dbo].[State]
(
	[StateID] [int] NOT NULL CONSTRAINT [PK_State_StateID] PRIMARY KEY,
	[StateName] [varchar](20) NOT NULL CONSTRAINT [AK_State_StateName] UNIQUE,
	[StatePostalCode] [varchar](2) NOT NULL CONSTRAINT [AK_State_StatePostalCode] UNIQUE,
)