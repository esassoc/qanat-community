CREATE TABLE [dbo].[FaqDisplayLocationType]
(
	[FaqDisplayLocationTypeID] [int] NOT NULL CONSTRAINT [PK_FaqDisplayLocationType_FaqDisplayLocationTypeID] PRIMARY KEY,
	[FaqDisplayLocationTypeName] [varchar](20) NOT NULL CONSTRAINT [AK_FaqDisplayLocationType_FaqDisplayLocationTypeName] UNIQUE,
	[FaqDisplayLocationTypeDisplayName] [varchar](20) NOT NULL CONSTRAINT [AK_FaqDisplayLocationType_FaqDisplayLocationTypeDisplayName] UNIQUE,
)