CREATE TABLE [dbo].[StatementTemplateType]
(
	[StatementTemplateTypeID] INT NOT NULL CONSTRAINT [PK_StatementTemplateType_StatementTemplateTypeID] PRIMARY KEY,
	[StatementTemplateTypeName] VARCHAR(50) NOT NULL CONSTRAINT [AK_StatementTemplateType_StatementTemplateTypeName] UNIQUE,
	[StatementTemplateTypeDisplayName] VARCHAR(50) NOT NULL CONSTRAINT [AK_StatementTemplateType_StatementTemplateTypeDisplayName] UNIQUE,
	[CustomFieldDefaultParagraphs] VARCHAR(MAX) NOT NULL DEFAULT ('{}'),
	[CustomLabelDefaults] VARCHAR(MAX) NOT NULL DEFAULT ('{}'),
)
