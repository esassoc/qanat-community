MERGE INTO dbo.MeterStatus AS Target
USING (VALUES
(1, 'Active', 'Active'),
(2, 'Broken', 'Broken'),
(3, 'Retired', 'Retired')
)
AS Source (MeterStatusID, MeterStatusName, MeterStatusDisplayName)
ON Target.MeterStatusID = Source.MeterStatusID
WHEN MATCHED THEN
UPDATE SET
	MeterStatusName = Source.MeterStatusName,
	MeterStatusDisplayName = Source.MeterStatusDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (MeterStatusID, MeterStatusName, MeterStatusDisplayName)
	VALUES (MeterStatusID, MeterStatusName, MeterStatusDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
