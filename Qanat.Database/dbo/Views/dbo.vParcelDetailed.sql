create view dbo.vParcelDetailed
as

select 
    p.GeographyID,
    p.ParcelID,
    p.ParcelNumber,
    p.ParcelArea,
	p.WaterAccountID AS CurrentWaterAccountID,
    p.OwnerName,
    p.OwnerAddress,
    ps.ParcelStatusID,
    ps.ParcelStatusDisplayName,
    wa.WaterAccountID,
    wa.WaterAccountName,
    wa.WaterAccountNumber,
    wa.WaterAccountPIN,
    pca.CustomAttributes,
    pzs.ZoneIDs,
    wop.WellsOnParcel,
    ibw.IrrigatedByWells,
    RP.ReportingPeriodID,
    RP.EndDate as ReportingPeriodEndDate

from dbo.Parcel p
join dbo.ParcelStatus ps on p.ParcelStatusID = ps.ParcelStatusID
left join dbo.ParcelCustomAttribute pca on p.ParcelID = pca.ParcelID
join dbo.ReportingPeriod RP on RP.GeographyID = p.GeographyID -- Join to ReportingPeriod by matching Geography
left join dbo.WaterAccountParcel WAP  on WAP.ParcelID = p.ParcelID and WAP.ReportingPeriodID = RP.ReportingPeriodID -- Left join WaterAccountParcel by (ParcelID, ReportingPeriodID)
left join dbo.WaterAccount wa on wa.WaterAccountID = WAP.WaterAccountID -- WaterAccount is left-joined via WAP
left join 
(
    select 
        pz.ParcelID, 
        STRING_AGG(cast(z.ZoneID as nvarchar(max)), ',') 
            WITHIN GROUP (ORDER BY z.ZoneGroupID) as ZoneIDs
    from dbo.ParcelZone pz
    join dbo.[Zone] z 
        on pz.ZoneID = z.ZoneID
    group by pz.ParcelID
) pzs 
    on p.ParcelID = pzs.ParcelID

left join 
(
    select 
        w.ParcelID, 
        '[' + STRING_AGG(
                CAST(CONCAT(
                    '{"WellID":', w.WellID, 
                    ',"WellName":"', w.WellName, 
                    '"}'
                ) AS NVARCHAR(MAX)), 
                ','
            ) 
        WITHIN GROUP (ORDER BY w.WellID) + ']' AS WellsOnParcel
    from 
    (
        select distinct ParcelID, WellID, WellName 
        from dbo.Well
    ) w
    group by w.ParcelID
) wop 
    on p.ParcelID = wop.ParcelID

left join 
(
    select 
        wir.ParcelID, 
        '[' + STRING_AGG(
                CAST(CONCAT(
                    '{"WellID":', w.WellID, 
                    ',"WellName":"', w.WellName, 
                    '"}'
                ) AS NVARCHAR(MAX)), 
                ','
            ) 
        WITHIN GROUP (ORDER BY w.WellID) + ']' AS IrrigatedByWells
    from dbo.WellIrrigatedParcel wir
    join dbo.Well w 
        on wir.WellID = w.WellID
    group by wir.ParcelID
) ibw 
    on p.ParcelID = ibw.ParcelID

go

/*
select * from dbo.vParcelDetailed
*/