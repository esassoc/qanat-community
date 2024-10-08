create procedure dbo.pWaterTypeSupplyByYearAndWaterAccount
(
	@year int,
	@waterAccountID int
)
as

select wap.WaterAccountID,
       wt.WaterTypeID,
       wt.WaterTypeName,
       sum(ps.TransactionAmount) as TotalSupply

from dbo.ParcelSupply ps
join dbo.WaterType wt on wt.WaterTypeID = ps.WaterTypeID
join dbo.fReportingPeriod(@year) rp on ps.GeographyID = rp.GeographyID and ps.EffectiveDate between rp.StartDate and rp.EndDate
join dbo.fWaterAccountParcelByWaterAccountAndYear(@waterAccountID, @year) wap on ps.ParcelID = wap.ParcelID
group by wt.WaterTypeName, wt.WaterTypeID, wap.WaterAccountID

GO
