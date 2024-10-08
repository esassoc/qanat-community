create view dbo.vGeoServerAllParcels
as

select 
	p.ParcelID,
	p.GeographyID,
	p.ParcelNumber,
	p.ParcelArea,
	pg.Geometry4326 as ParcelGeometry,
	p.WaterAccountID,
    p.ParcelStatusID
FROM dbo.Parcel p 
join dbo.ParcelGeometry pg on p.ParcelID = pg.ParcelID

GO
/*
select * from dbo.vGeoServerAllParcels
*/