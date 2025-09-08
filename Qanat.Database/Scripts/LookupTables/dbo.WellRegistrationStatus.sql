MERGE INTO dbo.WellRegistrationStatus AS Target
USING (VALUES
(1, 'Draft', 'Draft'),
(2, 'Submitted', 'Submitted'),
(3, 'Returned', 'Returned'),
(4, 'Approved', 'Approved')
)
AS Source (WellRegistrationStatusID, WellRegistrationStatusName, WellRegistrationStatusDisplayName)
ON Target.WellRegistrationStatusID = Source.WellRegistrationStatusID
WHEN MATCHED THEN
UPDATE SET
	WellRegistrationStatusName = Source.WellRegistrationStatusName,
	WellRegistrationStatusDisplayName = Source.WellRegistrationStatusDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (WellRegistrationStatusID, WellRegistrationStatusName, WellRegistrationStatusDisplayName)
	VALUES (WellRegistrationStatusID, WellRegistrationStatusName, WellRegistrationStatusDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
