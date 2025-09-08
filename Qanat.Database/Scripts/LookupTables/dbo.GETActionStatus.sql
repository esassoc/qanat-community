MERGE INTO dbo.GETActionStatus AS Target
USING (VALUES
(1, null, 'Created', 'Created', 0),
(2, null, 'GETIntegrationFailure', 'GET Integration Failure', 0),
(3, 0, 'CreatedInGET', 'Created in GET', 0),
(4, 1, 'Queued', 'Queued', 0),
(5, 2, 'Processing', 'Processing', 0),
(6, 3, 'Complete', 'Complete', 1),
(7, 4, 'SystemError', 'System Error', 1),
(8, 5, 'InvalidOutput', 'Invalid Output', 1),
(9, 6, 'InvalidInput', 'Invalid Input', 1),
(10, 7, 'HasDryCells', 'Completed with Dry Cells', 1),
(11, 8, 'AnalysisFailed', 'Analysis Failed', 1),
(12, 9, 'AnalysisSuccess', 'Analysis Succeeded', 0),
(13, 10, 'ProcesingInputs', 'Processing Inputs', 0),
(14, 11, 'RunningAnalysis', 'Running Analysis', 0)
)
AS Source (GETActionStatusID, GETRunStatusID, GETActionStatusName, GETActionStatusDisplayName, IsTerminal)
ON Target.GETActionStatusID = Source.GETActionStatusID
WHEN MATCHED THEN
UPDATE SET
	GETRunStatusID = Source.GETRunStatusID,
	GETActionStatusName = Source.GETActionStatusName,
	GETActionStatusDisplayName = Source.GETActionStatusDisplayName,
	IsTerminal = Source.IsTerminal
WHEN NOT MATCHED BY TARGET THEN
	INSERT (GETActionStatusID, GETRunStatusID, GETActionStatusName, GETActionStatusDisplayName, IsTerminal)
	VALUES (GETActionStatusID, GETRunStatusID, GETActionStatusName, GETActionStatusDisplayName, IsTerminal)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
