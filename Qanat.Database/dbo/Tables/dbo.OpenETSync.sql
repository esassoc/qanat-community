CREATE TABLE [dbo].[OpenETSync](
	[OpenETSyncID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_OpenETSync_OpenETSyncID] PRIMARY KEY,
	[GeographyID] int not null constraint [FK_OpenETSync_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),	
	OpenETDataTypeID int not null constraint [FK_OpenETSync_OpenETDataType_OpenETDataTypeID] foreign key references dbo.OpenETDataType(OpenETDataTypeID),
	[Year] int NOT NULL,
	[Month] int not null,
	FinalizeDate datetime null
)