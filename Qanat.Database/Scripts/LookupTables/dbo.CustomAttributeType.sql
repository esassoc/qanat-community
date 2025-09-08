MERGE INTO dbo.CustomAttributeType AS Target
USING (VALUES
(1, 'WaterAccount', 'Water Account'),
(2, 'Parcel', 'Parcel')
)
AS Source (CustomAttributeTypeID, CustomAttributeTypeName, CustomAttributeTypeDisplayName)
ON Target.CustomAttributeTypeID = Source.CustomAttributeTypeID
WHEN MATCHED THEN
UPDATE SET
	CustomAttributeTypeName = Source.CustomAttributeTypeName,
	CustomAttributeTypeDisplayName = Source.CustomAttributeTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (CustomAttributeTypeID, CustomAttributeTypeName, CustomAttributeTypeDisplayName)
	VALUES (CustomAttributeTypeID, CustomAttributeTypeName, CustomAttributeTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
