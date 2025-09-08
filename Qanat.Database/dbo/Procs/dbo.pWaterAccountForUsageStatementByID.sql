create procedure dbo.pWaterAccountForUsageStatementByID
(
	@waterAccountID int,
	@reportingPeriodID int
)
as

select wa.WaterAccountID, wa.WaterAccountNumber, wac.ContactName, wac.[Address], wac.SecondaryAddress, wac.City, wac.[State], wac.ZipCode, wa.WaterAccountPIN,
	wap.ParcelIDs, wap.ParcelNumbers, wap.ParcelArea, wap.UsageLocationArea, wap.ZoneName,
	wap.ReportingPeriodStartDate, wap.ReportingPeriodEndDate,
	g.GeographyID, g.GeographyDisplayName, g.ContactEmail as GeographyContactEmail, g.ContactPhoneNumber as GeographyContactPhoneNumber, 
	g.ContactAddressLine1 as GeographyAddressLine1, g.ContactAddressLine2 as GeographyAddressLine2
from dbo.WaterAccount wa
join dbo.WaterAccountContact wac on wa.WaterAccountContactID = wac.WaterAccountContactID
join dbo.Geography g on wa.GeographyID = g.GeographyID
join (
	select wap.WaterAccountID, string_agg(p.ParcelID, ',') as ParcelIDs, string_agg(p.ParcelNumber, ',') as ParcelNumbers, sum(p.ParcelArea) as ParcelArea,
		sum(ula.UsageLocationArea) as UsageLocationArea, max(z.ZoneName) as ZoneName,
		max(rp.StartDate) as ReportingPeriodStartDate, max(rp.EndDate) as ReportingPeriodEndDate
	from dbo.Parcel p
	join dbo.WaterAccountParcel wap on p.ParcelID = wap.ParcelID
	join dbo.ReportingPeriod rp on wap.ReportingPeriodID = rp.ReportingPeriodID
	join dbo.GeographyAllocationPlanConfiguration gapc on p.GeographyID = gapc.GeographyID
	join dbo.ParcelZone pz on p.ParcelID = pz.ParcelID
	join dbo.Zone z on pz.ZoneID = z.ZoneID and z.ZoneGroupID = gapc.ZoneGroupID
	join (
		select p.ParcelID, ul.ReportingPeriodID, sum(ul.Area) as UsageLocationArea
		from dbo.Parcel p 
		join dbo.UsageLocation ul on p.ParcelID = ul.ParcelID
		group by p.ParcelID, ul.ReportingPeriodID
	) ula on p.ParcelID = ula.ParcelID and rp.ReportingPeriodID = ula.ReportingPeriodID
	where wap.WaterAccountID = @waterAccountID and rp.ReportingPeriodID = @reportingPeriodID
	group by wap.WaterAccountID
) wap on wap.WaterAccountID = wa.WaterAccountID
where wap.WaterAccountID = @waterAccountID