CREATE TABLE [dbo].[ScenarioRunFileResource]
(
	[ScenarioRunFileResourceID] int not null identity(1,1) constraint PK_ScenarioRunFileResource_ScenarioRunFileResourceID primary key,
	[ScenarioRunID] int not null constraint FK_ScenarioRunFileResource_ScenarioRun_ScenarioRunID foreign key references dbo.ScenarioRun(ScenarioRunID),
	[FileResourceID] int not null constraint FK_ScenarioRunFileResource_FileResource_FileResourceID foreign key references dbo.FileResource(FileResourceID)
)
