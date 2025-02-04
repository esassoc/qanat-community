CREATE TABLE [dbo].[WaterMeasurementSelfReportStatus]
(
	[WaterMeasurementSelfReportStatusID]			INT NOT NULL CONSTRAINT [PK_WaterMeasurementSelfReportStatus_WaterMeasurementSelfReportStatusID] PRIMARY KEY,
	[WaterMeasurementSelfReportStatusName]			VARCHAR(20) NOT NULL CONSTRAINT [AK_WaterMeasurementSelfReportStatus_WaterMeasurementSelfReportStatusName] UNIQUE,
	[WaterMeasurementSelfReportStatusDisplayName]	VARCHAR(20) NOT NULL CONSTRAINT [AK_WaterMeasurementSelfReportStatus_WaterMeasurementSelfReportStatusDisplayName] UNIQUE
) 
