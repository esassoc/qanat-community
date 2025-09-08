MERGE INTO dbo.GETActionOutputFileType AS Target
USING (VALUES
(1, 'GroundWaterBudget', '.json'),
(2, 'TimeSeriesData', '.json'),
(3, 'Water Budget', '.json'),
(4, 'Points of Interest', '.json')
)
AS Source (GETActionOutputFileTypeID, GETActionOutputFileTypeName, GETActionOutputFileTypeExtension)
ON Target.GETActionOutputFileTypeID = Source.GETActionOutputFileTypeID
WHEN MATCHED THEN
UPDATE SET
	GETActionOutputFileTypeName = Source.GETActionOutputFileTypeName,
	GETActionOutputFileTypeExtension = Source.GETActionOutputFileTypeExtension
WHEN NOT MATCHED BY TARGET THEN
	INSERT (GETActionOutputFileTypeID, GETActionOutputFileTypeName, GETActionOutputFileTypeExtension)
	VALUES (GETActionOutputFileTypeID, GETActionOutputFileTypeName, GETActionOutputFileTypeExtension)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;

