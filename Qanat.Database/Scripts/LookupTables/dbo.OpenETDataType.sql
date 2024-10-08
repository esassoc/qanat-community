MERGE INTO dbo.OpenETDataType AS Target
USING (VALUES
(1, 'Evapotranspiration', 'Evapotranspiration', 'et'),
(2, 'Precipitation', 'Precipitation', 'pr')
)
AS Source (OpenETDataTypeID, OpenETDataTypeName, OpenETDataTypeDisplayName, OpenETDataTypeVariableName)
ON Target.OpenETDataTypeID = Source.OpenETDataTypeID
WHEN MATCHED THEN
UPDATE SET
	OpenETDataTypeName = Source.OpenETDataTypeName,
	OpenETDataTypeDisplayName = Source.OpenETDataTypeDisplayName,
	OpenETDataTypeVariableName = Source.OpenETDataTypeVariableName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (OpenETDataTypeID, OpenETDataTypeName, OpenETDataTypeDisplayName, OpenETDataTypeVariableName)
	VALUES (OpenETDataTypeID, OpenETDataTypeName, OpenETDataTypeDisplayName, OpenETDataTypeVariableName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
