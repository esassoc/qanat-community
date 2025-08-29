MERGE INTO dbo.WellStatus AS Target
USING (VALUES
(1, 'Operational', 'Operational'),
(2, 'NonOperational', 'Non-Operational'),
(3, 'Duplicate', 'Duplicate')
)
AS Source (WellStatusID, WellStatusName, WellStatusDisplayName)
ON Target.WellStatusID = Source.WellStatusID
WHEN MATCHED THEN
UPDATE SET
	WellStatusName = Source.WellStatusName,
	WellStatusDisplayName = Source.WellStatusDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (WellStatusID, WellStatusName, WellStatusDisplayName)
	VALUES (WellStatusID, WellStatusName, WellStatusDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
