CREATE TABLE [dbo].[CustomAttributeType]
(
	[CustomAttributeTypeID] [int] NOT NULL CONSTRAINT [PK_CustomAttributeType_CustomAttributeTypeID] PRIMARY KEY,
	[CustomAttributeTypeName] [varchar](20) NOT NULL CONSTRAINT [AK_CustomAttributeType_CustomAttributeTypeName] UNIQUE,
	[CustomAttributeTypeDisplayName] [varchar](20) NOT NULL CONSTRAINT [AK_CustomAttributeType_CustomAttributeTypeDisplayName] UNIQUE,
)