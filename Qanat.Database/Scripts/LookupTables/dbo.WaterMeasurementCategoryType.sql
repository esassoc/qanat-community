MERGE INTO dbo.WaterMeasurementCategoryType AS Target
USING (VALUES
(1, 'ET', 'ET'),
(2, 'Precip', 'Precip'),
(3, 'Meter', 'Meter'),
(4, 'SurfaceWater', 'Surface Water'),
(5, 'Calculated', 'Calculated'),
(6, 'Precipitation Credit', 'PrecipitationCredit'),
(7, 'Manual Adjustment', 'ManualAdjustment')
)
AS Source (WaterMeasurementCategoryTypeID, WaterMeasurementCategoryTypeName, WaterMeasurementCategoryTypeDisplayName)
ON Target.WaterMeasurementCategoryTypeID = Source.WaterMeasurementCategoryTypeID
WHEN MATCHED THEN
UPDATE SET
	WaterMeasurementCategoryTypeName = Source.WaterMeasurementCategoryTypeName,
	WaterMeasurementCategoryTypeDisplayName = Source.WaterMeasurementCategoryTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (WaterMeasurementCategoryTypeID, WaterMeasurementCategoryTypeName, WaterMeasurementCategoryTypeDisplayName)
	VALUES (WaterMeasurementCategoryTypeID, WaterMeasurementCategoryTypeName, WaterMeasurementCategoryTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
