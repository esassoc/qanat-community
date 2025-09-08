CREATE TABLE [dbo].[UploadedWellGdb](
	[UploadedWellGdbID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_UploadedWellGdb_UploadedWellGdbID] PRIMARY KEY,
	[UserID] [int] NOT NULL CONSTRAINT [FK_UploadedWellGdbID_User_UserID] foreign key references dbo.[User]([UserID]),
	[GeographyID] int not null constraint [FK_UploadedWellGdb_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[CanonicalName] VARCHAR(100) NOT NULL,
	[UploadDate] [datetime] NOT NULL, 
    [EffectiveDate] DATETIME NULL,
	[Finalized] bit NOT NULL,
	[SRID] int NOT NULL default 2227
)
