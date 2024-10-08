create view dbo.vGeoServerMonitoringWells
as
select          mw.MonitoringWellID as PrimaryKey,
				mw.GeographyID,
				mw.MonitoringWellName,
				mw.SiteCode,
				mw.MonitoringWellSourceTypeID,
				mw.[Geometry]
                

FROM        dbo.MonitoringWell mw

GO
/*
select * from dbo.vGeoServerMonitoringWells
*/
