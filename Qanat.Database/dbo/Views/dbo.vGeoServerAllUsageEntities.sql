create view dbo.vGeoServerAllUsageEntities
as

select 
	ue.UsageEntityID,
	ue.GeographyID,
	ue.UsageEntityArea,
	ueg.Geometry4326,
	p.ParcelID,
	p.ParcelArea,
	p.WaterAccountID
FROM dbo.UsageEntity ue
join dbo.Parcel p on p.ParcelID = ue.ParcelID
join dbo.UsageEntityGeometry ueg on ue.UsageEntityID = ueg.UsageEntityID

GO
/*
select * from dbo.vGeoServerAllUsageEntities
*/