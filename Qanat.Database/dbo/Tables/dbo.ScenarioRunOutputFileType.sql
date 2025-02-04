CREATE TABLE [dbo].[ScenarioRunOutputFileType] (
    [ScenarioRunOutputFileTypeID]          INT           NOT NULL,
    [ScenarioRunOutputFileTypeName]        VARCHAR (100) NOT NULL,
    [ScenarioRunOutputFileTypeExtension] VARCHAR(10) NOT NULL,
    CONSTRAINT [PK_ScenarioRunOutputFileType_ScenarioRunOutputFileTypeID] PRIMARY KEY CLUSTERED ([ScenarioRunOutputFileTypeID] ASC),
    CONSTRAINT [AK_ScenarioRunOutputFileType_ScenarioRunOutputFileTypeName] UNIQUE NONCLUSTERED ([ScenarioRunOutputFileTypeName] ASC)
 );