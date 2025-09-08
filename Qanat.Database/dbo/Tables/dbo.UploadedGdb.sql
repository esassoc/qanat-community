CREATE TABLE [dbo].[UploadedGdb](
	[UploadedGdbID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_UploadedGdb_UploadedGdbID] PRIMARY KEY,
	[UserID] [int] NOT NULL CONSTRAINT [FK_UploadedGdbID_User_UserID] foreign key references dbo.[User]([UserID]),
	[GeographyID] int not null constraint [FK_UploadedGdb_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[CanonicalName] VARCHAR(100) NOT NULL,
	[UploadDate] [datetime] NOT NULL, 
    [EffectiveYear] INT NULL,
	[Finalized] bit NOT NULL,
	[SRID] int NOT NULL
)
