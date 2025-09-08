create procedure dbo.pWaterTypeSupplyByYearAndGeography
(
	@year int,
	@geographyID int
)
as

select null as WaterAccountID,
       wt.WaterTypeID,
       wt.WaterTypeName,
       sum(ps.TransactionAmount) as TotalSupply,
	   wt.SortOrder,
	   wt.WaterTypeColor,
	   SUM(ps.TransactionAmount) / ISNULL(SUM(CONVERT( DECIMAL(10, 4), p.ParcelArea)), 0) as TotalSupplyDepth

from dbo.ParcelSupply ps
join dbo.WaterType wt on wt.WaterTypeID = ps.WaterTypeID
join dbo.fReportingPeriod(@year) rp on ps.GeographyID = rp.GeographyID and ps.EffectiveDate between rp.StartDate and rp.EndDate
join dbo.WaterAccountParcel wap on ps.ParcelID = wap.ParcelID and wap.ReportingPeriodID = rp.ReportingPeriodID
join dbo.Parcel P on P.ParcelID = wap.ParcelID
where ps.GeographyID = @geographyID
group by wt.WaterTypeName, wt.WaterTypeID, wt.SortOrder, wt.WaterTypeColor
GO