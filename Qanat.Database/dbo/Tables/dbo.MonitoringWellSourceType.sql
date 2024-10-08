CREATE TABLE [dbo].[MonitoringWellSourceType] (
    [MonitoringWellSourceTypeID]          INT           NOT NULL,
    [MonitoringWellSourceTypeName]        VARCHAR (100) NOT NULL,
    [MonitoringWellSourceTypeDisplayName] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_MonitoringWellSourceType_MonitoringWellSourceTypeID] PRIMARY KEY CLUSTERED ([MonitoringWellSourceTypeID] ASC),
    CONSTRAINT [AK_MonitoringWellSourceType_MonitoringWellSourceTypeDisplayName] UNIQUE NONCLUSTERED ([MonitoringWellSourceTypeDisplayName] ASC),
    CONSTRAINT [AK_MonitoringWellSourceType_MonitoringWellSourceTypeName] UNIQUE NONCLUSTERED ([MonitoringWellSourceTypeName] ASC)
 );