MERGE INTO dbo.SelfReportStatus AS Target
USING (VALUES
	(1, 'Draft', 'Draft')
,	(2, 'Submitted', 'Submitted')
,	(3, 'Approved', 'Approved')
,	(4, 'Returned', 'Returned')
)
AS Source (SelfReportStatusID, SelfReportStatusName, SelfReportStatusDisplayName)
ON Target.SelfReportStatusID = Source.SelfReportStatusID
WHEN MATCHED THEN
UPDATE SET
	SelfReportStatusName = Source.SelfReportStatusName,
	SelfReportStatusDisplayName = Source.SelfReportStatusDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (SelfReportStatusID, SelfReportStatusName, SelfReportStatusDisplayName)
	VALUES (SelfReportStatusID, SelfReportStatusName, SelfReportStatusDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
