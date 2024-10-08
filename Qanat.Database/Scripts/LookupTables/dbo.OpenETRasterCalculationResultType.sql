MERGE INTO dbo.OpenETRasterCalculationResultType AS Target
USING (VALUES
(1, 'NotStarted', 'Not Started'),
(2, 'InProgress', 'In Progress'),
(3, 'Succeeded', 'Succeeded'),
(4, 'Failed', 'Failed')
)
AS Source (OpenETRasterCalculationResultTypeID, OpenETRasterCalculationResultTypeName, OpenETRasterCalculationResultTypeDisplayName)
ON Target.OpenETRasterCalculationResultTypeID = Source.OpenETRasterCalculationResultTypeID
WHEN MATCHED THEN
UPDATE SET
	OpenETRasterCalculationResultTypeName = Source.OpenETRasterCalculationResultTypeName,
	OpenETRasterCalculationResultTypeDisplayName = Source.OpenETRasterCalculationResultTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (OpenETRasterCalculationResultTypeID, OpenETRasterCalculationResultTypeName, OpenETRasterCalculationResultTypeDisplayName)
	VALUES (OpenETRasterCalculationResultTypeID, OpenETRasterCalculationResultTypeName, OpenETRasterCalculationResultTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;