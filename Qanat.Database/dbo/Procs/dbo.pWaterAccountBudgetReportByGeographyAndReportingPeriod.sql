create procedure dbo.pWaterAccountBudgetReportByGeographyAndReportingPeriod
(
    @geographyID int,
    @reportingPeriodID int
)
as

begin

    drop table if exists #waterAccountParcels
    select wap.WaterAccountParcelID, wap.WaterAccountID, p.ParcelID, p.ParcelArea, rp.ReportingPeriodID, rp.StartDate, rp.EndDate
    into #waterAccountParcels
    from dbo.WaterAccountParcel wap
    join dbo.Parcel p on wap.ParcelID = p.ParcelID
    join dbo.ReportingPeriod rp on wap.ReportingPeriodID = rp.ReportingPeriodID
    where wap.GeographyID = @geographyID and wap.ReportingPeriodID = @reportingPeriodID

    drop table if exists #parcelSupplies
    select wap.WaterAccountID,
        wt.WaterTypeID,
	    isnull(sum(ps.TransactionAmount), 0) as TotalSupply
    into #parcelSupplies
    from #waterAccountParcels wap
    cross join dbo.WaterType wt
    left join dbo.ParcelSupply ps on wt.WaterTypeID = ps.WaterTypeID and ps.ParcelID = wap.ParcelID and ps.EffectiveDate between wap.StartDate and wap.EndDate
    where wt.GeographyID = @geographyID
    group by wap.WaterAccountID, wt.WaterTypeID

    drop table if exists #zoneData
    select WaterAccountID, 
            STRING_AGG(cast(z.ZoneID as nvarchar(max)), ',') WITHIN GROUP (ORDER BY z.ZoneGroupID) as ZoneIDs
    into #zoneData
    from 
    (
        -- DISTINCT ensures we don't duplicate the same zone for a water account
        select distinct wap.WaterAccountID, pz.ZoneID
        from #waterAccountParcels wap
        join dbo.ParcelZone pz on wap.ParcelID = pz.ParcelID
    ) distinctZones
    join dbo.[Zone] z on distinctZones.ZoneID = z.ZoneID
    group by distinctZones.WaterAccountID

    drop table if exists #waterAccounts
    select wacc.WaterAccountID, wacc.GeographyID, wacc.WaterAccountNumber, wacc.WaterAccountName, 0 as ParcelCount, CAST(0 as float) ParcelArea, sum(isnull(ul.Area, 0)) as UsageLocationArea,
    cast(0 as float) as TotalSupply, cast(0 as float) as UsageToDate,
    cast(null as varchar(max)) as ZoneIDs, cast(null as varchar(max)) as WaterTypeSupplyBreakdown
    into #waterAccounts
    from dbo.WaterAccount wacc
    join #waterAccountParcels wap on wacc.WaterAccountID = wap.WaterAccountID AND wap.ReportingPeriodID = @reportingPeriodID
    left join dbo.UsageLocation ul on wap.ParcelID = ul.ParcelID and wap.ReportingPeriodID = ul.ReportingPeriodID
    group by wacc.WaterAccountID, wacc.GeographyID, wacc.WaterAccountNumber, wacc.WaterAccountName;

    -- fill in the parcel count and area
	with ParcelAgg as (
		select 
			wap.WaterAccountID,
			count(distinct wap.ParcelID) as ParcelCount,
			sum(isnull(wap.ParcelArea, 0)) as ParcelArea
		from #waterAccountParcels wap
		group by wap.WaterAccountID
	)
	update wacc
	set 
		wacc.ParcelCount = pa.ParcelCount,
		wacc.ParcelArea = pa.ParcelArea
	from #waterAccounts wacc
	join ParcelAgg pa on wacc.WaterAccountID = pa.WaterAccountID;

    -- because the left joins slow the query down quite a bit, we need to use update queries on #waterAccounts to fill in the optional data
    update wacc
    set wacc.UsageToDate = v.UsageToDate
    from #waterAccounts wacc
    join
    (
	    select wap.WaterAccountID, sum(wmsor.ReportedValueInAcreFeet) as UsageToDate
	    from dbo.vWaterMeasurementSourceOfRecord wmsor
	    join #waterAccountParcels wap on wmsor.ParcelID = wap.ParcelID and wmsor.ReportedDate between wap.StartDate and wap.EndDate
	    group by wap.WaterAccountID
    ) v on wacc.WaterAccountID = v.WaterAccountID

    update wacc
    set wacc.WaterTypeSupplyBreakdown = v.WaterTypeSupplyBreakdown, wacc.TotalSupply = v.TotalSupply
    from #waterAccounts wacc
    join
    (
	    select WaterAccountID,
        '{' + STRING_AGG(cast(concat('"', WaterTypeID, '": ', TotalSupply) as nvarchar(max)), ', ') WITHIN GROUP (ORDER BY WaterTypeID) + '}' as WaterTypeSupplyBreakdown,
		    sum(TotalSupply) as TotalSupply
	    from #parcelSupplies ps
	    group by WaterAccountID
    ) v on wacc.WaterAccountID = v.WaterAccountID
    
    update wacc
    set wacc.ZoneIDs = zd.ZoneIDs
    from #waterAccounts wacc
    join #zoneData zd on wacc.WaterAccountID = zd.WaterAccountID

    select WaterAccountID, GeographyID, WaterAccountNumber, WaterAccountName, ParcelCount, ParcelArea, UsageLocationArea,
    TotalSupply, UsageToDate, TotalSupply - UsageToDate as CurrentAvailable,
    ZoneIDs, WaterTypeSupplyBreakdown
    from #waterAccounts
    order by WaterAccountID, GeographyID, WaterAccountNumber, WaterAccountName

end


--drop procedure pWaterAccountBudgetReportByGeographyAndReportingPeriod