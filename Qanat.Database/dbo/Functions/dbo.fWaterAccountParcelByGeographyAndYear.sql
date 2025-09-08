CREATE FUNCTION dbo.fWaterAccountParcelByGeographyAndYear(@geographyID int, @year int)
RETURNS TABLE 
AS
RETURN
(
    select WaterAccountParcelID, WaterAccountID, ParcelID, ParcelNumber, ReportingPeriodID
    from
    (
	    select wap.WaterAccountParcelID, wap.WaterAccountID, wap.ParcelID, p.ParcelNumber, rp.ReportingPeriodID, rank() over (partition by wap.WaterAccountID, wap.ParcelID order by RP.EndDate desc) as Ranking 
	    from dbo.WaterAccountParcel wap
        join dbo.Parcel p on wap.ParcelID = p.ParcelID
        join dbo.ReportingPeriod rp on rp.ReportingPeriodID = wap.ReportingPeriodID
	    where wap.GeographyID = @geographyID 
        and YEAR(rp.StartDate) <= @year and YEAR(rp.EndDate) >= @year
	) wap 
    where wap.Ranking = 1
)
GO

