--TODO: DELETE ME, RENAMING GETACTION TO SCENARIORUN--
CREATE TABLE [dbo].[GETActionOutputFile]
(
	[GETActionOutputFileID] int not null identity(1,1) constraint PK_GETActionOutputFile_GETActionOutputFileID primary key,
	[GETActionOutputFileTypeID] int not null constraint FK_GETActionOutputFile_GETActionOutputFileType_GETActionOutputFileTypeID foreign key references dbo.GETActionOutputFileType(GETActionOutputFileTypeID),
	[GETActionID] int not null constraint FK_GETActionOutputFile_GETAction_GETActionID foreign key references dbo.GETAction(GETActionID),
	[FileResourceID] int not null constraint FK_GETActionOutputFile_FileResource_FileResourceID foreign key references dbo.FileResource(FileResourceID),
	CONSTRAINT [AK_GETActionOutputFile_Unique_GETActionOutputFileTypeID_GETActionID] unique ([GETActionOutputFileTypeID], [GETActionID])
)
