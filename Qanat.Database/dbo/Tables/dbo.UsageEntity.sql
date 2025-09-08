CREATE TABLE [dbo].[UsageEntity]
(
	[UsageEntityID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_UsageEntity_UsageEntityID] PRIMARY KEY,
	[ParcelID] int not null constraint [FK_UsageEntity_Parcel_ParcelID] foreign key references dbo.[Parcel]([ParcelID]),
	[GeographyID] int not null constraint [FK_UsageEntity_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[UsageEntityName] [varchar](100) NOT NULL,
	[UsageEntityArea] [float] NOT NULL,
	CONSTRAINT [AK_UsageEntity_UsageEntityName_GeographyID] UNIQUE (UsageEntityName, GeographyID)
)
GO

