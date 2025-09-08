CREATE TABLE [dbo].[ExternalMapLayerType]
(
	[ExternalMapLayerTypeID] [int] NOT NULL CONSTRAINT [PK_ExternalMapLayerType_ExternalMapLayerTypeID] PRIMARY KEY,
	[ExternalMapLayerTypeName] [varchar](50) NOT NULL CONSTRAINT [AK_ExternalMapLayerType_ExternalMapLayerTypeName] UNIQUE,
	[ExternalMapLayerTypeDisplayName] [varchar](50) NOT NULL CONSTRAINT [AK_ExternalMapLayerType_ExternalMapLayerTypeDisplayName] UNIQUE
)
