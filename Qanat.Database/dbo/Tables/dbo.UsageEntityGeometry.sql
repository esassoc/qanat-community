CREATE TABLE [dbo].[UsageEntityGeometry]
(
	[UsageEntityID] [int] NOT NULL constraint PK_UsageEntityGeometry_UsageEntityID PRIMARY KEY,
	[GeometryNative] [geometry] NOT NULL,
	[Geometry4326] [geometry] NOT NULL,
	constraint [FK_UsageEntityGeometry_UsageEntity_UsageEntityID] foreign key ([UsageEntityID]) references dbo.[UsageEntity]([UsageEntityID]) ON DELETE CASCADE,
)
GO

CREATE SPATIAL INDEX SPATIAL_UsageEntityGeometry_Geometry4326 ON dbo.UsageEntityGeometry (Geometry4326)
USING  GEOMETRY_AUTO_GRID WITH (BOUNDING_BOX =(-124.409591, 32.534156, -114.131211, 42.009518), CELLS_PER_OBJECT = 8)              