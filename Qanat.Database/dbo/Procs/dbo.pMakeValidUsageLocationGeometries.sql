create procedure dbo.pMakeValidUsageLocationGeometries
with execute as owner
as

begin
	update dbo.[UsageLocationGeometry] set Geometry4326 = Geometry4326.MakeValid()
	where Geometry4326.STIsValid() = 0

	update dbo.[UsageLocationGeometry] set GeometryNative = GeometryNative.MakeValid()
	where GeometryNative.STIsValid() = 0
	
end