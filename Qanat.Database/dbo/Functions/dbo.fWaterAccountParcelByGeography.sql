CREATE FUNCTION dbo.fWaterAccountParcelByGeography(@geographyID int, @year int)
RETURNS TABLE 
AS
RETURN
(
    select WaterAccountID, ParcelID
    from
    (
        select wap.WaterAccountID, wap.ParcelID, 
	        rank() over (partition by wap.WaterAccountID, wap.ParcelID order by wap.EffectiveYear desc) as Ranking 
        from dbo.WaterAccountParcel wap
        --join dbo.fReportingPeriod(@year) rp on wap.GeographyID = rp.GeographyID
        where wap.GeographyID = @geographyID 
        and wap.EffectiveYear <= @year
        --and wap.EffectiveDate <= rp.EndDate
    ) a
    where a.Ranking = 1
)

GO

