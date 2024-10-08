CREATE TABLE [dbo].[ExternalMapLayer](
	[ExternalMapLayerID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ExternalMapLayer_ExternalMapLayerID] PRIMARY KEY,
	[GeographyID] int not null constraint [FK_ExternalMapLayer_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[ExternalMapLayerDisplayName] [varchar](100) NOT NULL,
	[ExternalMapLayerTypeID] [int] NOT NULL CONSTRAINT [FK_ExternalMapLayer_ExternalMapLayerType_ExternalMapLayerTypeID] FOREIGN KEY REFERENCES [dbo].[ExternalMapLayerType] ([ExternalMapLayerTypeID]),
	[ExternalMapLayerURL] [varchar](500) NOT NULL,
	[LayerIsOnByDefault] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[ExternalMapLayerDescription] [varchar](200) NULL,
	[PopUpField] [varchar](100) NULL,
	[MinZoom] int NULL
	CONSTRAINT [AK_ExternalMapLayers_Unique_ExternalMapLayerDisplayName_GeographyID] unique ([ExternalMapLayerDisplayName], [GeographyID])
)
