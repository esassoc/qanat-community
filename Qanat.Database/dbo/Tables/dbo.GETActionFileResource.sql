--TODO: DELETE ME, RENAMING GETACTION TO SCENARIORUN--
CREATE TABLE [dbo].[GETActionFileResource]
(
	[GETActionFileResourceID] int not null identity(1,1) constraint PK_GETActionFileResource_GETActionFileResourceID primary key,
	[GETActionID] int not null constraint FK_GETActionFileResource_GETAction_GETActionID foreign key references dbo.GETAction(GETActionID),
	[FileResourceID] int not null constraint FK_GETActionFileResource_FileResource_FileResourceID foreign key references dbo.FileResource(FileResourceID)
)
