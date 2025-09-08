create view dbo.vGeoserverGeographyGSABoundaries
as

select          g.GeographyID as PrimaryKey,
                g.GeographyID,
                g.GeographyName,
                g.GeographyDisplayName,
                gb.GSABoundary,
                g.Color as GeographyColor
                
FROM        dbo.Geography g
join        dbo.GeographyBoundary gb on g.GeographyID = gb.GeographyID

GO
/*
select * from dbo.vGeoserverGeographyGSABoundaries
*/