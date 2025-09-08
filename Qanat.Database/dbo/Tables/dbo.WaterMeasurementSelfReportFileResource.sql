CREATE TABLE [dbo].[WaterMeasurementSelfReportFileResource]
(
    [WaterMeasurementSelfReportFileResourceID]  INT             NOT NULL IDENTITY(1,1),
    [WaterMeasurementSelfReportID]              INT             NOT NULL,
    [FileResourceID]                            INT             NOT NULL,
    [FileDescription]                           VARCHAR(200)    NULL,

    CONSTRAINT [PK_WaterMeasurementSelfReportFileResource_WaterMeasurementSelfReportFileResourceID]                 PRIMARY KEY ([WaterMeasurementSelfReportFileResourceID]),

    CONSTRAINT [FK_WaterMeasurementSelfReportFileResource_WaterMeasurementSelfReport_WaterMeasurementSelfReportID]  FOREIGN KEY ([WaterMeasurementSelfReportID])    REFERENCES dbo.WaterMeasurementSelfReport([WaterMeasurementSelfReportID]),
    CONSTRAINT [FK_WaterMeasurementSelfReportFileResource_FileResource_FileResourceID]                              FOREIGN KEY ([FileResourceID])                  REFERENCES dbo.FileResource([FileResourceID])
);