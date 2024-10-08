MERGE INTO dbo.WellRegistrationContactType AS Target
USING (VALUES
(1, 'Landowner', 'Landowner'),
(2, 'OwnerOperator', 'OwnerOperator')
)
AS Source (WellRegistrationContactTypeID, WellRegistrationContactTypeName, WellRegistrationContactTypeDisplayName)
ON Target.WellRegistrationContactTypeID = Source.WellRegistrationContactTypeID
WHEN MATCHED THEN
UPDATE SET
	WellRegistrationContactTypeName = Source.WellRegistrationContactTypeName,
	WellRegistrationContactTypeDisplayName = Source.WellRegistrationContactTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (WellRegistrationContactTypeID, WellRegistrationContactTypeName, WellRegistrationContactTypeDisplayName)
	VALUES (WellRegistrationContactTypeID, WellRegistrationContactTypeName, WellRegistrationContactTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
