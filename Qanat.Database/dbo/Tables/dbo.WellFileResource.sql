CREATE TABLE [dbo].[WellFileResource]
(
	[WellFileResourceID] int not null identity(1,1) constraint PK_WellFileResource_WellFileResourceID primary key,
	[WellID] int not null constraint FK_WellFileResource_Well_WellID foreign key references dbo.Well(WellID),
	[FileResourceID] int not null constraint FK_WellFileResource_FileResource_FileResourceID foreign key references dbo.FileResource(FileResourceID),
	[FileDescription] varchar(200) null
)
