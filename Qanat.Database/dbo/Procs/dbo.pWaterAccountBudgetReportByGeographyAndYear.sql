create procedure dbo.pWaterAccountBudgetReportByGeographyAndYear
(
    @geographyID int,
    @year int
)
as

begin

with waterAccountParcels(WaterAccountID, ParcelID, ParcelArea)
as
(
    select wap.WaterAccountID, p.ParcelID, p.ParcelArea
    from dbo.Parcel p
    join dbo.fWaterAccountParcelByGeographyAndYear(@geographyID, @year) wap on p.ParcelID = wap.ParcelID
),
reportingPeriod(GeographyID, StartDate, EndDate)
as
(
    select GeographyID, StartDate, EndDate from dbo.fReportingPeriod(@year) where GeographyID = @geographyID
)

select wacc.WaterAccountID, wacc.GeographyID, wacc.WaterAccountNumber, wacc.WaterAccountName, isnull(pa.AcresManaged, 0) as AcresManaged,
supply.TotalSupply, u.UsageToDate,
isnull(supply.TotalSupply, 0) - isnull(u.UsageToDate, 0) as CurrentAvailable

from dbo.WaterAccount wacc
join
(
	select WaterAccountID, sum(ParcelArea) as AcresManaged
	from waterAccountParcels wap
	group by WaterAccountID
) pa on wacc.WaterAccountID = pa.WaterAccountID
left join (
	select wap.WaterAccountID,
		sum(wmsor.ReportedValueInAcreFeet) as UsageToDate
	from dbo.vWaterMeasurementSourceOfRecord wmsor
    join reportingPeriod rp on wmsor.GeographyID = rp.GeographyID and wmsor.ReportedDate between rp.StartDate and rp.EndDate
	join waterAccountParcels wap on wmsor.ParcelID = wap.ParcelID
	group by wap.WaterAccountID
) u on wacc.WaterAccountID = u.WaterAccountID
left join (
	select wap.WaterAccountID,
		sum(ps.TransactionAmount) as TotalSupply
	from dbo.ParcelSupply ps
    join reportingPeriod rp on ps.GeographyID = rp.GeographyID and ps.EffectiveDate between rp.StartDate and rp.EndDate
	join waterAccountParcels wap on ps.ParcelID = wap.ParcelID
	group by wap.WaterAccountID
) supply on wacc.WaterAccountID = supply.WaterAccountID
order by wacc.WaterAccountID, wacc.GeographyID, wacc.WaterAccountNumber, wacc.WaterAccountName
end
