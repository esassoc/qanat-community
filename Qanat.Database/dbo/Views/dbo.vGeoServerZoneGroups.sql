create view dbo.vGeoServerZoneGroups
as
select          p.ParcelID,
				zg.ZoneGroupID,
				z.ZoneID,
                p.GeographyID,
                p.ParcelNumber,
                p.ParcelArea,
                pg.Geometry4326 as ParcelGeometry,
				z.ZoneColor
                
FROM        dbo.ZoneGroup zg
join dbo.[Zone] z on zg.ZoneGroupID = z.ZoneGroupID
join dbo.ParcelZone pz on z.ZoneID = pz.ZoneID
join dbo.Parcel p on pz.ParcelID = p.ParcelID
join dbo.ParcelGeometry pg on p.ParcelID = pg.ParcelID

GO
/*
select * from dbo.vGeoServerZoneGroups
*/