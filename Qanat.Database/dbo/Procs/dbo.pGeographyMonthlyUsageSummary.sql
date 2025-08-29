create procedure dbo.pGeographyMonthlyUsageSummary
(
    @geographyID int,
    @year int
)
as

begin

drop table if exists #waterAccountParcels
select p.ParcelID, p.ParcelNumber, cast(p.ParcelArea as decimal(20, 4)) as ParcelArea, cast(sum(ul.Area) as decimal(20, 4)) as UsageLocationArea
into #waterAccountParcels
from dbo.Parcel p
join dbo.fWaterAccountParcelByGeographyAndYear(@geographyID, @year) wap on p.ParcelID = wap.ParcelID
join dbo.UsageLocation ul on p.ParcelID = ul.ParcelID AND UL.ReportingPeriodID = wap.ReportingPeriodID --MK 2/18/2025: Filter out parcels without usage locations.
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

declare @WaterMeasurementTypeID int, @WaterMeasurementTypeName varchar(100), @WaterMeasurementCategoryTypeName varchar(100), @WaterMeasurementTypeSortOrder int
select @WaterMeasurementTypeID = WaterMeasurementTypeID, @WaterMeasurementTypeName = WaterMeasurementTypeName, @WaterMeasurementCategoryTypeName = wmct.WaterMeasurementCategoryTypeDisplayName, @WaterMeasurementTypeSortOrder = wmt.SortOrder
from dbo.[Geography] g 
join dbo.WaterMeasurementType wmt on g.SourceOfRecordWaterMeasurementTypeID = wmt.WaterMeasurementTypeID
join dbo.WaterMeasurementCategoryType wmct on wmt.WaterMeasurementCategoryTypeID = wmct.WaterMeasurementCategoryTypeID
where g.GeographyID = @geographyID


select m.ParcelID, m.ParcelNumber, m.ParcelArea, @WaterMeasurementTypeID as WaterMeasurementTypeID, @WaterMeasurementTypeName as WaterMeasurementTypeName, @WaterMeasurementCategoryTypeName as WaterMeasurementCategoryTypeName, @WaterMeasurementTypeSortOrder as WaterMeasurementTypeSortOrder, m.EffectiveDate
, u.CurrentUsageAmount
, CASE WHEN m.ParcelArea != 0 THEN u.CurrentUsageAmount / m.ParcelArea ELSE NULL END as CurrentUsageAmountDepth
, uavg.AverageUsageAmount
, CASE WHEN m.ParcelArea != 0 THEN uavg.AverageUsageAmount / m.ParcelArea ELSE NULL END as AverageUsageAmountDepth
, case when CurrentUsageAmount is null then null else sum(CurrentUsageAmount) over(partition by m.ParcelID order by m.EffectiveDate rows unbounded preceding) end as CurrentCumulativeUsageAmount
, case when AverageUsageAmount is null then null else sum(AverageUsageAmount) over(partition by m.ParcelID order by m.EffectiveDate rows unbounded preceding) end as AverageCumulativeUsageAmount
, m.UsageLocationArea
from 
(
    select ParcelID, ParcelNumber, ParcelArea, UsageLocationArea, EffectiveDate
    from 
    #waterAccountParcels
    cross join #effectiveDates
) m
left join (
	select ParcelID, WaterMeasurementTypeID, EffectiveMonth, CASE WHEN count(EffectiveYear) != 0 THEN sum(UsageSum) / count(EffectiveYear) ELSE NULL END as AverageUsageAmount
	from (
		select wap.ParcelID, wmsor.WaterMeasurementTypeID, month(wmsor.ReportedDate) as EffectiveMonth, year(wmsor.ReportedDate) as EffectiveYear, sum(wmsor.ReportedValueInAcreFeet) as UsageSum
		from dbo.vWaterMeasurementSourceOfRecord wmsor
		join #waterAccountParcels wap on wmsor.ParcelID = wap.ParcelID
		group by wap.ParcelID, wmsor.WaterMeasurementTypeID, month(wmsor.ReportedDate), year(wmsor.ReportedDate)
	) plmy
	group by ParcelID, WaterMeasurementTypeID, EffectiveMonth
) uavg on m.ParcelID = uavg.ParcelID and month(m.EffectiveDate) = uavg.EffectiveMonth
left join (
	select wap.ParcelID, wmsor.WaterMeasurementTypeID, cast(concat(month(wmsor.ReportedDate), '/1/', year(wmsor.ReportedDate)) as DateTime) as EffectiveDate, sum(wmsor.ReportedValueInAcreFeet) as CurrentUsageAmount
	from dbo.vWaterMeasurementSourceOfRecord wmsor
    join #reportingPeriods rp on wmsor.GeographyID = rp.GeographyID and wmsor.ReportedDate between rp.StartDate and rp.EndDate
	join #waterAccountParcels wap on wmsor.ParcelID = wap.ParcelID
    group by wap.ParcelID, wmsor.WaterMeasurementTypeID, cast(concat(month(wmsor.ReportedDate), '/1/', year(wmsor.ReportedDate)) as DateTime)
) u on m.ParcelID = u.ParcelID and m.EffectiveDate = u.EffectiveDate
order by WaterMeasurementTypeSortOrder, m.EffectiveDate, m.ParcelID

end