CREATE TABLE [dbo].[ContentType] (
    [ContentTypeID]          INT           NOT NULL,
    [ContentTypeName]        VARCHAR (100) NOT NULL,
    [ContentTypeDisplayName] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_ContentType_ContentTypeID] PRIMARY KEY CLUSTERED ([ContentTypeID] ASC),
    CONSTRAINT [AK_ContentType_ContentTypeDisplayName] UNIQUE NONCLUSTERED ([ContentTypeDisplayName] ASC),
    CONSTRAINT [AK_ContentType_ContentTypeName] UNIQUE NONCLUSTERED ([ContentTypeName] ASC)
);