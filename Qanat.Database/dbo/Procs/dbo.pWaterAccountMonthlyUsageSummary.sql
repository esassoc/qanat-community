create procedure dbo.pWaterAccountMonthlyUsageSummary
(
	@waterAccountID int,
    @year int,
    @userID int
)
as

declare @geographyID int
select @geographyID = GeographyID from dbo.WaterAccount where WaterAccountID = @waterAccountID

declare @filterForLandOwner bit = 0
if exists(select 1 from dbo.WaterAccountUser where WaterAccountID = @waterAccountID and UserID = @userID)
begin
    set @filterForLandOwner = 1
end

drop table if exists #waterAccountParcels
select p.ParcelID, p.ParcelNumber, cast(p.ParcelArea as decimal(20, 4)) as ParcelArea, cast(sum(ul.Area) as decimal(20, 4)) as UsageLocationArea
into #waterAccountParcels
from dbo.Parcel p
join dbo.fWaterAccountParcelByWaterAccountAndYear(@waterAccountID, @year) wap on p.ParcelID = wap.ParcelID
join dbo.UsageLocation ul on p.ParcelID = ul.ParcelID --MK 2/18/2025: This used to be a left join, but was causing issues on the Water Budget page, specifically the cumulative chart. Only considering parcels with usage locations.
join dbo.ReportingPeriod rp on rp.ReportingPeriodID = ul.ReportingPeriodID 
where YEAR(rp.EndDate) = @year AND P.GeographyID = @geographyID
group by p.ParcelID, p.ParcelNumber, p.ParcelArea

drop table if exists #reportingPeriods
select GeographyID, StartDate, EndDate
into #reportingPeriods
from dbo.fReportingPeriod(@year)
where GeographyID = @geographyID

drop table if exists #effectiveDates
SELECT dateadd(month, EffectiveMonth, rp.StartDate) as EffectiveDate
into #effectiveDates
FROM (
    select 0 as EffectiveMonth union select 1 union select 2 union select 3 union select 4 union select 5 union select 6 union select 7 union select 8 union select 9 union select 10 union select 11
) months
cross join #reportingPeriods rp

select m.ParcelID, m.ParcelNumber, m.ParcelArea, m.WaterMeasurementTypeID, m.WaterMeasurementTypeName, m.WaterMeasurementCategoryTypeName, m.WaterMeasurementTypeSortOrder, m.EffectiveDate
, u.CurrentUsageAmount, u.CurrentUsageAmount / m.UsageLocationArea as CurrentUsageAmountDepth
, uavg.AverageUsageAmount, uavg.AverageUsageAmount / m.UsageLocationArea as AverageUsageAmountDepth
, case when CurrentUsageAmount is null then null else sum(CurrentUsageAmount) over(partition by m.ParcelID, m.ParcelNumber, m.WaterMeasurementTypeID order by m.EffectiveDate rows unbounded preceding) end as CurrentCumulativeUsageAmount
, case when AverageUsageAmount is null then null else sum(AverageUsageAmount) over(partition by m.ParcelID, m.ParcelNumber, m.WaterMeasurementTypeID order by m.EffectiveDate rows unbounded preceding) end as AverageCumulativeUsageAmount
, m.UsageLocationArea
from 
(
    select ParcelID, ParcelNumber, ParcelArea, UsageLocationArea, EffectiveDate, WaterMeasurementTypeID, WaterMeasurementTypeName, WaterMeasurementCategoryTypeDisplayName as WaterMeasurementCategoryTypeName, WaterMeasurementTypeSortOrder
    from 
    #waterAccountParcels
    cross join #effectiveDates
    cross join 
    (
        select WaterMeasurementTypeID, WaterMeasurementTypeName, wmct.WaterMeasurementCategoryTypeDisplayName, wmt.SortOrder as WaterMeasurementTypeSortOrder
        from dbo.WaterMeasurementType wmt
        join dbo.WaterMeasurementCategoryType wmct on wmt.WaterMeasurementCategoryTypeID = wmct.WaterMeasurementCategoryTypeID
        where GeographyID = @geographyID and (wmt.ShowToLandowner = 1 or @filterForLandOwner = 0)
    ) wmts
) m
left join
(
	select ParcelID, WaterMeasurementTypeID, EffectiveMonth, sum(UsageSum) / count(EffectiveYear) as AverageUsageAmount
	from (
		select wap.ParcelID, wmsor.WaterMeasurementTypeID, month(wmsor.ReportedDate) as EffectiveMonth, year(wmsor.ReportedDate) as EffectiveYear, sum(wmsor.ReportedValueInAcreFeet) as UsageSum
		from dbo.vWaterMeasurement wmsor
		join #waterAccountParcels wap on wmsor.ParcelID = wap.ParcelID
		where wmsor.GeographyID = @geographyID
		group by wap.ParcelID, wmsor.WaterMeasurementTypeID, month(wmsor.ReportedDate), year(wmsor.ReportedDate)
	) umy
	group by umy.ParcelID, umy.WaterMeasurementTypeID, umy.EffectiveMonth
) uavg on m.ParcelID = uavg.ParcelID and m.WaterMeasurementTypeID = uavg.WaterMeasurementTypeID and month(m.EffectiveDate) = uavg.EffectiveMonth
left join (
	select wap.ParcelID, wmsor.WaterMeasurementTypeID, cast(concat(month(wmsor.ReportedDate), '/1/', year(wmsor.ReportedDate)) as DateTime) as EffectiveDate, sum(wmsor.ReportedValueInAcreFeet) as CurrentUsageAmount
	from dbo.vWaterMeasurement wmsor	
    join #reportingPeriods rp on wmsor.GeographyID = rp.GeographyID and wmsor.ReportedDate between rp.StartDate and rp.EndDate
	join #waterAccountParcels wap on wmsor.ParcelID = wap.ParcelID
	where wmsor.GeographyID = @geographyID
	group by wap.ParcelID, wmsor.WaterMeasurementTypeID, cast(concat(month(wmsor.ReportedDate), '/1/', year(wmsor.ReportedDate)) as DateTime)
) u on m.ParcelID = u.ParcelID and m.WaterMeasurementTypeID = u.WaterMeasurementTypeID and m.EffectiveDate = u.EffectiveDate
order by m.WaterMeasurementTypeSortOrder, m.EffectiveDate

GO