CREATE FUNCTION dbo.fWaterAccountParcelByUser(@userID int, @year int)
RETURNS TABLE 
AS
RETURN
(
    select WaterAccountID, ParcelID
    from
    (
        select was.WaterAccountID, wap.ParcelID, 
	        rank() over (partition by wap.WaterAccountID, wap.ParcelID order by wap.EffectiveYear desc) as Ranking 
        from dbo.WaterAccountParcel wap
        join dbo.fWaterAccountUser(@userID) was on wap.WaterAccountID = was.WaterAccountID
        where wap.EffectiveYear <= @year
--        join dbo.fReportingPeriod(@year) rp on was.GeographyID = rp.GeographyID
--        where wap.EffectiveYear <= rp.EndDate
    ) a
    where a.Ranking = 1
)

GO

