MERGE INTO dbo.ContentType AS Target
USING (VALUES
(1, 'PageInstructions', 'Page Instructions'),
(2, 'FormInstructions', 'Form Instructions'),
(3, 'FieldDefinition', 'Field Definition')
)
AS Source (ContentTypeID, ContentTypeName, ContentTypeDisplayName)
ON Target.ContentTypeID = Source.ContentTypeID
WHEN MATCHED THEN
UPDATE SET
	ContentTypeName = Source.ContentTypeName,
	ContentTypeDisplayName = Source.ContentTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (ContentTypeID, ContentTypeName, ContentTypeDisplayName)
	VALUES (ContentTypeID, ContentTypeName, ContentTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
