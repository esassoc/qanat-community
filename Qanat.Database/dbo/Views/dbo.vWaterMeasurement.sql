create view dbo.vWaterMeasurement
as

select wmt.[WaterMeasurementID], wmt.[GeographyID], [WaterMeasurementTypeID], [UnitTypeID], ue.ParcelID, wmt.[UsageEntityName], [ReportedDate], [ReportedValue], [ReportedValueInAcreFeet], wmt.[UsageEntityArea], [LastUpdateDate], [FromManualUpload], [Comment]
from dbo.WaterMeasurement wmt
join dbo.UsageEntity ue on wmt.UsageEntityName = ue.UsageEntityName and wmt.GeographyID = ue.GeographyID

GO

/*
select * from dbo.vWaterMeasurement
*/