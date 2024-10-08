create view dbo.vWaterMeasurementSourceOfRecord
as

-- do we need reported date to be new DateTime(year, month, 1).AddMonths(1).AddDays(-1)?

select wmt.[WaterMeasurementID], wmt.[GeographyID], [WaterMeasurementTypeID], [UnitTypeID], ue.ParcelID, wmt.[UsageEntityName], [ReportedDate], [ReportedValue], [ReportedValueInAcreFeet], wmt.[UsageEntityArea], [LastUpdateDate], [FromManualUpload], [Comment]
from dbo.WaterMeasurement wmt
join dbo.UsageEntity ue on wmt.UsageEntityName = ue.UsageEntityName and wmt.GeographyID = ue.GeographyID
join dbo.Geography g on wmt.GeographyID = g.GeographyID and wmt.WaterMeasurementTypeID = g.SourceOfRecordWaterMeasurementTypeID

GO

/*
select * from dbo.vWaterMeasurementSourceOfRecord
*/