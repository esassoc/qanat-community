CREATE FUNCTION dbo.fWaterAccountParcelByGeographyAndYear(@geographyID int, @year int)
RETURNS TABLE 
AS
RETURN
(
    select WaterAccountID, ParcelID, ParcelNumber
    from
    (
	    select wap.WaterAccountID, wap.ParcelID, p.ParcelNumber, rank() over (partition by wap.WaterAccountID, wap.ParcelID order by wap.EffectiveYear desc) as Ranking 
	    from dbo.WaterAccountParcel wap
        join dbo.Parcel p on wap.ParcelID = p.ParcelID
	    where wap.GeographyID = @geographyID 
        /*
        EffectiveYear can equal EndYear, so that means:
        EffectiveYear 2024, EndYear 2024 = only showing up for 2024
        EffectiveYear 2024, EndYear 2025 = showing up for 2024 and 2025
        */
        and wap.EffectiveYear <= @year and (wap.EndYear is null or wap.EndYear >= @year)
	) wap 
    where wap.Ranking = 1
)
GO