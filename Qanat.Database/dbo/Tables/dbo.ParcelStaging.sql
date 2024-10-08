CREATE TABLE [dbo].[ParcelStaging](
	[ParcelStagingID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ParcelStaging_ParcelStagingID] PRIMARY KEY,
	[GeographyID] [int] NOT NULL constraint [FK_ParcelStaging_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[ParcelNumber] [varchar](20) NOT NULL,
	[Geometry] [geometry] NULL,
	[OwnerName] [varchar](200) NOT NULL,
	[Geometry4326] [geometry] NULL,
	[OwnerAddress] [varchar](200) NOT NULL,
	[HasConflict] [bit] NOT NULL,
)
