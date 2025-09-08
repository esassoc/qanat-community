CREATE TABLE [dbo].[CustomRichTextType] (
    [CustomRichTextTypeID]          INT           NOT NULL,
    [CustomRichTextTypeName]        VARCHAR (100) NOT NULL,
    [CustomRichTextTypeDisplayName] VARCHAR (100) NOT NULL,
    [ContentTypeID]                 INT           NULL
    CONSTRAINT [PK_CustomRichTextType_CustomRichTextTypeID] PRIMARY KEY CLUSTERED ([CustomRichTextTypeID] ASC),
    CONSTRAINT [AK_CustomRichTextType_CustomRichTextTypeDisplayName] UNIQUE NONCLUSTERED ([CustomRichTextTypeDisplayName] ASC),
    CONSTRAINT [AK_CustomRichTextType_CustomRichTextTypeName] UNIQUE NONCLUSTERED ([CustomRichTextTypeName] ASC),
    CONSTRAINT [FK_CustomRichTextType_ContentType_ContentTypeID] FOREIGN KEY ([ContentTypeID]) REFERENCES [dbo].[ContentType] ([ContentTypeID])
);