create procedure dbo.pZoneGroupMonthlyUsage
(
    @geographyID int,
	@zoneGroupID int
)
as

begin

select pl.ZoneName, pl.ZoneColor, datefromparts(pl.EffectiveYear, pl.EffectiveMonth, 1) as EffectiveDate, 
	pl.TotalMonthlyUsage, pl.TotalMonthlyUsage / pa.ParcelArea as TotalMonthlyUsageDepth
from (
	select z.ZoneName, max(z.ZoneColor) as ZoneColor, month(pl.ReportedDate) as EffectiveMonth, year(pl.ReportedDate) as EffectiveYear,
		sum(pl.ReportedValueInAcreFeet) as TotalMonthlyUsage
	from dbo.vWaterMeasurementSourceOfRecord pl
	join dbo.ParcelZone pz on pl.ParcelID = pz.ParcelID
	join dbo.[Zone] z on pz.ZoneID = z.ZoneID
	where pl.GeographyID = @geographyID and z.ZoneGroupID = @zoneGroupID
	group by z.ZoneName, month(pl.ReportedDate), year(pl.ReportedDate)
) pl
left join (
	select z.ZoneName, sum(p.ParcelArea) as ParcelArea 
	from dbo.Parcel p
	join dbo.ParcelZone pz on p.ParcelID = pz.ParcelID
	join dbo.[Zone] z on pz.ZoneID = z.ZoneID
	where p.GeographyID = @geographyID and z.ZoneGroupID = @zoneGroupID
	group by z.ZoneName
) pa on pl.ZoneName = pa.ZoneName

end