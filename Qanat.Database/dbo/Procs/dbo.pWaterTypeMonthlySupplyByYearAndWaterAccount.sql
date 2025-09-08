create procedure dbo.pWaterTypeMonthlySupplyByYearAndWaterAccount
(
	@year int,
	@waterAccountID int
)
as

drop table if exists #waterAccountParcels
select * into #waterAccountParcels
from dbo.fWaterAccountParcelByWaterAccountAndYear(@waterAccountID, @year)

drop table if exists #effectiveDates
SELECT dateadd(month, EffectiveMonth, rp.StartDate) as EffectiveDate
into #effectiveDates
FROM (
    select 0 as EffectiveMonth union select 1 union select 2 union select 3 union select 4 union select 5 union select 6 union select 7 union select 8 union select 9 union select 10 union select 11
) months
cross join dbo.fReportingPeriod(@year) rp
join dbo.WaterAccount wa on wa.WaterAccountID = @waterAccountID and rp.GeographyID = wa.GeographyID


declare @totalParcelArea float;

set @totalparcelarea = (SELECT SUM(ISNULL(P.ParcelArea, 0)) 
FROM #waterAccountParcels wap
JOIN dbo.Parcel P ON P.ParcelID = wap.ParcelID)

select WaterAccountID, EffectiveDate,
	wawted.WaterTypeID, wawted.WaterTypeName, wawted.WaterTypeColor, wawted.WaterTypeSortOrder, wawted.WaterTypeDefinition,
	CurrentSupplyAmount,
	sum(CurrentSupplyAmount) over(partition by wawted.WaterTypeID order by wawted.EffectiveDate rows unbounded preceding) as CurrentCumulativeSupplyAmount,
	CASE 
		WHEN @totalParcelArea IS NULL THEN 0 
		ELSE (SUM(CurrentSupplyAmount) over(partition by wawted.WaterTypeID order by wawted.EffectiveDate rows unbounded preceding)) / @totalParcelArea
	END as CurrentCumulativeSupplyAmountDepth
from (
	select wa.WaterAccountID, ed.EffectiveDate,
		wt.WaterTypeID, wt.WaterTypeName, wt.WaterTypeColor, wt.SortOrder as WaterTypeSortOrder, wt.WaterTypeDefinition
	from dbo.WaterAccount wa
	join dbo.WaterType wt on wa.GeographyID = wt.GeographyID
	cross join #effectiveDates ed
	where WaterAccountID = @waterAccountID and wt.IsActive = 1
) wawted
left join (
	select ps.WaterTypeID, month(ps.EffectiveDate) as EffectiveMonth, sum(ps.TransactionAmount) as CurrentSupplyAmount, sum(ps.TransactionAmount / p.ParcelArea) as CurrentSupplyAmountDepth
	from dbo.ParcelSupply ps
	join #waterAccountParcels wap on ps.ParcelID = wap.ParcelID
	join dbo.Parcel p on ps.ParcelID = p.ParcelID
	join dbo.fReportingPeriod(@year) rp on ps.GeographyID = rp.GeographyID and ps.EffectiveDate between rp.StartDate and rp.EndDate
	group by ps.WaterTypeID, month(ps.EffectiveDate)
) mps on month(wawted.EffectiveDate) = mps.EffectiveMonth and wawted.WaterTypeID = mps.WaterTypeID
order by wawted.EffectiveDate, wawted.WaterTypeID

GO
