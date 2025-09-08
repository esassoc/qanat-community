create procedure dbo.pMakeValidGeographyBoundaries
with execute as owner
as

begin
	update dbo.[GeographyBoundary] set GSABoundary = GSABoundary.MakeValid()
	where GSABoundary.STIsValid() = 0
	
end