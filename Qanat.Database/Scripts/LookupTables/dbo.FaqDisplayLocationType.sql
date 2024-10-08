MERGE INTO dbo.FaqDisplayLocationType AS Target
USING (VALUES
(1, 'GrowersGuide', 'Growers Guide'),
(2, 'WaterManagerGuide', 'Water Manager Guide')
)
AS Source (FaqDisplayLocationTypeID, FaqDisplayLocationTypeName, FaqDisplayLocationTypeDisplayName)
ON Target.FaqDisplayLocationTypeID = Source.FaqDisplayLocationTypeID
WHEN MATCHED THEN
UPDATE SET
	FaqDisplayLocationTypeName = Source.FaqDisplayLocationTypeName,
	FaqDisplayLocationTypeDisplayName = Source.FaqDisplayLocationTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (FaqDisplayLocationTypeID, FaqDisplayLocationTypeName, FaqDisplayLocationTypeDisplayName)
	VALUES (FaqDisplayLocationTypeID, FaqDisplayLocationTypeName, FaqDisplayLocationTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
