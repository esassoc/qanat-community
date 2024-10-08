CREATE TABLE [dbo].[GETActionOutputFileType] (
    [GETActionOutputFileTypeID]          INT           NOT NULL,
    [GETActionOutputFileTypeName]        VARCHAR (100) NOT NULL,
    [GETActionOutputFileTypeExtension] VARCHAR(10) NOT NULL,
    CONSTRAINT [PK_GETActionOutputFileType_GETActionOutputFileTypeID] PRIMARY KEY CLUSTERED ([GETActionOutputFileTypeID] ASC),
    CONSTRAINT [AK_GETActionOutputFileType_GETActionOutputFileTypeName] UNIQUE NONCLUSTERED ([GETActionOutputFileTypeName] ASC)
 );