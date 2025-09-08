CREATE TABLE [dbo].[StatementTemplate]
(
	[StatementTemplateID] INT NOT NULL IDENTITY(1,1) CONSTRAINT [PK_StatementTemplate_StatementTemplateID] PRIMARY KEY,
	[GeographyID] INT NOT NULL CONSTRAINT [FK_StatementTemplate_Geography_GeographyID] FOREIGN KEY REFERENCES [dbo].[Geography]([GeographyID]),
	[StatementTemplateTypeID] INT NOT NULL CONSTRAINT [FK_StatementTemplate_StatementTemplateType_StatementTemplateTypeID] FOREIGN KEY REFERENCES [dbo].[StatementTemplateType]([StatementTemplateTypeID]),
	[TemplateTitle] VARCHAR(100) NOT NULL,
	[LastUpdated] DATETIME NOT NULL,
	[UpdateUserID] INT NOT NULL CONSTRAINT [FK_StatementTemplate_User_UpdateUserID_UserID] FOREIGN KEY REFERENCES [dbo].[User]([UserID]),
	[InternalDescription] VARCHAR(MAX) NULL,
	[CustomFieldsContent] VARCHAR(MAX) NOT NULL DEFAULT ('{}'),
	[CustomLabels] VARCHAR(MAX) NULL,
	CONSTRAINT [AK_StatementTemplate_GeographyID_TemplateName] UNIQUE ([GeographyID], [TemplateTitle])
)