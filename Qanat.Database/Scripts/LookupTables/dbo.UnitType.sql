MERGE INTO dbo.UnitType AS Target
USING (VALUES
(1, 'Inches', 'inches', 'in'),
(2, 'Millimeters', 'millimeters', 'mm')
--(3, 'AcreFeet', 'acre-feet', 'ac-ft') # commented out because our water measurements process groups by depth,
)
AS Source (UnitTypeID, UnitTypeName, UnitTypeDisplayName, UnitTypeAbbreviation)
ON Target.UnitTypeID = Source.UnitTypeID
WHEN MATCHED THEN
UPDATE SET
	UnitTypeName = Source.UnitTypeName,
	UnitTypeDisplayName = Source.UnitTypeDisplayName,
	UnitTypeAbbreviation = Source.UnitTypeAbbreviation
WHEN NOT MATCHED BY TARGET THEN
	INSERT (UnitTypeID, UnitTypeName, UnitTypeDisplayName, UnitTypeAbbreviation)
	VALUES (UnitTypeID, UnitTypeName, UnitTypeDisplayName, UnitTypeAbbreviation)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
