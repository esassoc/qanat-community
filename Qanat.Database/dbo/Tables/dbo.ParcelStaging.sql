CREATE TABLE [dbo].[ParcelStaging](
	[ParcelStagingID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ParcelStaging_ParcelStagingID] PRIMARY KEY,
	[GeographyID] [int] NOT NULL constraint [FK_ParcelStaging_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[ParcelNumber] [varchar](64) NOT NULL,
	[Geometry] [geometry] NULL,
	[OwnerName] [varchar](200) NOT NULL,
	[Geometry4326] [geometry] NULL,
	[OwnerAddress] [varchar](200) NOT NULL,
	[Acres] FLOAT NOT NULL,
	[HasConflict] [bit] NOT NULL,
)
GO

CREATE INDEX IX_ParcelStaging_GeographyID on dbo.ParcelStaging(GeographyID);
GO