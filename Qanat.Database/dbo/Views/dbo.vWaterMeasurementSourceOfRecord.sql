create view dbo.vWaterMeasurementSourceOfRecord
as

-- do we need reported date to be new DateTime(year, month, 1).AddMonths(1).AddDays(-1)?

select wm.[WaterMeasurementID], wm.[GeographyID], wm.[WaterMeasurementTypeID], wmt.WaterMeasurementTypeName, [UnitTypeID], ul.ParcelID, UL.UsageLocationID, UL.[Name] as UsageLocationName, UL.ReportingPeriodID, [ReportedDate], [ReportedValueInNativeUnits], [ReportedValueInAcreFeet], [ReportedValueInFeet], UL.[Area] as UsageLocationArea, [LastUpdateDate], [FromManualUpload], [Comment]
from dbo.WaterMeasurement wm
join dbo.WaterMeasurementType wmt on wm.WaterMeasurementTypeID = wmt.WaterMeasurementTypeID
join dbo.UsageLocation ul on wm.UsageLocationID = ul.UsageLocationID and wm.GeographyID = ul.GeographyID
join dbo.[Geography] g on wm.GeographyID = g.GeographyID and wm.WaterMeasurementTypeID = g.SourceOfRecordWaterMeasurementTypeID

GO

/*
select * from dbo.vWaterMeasurementSourceOfRecord
*/