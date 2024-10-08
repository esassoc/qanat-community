MERGE INTO dbo.FuelType AS Target
USING (VALUES
(1, 'Electric', 'Electric'),
(2, 'Diesel', 'Diesel'),
(3, 'LP Gas', 'LP Gas'),
(4, 'Other', 'Other')
)
AS Source (FuelTypeID, FuelTypeName, FuelTypeDisplayName)
ON Target.FuelTypeID = Source.FuelTypeID
WHEN MATCHED THEN
UPDATE SET
	FuelTypeName = Source.FuelTypeName,
	FuelTypeDisplayName = Source.FuelTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (FuelTypeID, FuelTypeName, FuelTypeDisplayName)
	VALUES (FuelTypeID, FuelTypeName, FuelTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
