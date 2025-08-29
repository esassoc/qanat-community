create view dbo.vGeoServerAllUsageLocations
as

SELECT 
    ul.UsageLocationID,
	ul.[Name] as UsageLocationName,
    ul.GeographyID,
    ul.Area,
	ul.ReportingPeriodID,
    ulg.Geometry4326,
	ult.ColorHex as UsageLocationTypeColor,
    p.ParcelID,
    p.ParcelArea,
    wap.WaterAccountID,
	CASE 
        WHEN RP.ReportingPeriodID IS NULL THEN CONVERT(BIT, 1) 
        WHEN YEAR(RP.EndDate) = YEAR(GETUTCDATE()) THEN CONVERT(BIT, 1)
        ELSE CONVERT(BIT, 0)
    END AS IsCurrent
FROM dbo.UsageLocation				ul
JOIN dbo.UsageLocationType			ult ON ul.UsageLocationTypeID = ult.UsageLocationTypeID
JOIN dbo.UsageLocationGeometry		ulg ON ul.UsageLocationID = ulg.UsageLocationID
JOIN dbo.ReportingPeriod			rp  ON rp.ReportingPeriodID = ul.ReportingPeriodID
JOIN dbo.Parcel						p   ON p.ParcelID = ul.ParcelID
LEFT JOIN dbo.WaterAccountParcel    wap ON wap.ParcelID = p.ParcelID AND wap.ReportingPeriodID = rp.ReportingPeriodID
GO
/*
select * from dbo.vGeoServerAllUsageLocations
*/