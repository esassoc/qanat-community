create procedure dbo.pWaterAccountMostRecentEffectiveDate
(
	@waterAccountID int,
	@year int
)
as

with waterAccountParcels(WaterAccountID, ParcelID)
as
(
    select wap.WaterAccountID, wap.ParcelID
	from dbo.fWaterAccountParcelByWaterAccountAndYear(@waterAccountID, @year) wap
)

select  @waterAccountID as WaterAccountID, 
(
	select top 1 ps.EffectiveDate
    from dbo.fWaterAccountParcelByWaterAccountAndYear(@waterAccountID, @year) wap
    join dbo.ParcelSupply ps on wap.ParcelID = ps.ParcelID
    join dbo.fReportingPeriod(@year) rp on ps.GeographyID = rp.GeographyID and ps.EffectiveDate between rp.StartDate and rp.EndDate
    where wap.WaterAccountID = @waterAccountID
	order by ps.EffectiveDate desc
) as MostRecentSupplyEffectiveDate,
(
	select top 1 wmsor.ReportedDate
	from dbo.vWaterMeasurementSourceOfRecord wmsor
    join dbo.fReportingPeriod(@year) rp on wmsor.GeographyID = rp.GeographyID and wmsor.ReportedDate between rp.StartDate and rp.EndDate
	join waterAccountParcels wap on wmsor.ParcelID = wap.ParcelID
    where wap.WaterAccountID = @waterAccountID
	order by wmsor.ReportedDate desc
) as MostRecentUsageEffectiveDate

GO
