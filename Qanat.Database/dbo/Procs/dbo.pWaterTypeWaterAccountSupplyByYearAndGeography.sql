create procedure dbo.pWaterTypeWaterAccountSupplyByYearAndGeography
(
	@year int,
	@geographyID int
)
as

select wap.WaterAccountID as WaterAccountID,
       wt.WaterTypeID,
       wt.WaterTypeName,
       sum(ps.TransactionAmount) as TotalSupply,
	   wt.SortOrder

from dbo.ParcelSupply ps
join dbo.fReportingPeriod(@year) rp on ps.GeographyID = rp.GeographyID and ps.EffectiveDate between rp.StartDate and rp.EndDate
join dbo.WaterAccountParcel wap on ps.ParcelID = wap.ParcelID and wap.ReportingPeriodID = rp.ReportingPeriodID
join dbo.WaterType wt on wt.WaterTypeID = ps.WaterTypeID
where ps.GeographyID = @geographyID
group by wap.WaterAccountID, wt.WaterTypeName, wt.WaterTypeID, wt.SortOrder

GO