MERGE INTO dbo.WellRegistrationWaterUseType AS Target
USING (VALUES
(1, 'Agricultural', 'Agricultural'),
(2, 'StockWatering', 'Stock Watering'),
(3, 'Domestic', 'Domestic'),
(4, 'PublicMunicipal', 'Public Municipal'),
(5, 'PrivateMunicipal', 'Private Municipal'),
(6, 'Other', 'Other')
)
AS Source (WellRegistrationWaterUseTypeID, WellRegistrationWaterUseTypeName, WellRegistrationWaterUseTypeDisplayName)
ON Target.WellRegistrationWaterUseTypeID = Source.WellRegistrationWaterUseTypeID
WHEN MATCHED THEN
UPDATE SET
	WellRegistrationWaterUseTypeName = Source.WellRegistrationWaterUseTypeName,
	WellRegistrationWaterUseTypeDisplayName = Source.WellRegistrationWaterUseTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (WellRegistrationWaterUseTypeID, WellRegistrationWaterUseTypeName, WellRegistrationWaterUseTypeDisplayName)
	VALUES (WellRegistrationWaterUseTypeID, WellRegistrationWaterUseTypeName, WellRegistrationWaterUseTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
