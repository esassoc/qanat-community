MERGE INTO dbo.ExternalMapLayerType AS Target
USING (VALUES
(1, 'ESRIFeatureServer', 'ESRI Feature Server (WFS/vector)'),
(2, 'ESRIMapServer', 'ESRI Map Server (WMS/raster)')
)
AS Source (ExternalMapLayerTypeID, ExternalMapLayerTypeName, ExternalMapLayerTypeDisplayName)
ON Target.ExternalMapLayerTypeID = Source.ExternalMapLayerTypeID
WHEN MATCHED THEN
UPDATE SET
	ExternalMapLayerTypeName = Source.ExternalMapLayerTypeName,
	ExternalMapLayerTypeDisplayName = Source.ExternalMapLayerTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (ExternalMapLayerTypeID, ExternalMapLayerTypeName, ExternalMapLayerTypeDisplayName)
	VALUES (ExternalMapLayerTypeID, ExternalMapLayerTypeName, ExternalMapLayerTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
