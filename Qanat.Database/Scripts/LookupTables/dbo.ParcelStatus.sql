MERGE INTO dbo.ParcelStatus AS Target
USING (VALUES
(1, 'Assigned', 'Active'),
(2, 'Inactive', 'Inactive'),
(3, 'Unassigned', 'Unassigned'),
(4, 'Excluded', 'Excluded')
)
AS Source (ParcelStatusID, ParcelStatusName, ParcelStatusDisplayName)
ON Target.ParcelStatusID = Source.ParcelStatusID
WHEN MATCHED THEN
UPDATE SET
	ParcelStatusName = Source.ParcelStatusName,
	ParcelStatusDisplayName = Source.ParcelStatusDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (ParcelStatusID, ParcelStatusName, ParcelStatusDisplayName)
	VALUES (ParcelStatusID, ParcelStatusName, ParcelStatusDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
