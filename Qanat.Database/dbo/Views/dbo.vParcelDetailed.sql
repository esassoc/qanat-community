create view dbo.vParcelDetailed
as

select p.GeographyID, p.ParcelID, p.ParcelNumber, p.ParcelArea, p.OwnerName, p.OwnerAddress
        , ps.ParcelStatusID, ps.ParcelStatusDisplayName
        , wa.WaterAccountID, wa.WaterAccountName, wa.WaterAccountNumber, wa.WaterAccountPIN
        , pca.CustomAttributes, pzs.Zones
		, wop.WellsOnParcel, ibw.IrrigatedByWells

from dbo.Parcel p
join dbo.ParcelStatus ps on p.ParcelStatusID = ps.ParcelStatusID
left join dbo.ParcelCustomAttribute pca on p.ParcelID = pca.ParcelID
left join dbo.WaterAccount wa on p.WaterAccountID = wa.WaterAccountID
left join 
(
    select ParcelID, '[' + STRING_AGG(cast(concat('{"ZoneID":', z.ZoneID, ',"ZoneGroupID":', z.ZoneGroupID, ',"ZoneName":"', z.ZoneName, '", "ZoneColor":"', z.ZoneColor, '", "ZoneAccentColor":"', z.ZoneAccentColor, '"}') as nvarchar(max)), ',') WITHIN GROUP (ORDER BY z.ZoneID) + ']' as Zones
    from dbo.ParcelZone pz
    join dbo.[Zone] z on pz.ZoneID = z.ZoneID
    group by pz.ParcelID
) pzs on p.ParcelID = pzs.ParcelID

LEFT JOIN (
    SELECT 
        w.ParcelID, 
        '[' + STRING_AGG(CAST(CONCAT(
            '{"WellID":', w.WellID, 
            ',"WellName":"', w.WellName, 
            '"}') AS NVARCHAR(MAX)), ',') 
        WITHIN GROUP (ORDER BY w.WellID) + ']' AS WellsOnParcel
    FROM (
        SELECT DISTINCT ParcelID, WellID, WellName 
        FROM dbo.Well
    ) w
    GROUP BY w.ParcelID
) wop ON p.ParcelID = wop.ParcelID
LEFT JOIN (
    SELECT 
        wir.ParcelID, 
        '[' + STRING_AGG(CAST(CONCAT(
            '{"WellID":', w.WellID, 
            ',"WellName":"', w.WellName, 
            '"}') AS NVARCHAR(MAX)), ',') 
        WITHIN GROUP (ORDER BY w.WellID) + ']' AS IrrigatedByWells
    FROM dbo.WellIrrigatedParcel wir
    JOIN dbo.Well w ON wir.WellID = w.WellID
    GROUP BY wir.ParcelID
) ibw ON p.ParcelID = ibw.ParcelID
GO

/*
select * from dbo.vParcelDetailed
*/