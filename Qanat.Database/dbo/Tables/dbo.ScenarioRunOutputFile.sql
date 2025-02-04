CREATE TABLE [dbo].[ScenarioRunOutputFile]
(
	[ScenarioRunOutputFileID] int not null identity(1,1) constraint PK_ScenarioRunOutputFile_ScenarioRunOutputFileID primary key,
	[ScenarioRunOutputFileTypeID] int not null constraint FK_ScenarioRunOutputFile_ScenarioRunOutputFileType_ScenarioRunOutputFileTypeID foreign key references dbo.ScenarioRunOutputFileType(ScenarioRunOutputFileTypeID),
	[ScenarioRunID] int not null constraint FK_ScenarioRunOutputFile_ScenarioRun_ScenarioRunID foreign key references dbo.ScenarioRun(ScenarioRunID),
	[FileResourceID] int not null constraint FK_ScenarioRunOutputFile_FileResource_FileResourceID foreign key references dbo.FileResource(FileResourceID),
	CONSTRAINT [AK_ScenarioRunOutputFile_Unique_ScenarioRunOutputFileTypeID_ScenarioRunID] unique ([ScenarioRunOutputFileTypeID], [ScenarioRunID])
)
