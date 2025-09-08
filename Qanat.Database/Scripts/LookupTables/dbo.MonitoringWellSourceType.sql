MERGE INTO dbo.MonitoringWellSourceType AS Target
USING (VALUES
(1, 'CNRA', 'California Natural Resources Agency'),
(2, 'YoloWRID', 'Yolo WRID')
)
AS Source (MonitoringWellSourceTypeID, MonitoringWellSourceTypeName, MonitoringWellSourceTypeDisplayName)
ON Target.MonitoringWellSourceTypeID = Source.MonitoringWellSourceTypeID
WHEN MATCHED THEN
UPDATE SET
	MonitoringWellSourceTypeName = Source.MonitoringWellSourceTypeName,
	MonitoringWellSourceTypeDisplayName = Source.MonitoringWellSourceTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (MonitoringWellSourceTypeID, MonitoringWellSourceTypeName, MonitoringWellSourceTypeDisplayName)
	VALUES (MonitoringWellSourceTypeID, MonitoringWellSourceTypeName, MonitoringWellSourceTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
