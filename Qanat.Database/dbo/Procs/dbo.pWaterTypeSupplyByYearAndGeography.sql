create procedure dbo.pWaterTypeSupplyByYearAndGeography
(
	@year int,
	@geographyID int
)
as

select null as WaterAccountID,
       wt.WaterTypeID,
       wt.WaterTypeName,
       sum(ps.TransactionAmount) as TotalSupply

from dbo.ParcelSupply ps
join dbo.WaterType wt on wt.WaterTypeID = ps.WaterTypeID
join dbo.fReportingPeriod(@year) rp on ps.GeographyID = rp.GeographyID and ps.EffectiveDate between rp.StartDate and rp.EndDate
where ps.GeographyID = @geographyID
group by wt.WaterTypeName, wt.WaterTypeID

GO
