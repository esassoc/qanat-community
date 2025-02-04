MERGE INTO dbo.WaterMeasurementSelfReportStatus AS Target
USING (VALUES
	(1, 'Draft', 'Draft')
,	(2, 'Submitted', 'Submitted')
,	(3, 'Approved', 'Approved')
,	(4, 'Returned', 'Returned')
)
AS Source (WaterMeasurementSelfReportStatusID, WaterMeasurementSelfReportStatusName, WaterMeasurementSelfReportStatusDisplayName)
ON Target.WaterMeasurementSelfReportStatusID = Source.WaterMeasurementSelfReportStatusID
WHEN MATCHED THEN
UPDATE SET
	WaterMeasurementSelfReportStatusName = Source.WaterMeasurementSelfReportStatusName,
	WaterMeasurementSelfReportStatusDisplayName = Source.WaterMeasurementSelfReportStatusDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (WaterMeasurementSelfReportStatusID, WaterMeasurementSelfReportStatusName, WaterMeasurementSelfReportStatusDisplayName)
	VALUES (WaterMeasurementSelfReportStatusID, WaterMeasurementSelfReportStatusName, WaterMeasurementSelfReportStatusDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
