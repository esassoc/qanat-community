MERGE INTO MeterReadingUnitType AS Target
USING (VALUES
	(1, 'AcreFeet', 'acre-feet', 'ac-ft','AF'),
	(2, 'Gallons', 'gallons', 'gal', null)
)
AS Source (MeterReadingUnitTypeID, MeterReadingUnitTypeName, MeterReadingUnitTypeDisplayName, MeterReadingUnitTypeAbbreviation, MeterReadingUnitTypeAlternateDisplayName)
ON Target.MeterReadingUnitTypeID = Source.MeterReadingUnitTypeID
WHEN MATCHED THEN
UPDATE SET
	MeterReadingUnitTypeName = Source.MeterReadingUnitTypeName,
	MeterReadingUnitTypeDisplayName = Source.MeterReadingUnitTypeDisplayName,
	MeterReadingUnitTypeAbbreviation = Source.MeterReadingUnitTypeAbbreviation,
	MeterReadingUnitTypeAlternateDisplayName = Source.MeterReadingUnitTypeAlternateDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (MeterReadingUnitTypeID, MeterReadingUnitTypeName, MeterReadingUnitTypeDisplayName, MeterReadingUnitTypeAbbreviation, MeterReadingUnitTypeAlternateDisplayName)
	VALUES (MeterReadingUnitTypeID, MeterReadingUnitTypeName, MeterReadingUnitTypeDisplayName, MeterReadingUnitTypeAbbreviation, MeterReadingUnitTypeAlternateDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
