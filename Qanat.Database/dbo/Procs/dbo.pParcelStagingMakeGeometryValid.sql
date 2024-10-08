create procedure dbo.pParcelStagingMakeGeometryValid
as

begin
	update dbo.ParcelStaging
	set [Geometry] = [Geometry].MakeValid()
    where [Geometry].STIsValid() = 0

    update dbo.ParcelStaging
	set [Geometry4326] = [Geometry4326].MakeValid()
    where [Geometry4326].STIsValid() = 0
end 
