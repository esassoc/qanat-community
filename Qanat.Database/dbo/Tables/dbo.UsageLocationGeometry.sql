CREATE TABLE [dbo].[UsageLocationGeometry]
(
	[UsageLocationID]	INT			NOT NULL,
	[GeometryNative]	GEOMETRY	NOT NULL,
	[Geometry4326]		GEOMETRY	NOT NULL,

	CONSTRAINT [PK_UsageLocationGeometry_UsageLocationID]					PRIMARY KEY([UsageLocationID]),
	CONSTRAINT [FK_UsageLocationGeometry_UsageLocation_UsageLocationID]		FOREIGN KEY([UsageLocationID]) references dbo.[UsageLocation]([UsageLocationID]) ON DELETE CASCADE,
)
GO

CREATE INDEX IX_UsageLocationGeometry_UsageLocationID on dbo.UsageLocationGeometry(UsageLocationID);
GO

CREATE SPATIAL INDEX SPATIAL_UsageLocationGeometry_Geometry4326 ON dbo.UsageLocationGeometry(Geometry4326)
USING GEOMETRY_AUTO_GRID WITH (BOUNDING_BOX =(-124.409591, 32.534156, -114.131211, 42.009518), CELLS_PER_OBJECT = 8);        
GO