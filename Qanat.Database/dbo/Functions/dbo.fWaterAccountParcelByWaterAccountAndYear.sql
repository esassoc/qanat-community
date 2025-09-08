CREATE FUNCTION dbo.fWaterAccountParcelByWaterAccountAndYear(@waterAccountID int, @year int)
RETURNS TABLE 
AS
RETURN
(
    select WaterAccountID, ParcelID, ParcelNumber
    from
    (
	    select wap.WaterAccountID, wap.ParcelID, p.ParcelNumber, rank() over (partition by wap.WaterAccountID, wap.ParcelID order by rp.EndDate desc) as Ranking 
	    from dbo.WaterAccountParcel wap
        join dbo.Parcel p on wap.ParcelID = p.ParcelID
        join dbo.ReportingPeriod rp on rp.ReportingPeriodID = wap.ReportingPeriodID
	    where wap.WaterAccountID = @waterAccountID 
        and YEAR(RP.StartDate) <= @year and (RP.EndDate >= @year)
	) wap 
    where wap.Ranking = 1
)
GO