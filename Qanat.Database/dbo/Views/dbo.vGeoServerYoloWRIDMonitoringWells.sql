create view dbo.vGeoServerYoloWRIDMonitoringWells
as
select          mw.MonitoringWellID as PrimaryKey,
				mw.GeographyID,
				mw.MonitoringWellName,
				mw.SiteCode,
				mw.MonitoringWellSourceTypeID,
				mw.[Geometry]
				
                
FROM        dbo.MonitoringWell mw
WHERE		mw.MonitoringWellSourceTypeID = 2

GO
/*
select * from dbo.vGeoServerYoloWRIDMonitoringWells
*/