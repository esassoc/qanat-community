CREATE TABLE [dbo].[ParcelGeometry]
(
	[ParcelGeometryID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ParcelGeometry_ParcelGeometryID] PRIMARY KEY,
	[GeographyID] int not null constraint [FK_ParcelGeometry_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[ParcelID] int not null constraint [FK_ParcelGeometry_Parcel_ParcelID] foreign key references dbo.[Parcel]([ParcelID]),
	[GeometryNative] [geometry] NOT NULL,
	[Geometry4326] [geometry] NULL,
	CONSTRAINT AK_ParcelGeometry_ParcelID unique (ParcelID)
)
GO

CREATE SPATIAL INDEX SPATIAL_ParcelGeometry_Geometry4326 ON dbo.ParcelGeometry (Geometry4326)
USING  GEOMETRY_AUTO_GRID WITH (BOUNDING_BOX =(-124.409591, 32.534156, -114.131211, 42.009518), CELLS_PER_OBJECT = 8)              