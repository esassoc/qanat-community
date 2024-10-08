CREATE TABLE [dbo].[WellRegistrationFileResource]
(
	[WellRegistrationFileResourceID] int not null identity(1,1) constraint PK_WellRegistrationFileResource_WellRegistrationFileResourceID primary key,
	[WellRegistrationID] int not null constraint FK_WellRegistrationFileResource_WellRegistration_WellRegistrationID foreign key references dbo.WellRegistration(WellRegistrationID),
	[FileResourceID] int not null constraint FK_WellRegistrationFileResource_FileResource_FileResourceID foreign key references dbo.FileResource(FileResourceID),
	[FileDescription] varchar(200) null
)
