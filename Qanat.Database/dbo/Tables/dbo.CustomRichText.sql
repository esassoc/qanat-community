CREATE TABLE [dbo].[CustomRichText] (
    [CustomRichTextID]      INT          IDENTITY (1, 1) NOT NULL,
    [CustomRichTextTypeID]  INT          NOT NULL,
    [CustomRichTextTitle]   varchar(200) NULL,
    [CustomRichTextContent] [dbo].[html] NULL,
    [GeographyID] int null constraint [FK_CustomRichText_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
    CONSTRAINT [PK_CustomRichText_CustomRichTextID] PRIMARY KEY CLUSTERED ([CustomRichTextID] ASC),
    CONSTRAINT [FK_CustomRichText_CustomRichTextType_CustomRichTextTypeID] FOREIGN KEY ([CustomRichTextTypeID]) REFERENCES [dbo].[CustomRichTextType] ([CustomRichTextTypeID]),
    CONSTRAINT [AK_CustomRichText_Unique_CustomRichTextTypeID_GeographyID] unique ([CustomRichTextTypeID], [GeographyID])
);

