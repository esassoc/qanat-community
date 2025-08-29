CREATE FUNCTION dbo.fWaterAccountParcelByUser(@userID int, @year int)
RETURNS TABLE 
AS
RETURN
(
    select WaterAccountID, ParcelID
    from
    (
        select was.WaterAccountID, wap.ParcelID, 
	        rank() over (partition by wap.WaterAccountID, wap.ParcelID order by rp.EndDate desc) as Ranking 
        from dbo.WaterAccountParcel wap
        join dbo.ReportingPeriod rp on rp.ReportingPeriodID = wap.ReportingPeriodID
        join dbo.fWaterAccountUser(@userID) was on wap.WaterAccountID = was.WaterAccountID
        where YEAR(rp.EndDate) <= @year
    ) a
    where a.Ranking = 1
)

GO

