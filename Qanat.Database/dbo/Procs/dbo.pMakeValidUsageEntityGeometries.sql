create procedure dbo.pMakeValidUsageEntityGeometries
with execute as owner
as

begin
	update dbo.[UsageEntityGeometry] set Geometry4326 = Geometry4326.MakeValid()
	where Geometry4326.STIsValid() = 0

	update dbo.[UsageEntityGeometry] set GeometryNative = GeometryNative.MakeValid()
	where GeometryNative.STIsValid() = 0
	
end