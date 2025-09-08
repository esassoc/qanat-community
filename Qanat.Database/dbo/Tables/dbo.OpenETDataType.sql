CREATE TABLE [dbo].[OpenETDataType](
	[OpenETDataTypeID] [int] NOT NULL CONSTRAINT [PK_OpenETDataType_OpenETDataTypeID] PRIMARY KEY,
	[OpenETDataTypeName] [varchar](100) NOT NULL CONSTRAINT [AK_OpenETDataType_AK_OpenETDataTypeName] UNIQUE,
	[OpenETDataTypeDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_OpenETDataType_OpenETDataTypeDisplayName] UNIQUE,
	[OpenETDataTypeVariableName] [varchar](20) NOT NULL CONSTRAINT [AK_OpenETDataType_OpenETDataTypeVariableName] UNIQUE
)
