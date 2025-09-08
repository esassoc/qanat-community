create view dbo.vGeoServerCNRAMonitoringWells
as
select          mw.MonitoringWellID as PrimaryKey,
				mw.GeographyID,
				mw.MonitoringWellName,
				mw.SiteCode,
				mw.MonitoringWellSourceTypeID,
				mw.[Geometry]
                

FROM        dbo.MonitoringWell mw
WHERE		mw.MonitoringWellSourceTypeID = 1

GO
/*
select * from dbo.vGeoServerCNRAMonitoringWells
*/
