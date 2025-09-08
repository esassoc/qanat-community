MERGE INTO dbo.ScenarioRunOutputFileType AS Target
USING (VALUES
(1, 'GroundWaterBudget', '.json'),
(2, 'TimeSeriesData', '.json'),
(3, 'Water Budget', '.json'),
(4, 'Points of Interest', '.json')
)
AS Source (ScenarioRunOutputFileTypeID, ScenarioRunOutputFileTypeName, ScenarioRunOutputFileTypeExtension)
ON Target.ScenarioRunOutputFileTypeID = Source.ScenarioRunOutputFileTypeID
WHEN MATCHED THEN
UPDATE SET
	ScenarioRunOutputFileTypeName = Source.ScenarioRunOutputFileTypeName,
	ScenarioRunOutputFileTypeExtension = Source.ScenarioRunOutputFileTypeExtension
WHEN NOT MATCHED BY TARGET THEN
	INSERT (ScenarioRunOutputFileTypeID, ScenarioRunOutputFileTypeName, ScenarioRunOutputFileTypeExtension)
	VALUES (ScenarioRunOutputFileTypeID, ScenarioRunOutputFileTypeName, ScenarioRunOutputFileTypeExtension)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;

