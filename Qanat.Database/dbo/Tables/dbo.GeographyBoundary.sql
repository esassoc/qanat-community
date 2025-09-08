CREATE TABLE [dbo].[GeographyBoundary](
	[GeographyBoundaryID] [int] NOT NULL CONSTRAINT [PK_GeographyBoundary_GeographyBoundaryID] PRIMARY KEY,
	[GeographyID] INT NOT NULL CONSTRAINT AK_GeographyBoundary_GeographyID UNIQUE(GeographyID) CONSTRAINT [FK_GeographyBoundary_Geography_GeographyID] FOREIGN KEY REFERENCES dbo.[Geography](GeographyID),
	[BoundingBox] geometry not null,
	[GSABoundary] GEOMETRY NULL,
	[GSABoundaryLastUpdated] DATETIME NULL
)
