MERGE INTO dbo.StatementTemplateType AS Target
USING (VALUES
	(1, 'UsageStatement', 'Usage Statement', '{ "Page 1: Additional Information": 1, "Page 1: About This Usage Statement": 5, "Page 2: Additional Information": 1, "Page 2: Have Questions?": 1}', '{ "Balance": "Balance" }')
)
AS Source (StatementTemplateTypeID, StatementTemplateTypeName, StatementTemplateTypeDisplayName, CustomFieldDefaultParagraphs, CustomLabelDefaults)
ON Target.StatementTemplateTypeID = Source.StatementTemplateTypeID
WHEN MATCHED THEN
UPDATE SET
	StatementTemplateTypeName = Source.StatementTemplateTypeName,
	StatementTemplateTypeDisplayName = Source.StatementTemplateTypeDisplayName,
	CustomFieldDefaultParagraphs = Source.CustomFieldDefaultParagraphs,
	CustomLabelDefaults = Source.CustomLabelDefaults
WHEN NOT MATCHED BY TARGET THEN
	INSERT (StatementTemplateTypeID, StatementTemplateTypeName, StatementTemplateTypeDisplayName, CustomFieldDefaultParagraphs, CustomLabelDefaults)
	VALUES (StatementTemplateTypeID, StatementTemplateTypeName, StatementTemplateTypeDisplayName, CustomFieldDefaultParagraphs, CustomLabelDefaults)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
