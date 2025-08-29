CREATE VIEW dbo.vGeoServerAllParcels
AS
SELECT 
    p.ParcelID,
    p.GeographyID,
    p.ParcelNumber,
    p.ParcelArea,
    pg.Geometry4326 AS ParcelGeometry,
    p.WaterAccountID AS CurrentWaterAccountID,
    p.ParcelStatusID,
    wap.ReportingPeriodID,
    wap.WaterAccountID,
    CASE 
        WHEN RP.ReportingPeriodID IS NULL THEN CONVERT(BIT, 1) 
        WHEN YEAR(RP.EndDate) = YEAR(GETUTCDATE()) THEN CONVERT(BIT, 1)
        ELSE CONVERT(BIT, 0)
    END AS IsCurrent
FROM dbo.Parcel p
JOIN dbo.ParcelGeometry pg ON p.ParcelID = pg.ParcelID
LEFT JOIN dbo.WaterAccountParcel wap ON wap.ParcelID = p.ParcelID
LEFT JOIN dbo.ReportingPeriod RP ON RP.ReportingPeriodID = wap.ReportingPeriodID;
/*
select * from dbo.vGeoServerAllParcels
*/
