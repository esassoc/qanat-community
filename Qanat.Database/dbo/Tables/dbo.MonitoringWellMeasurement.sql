CREATE TABLE dbo.MonitoringWellMeasurement(
	MonitoringWellMeasurementID int NOT NULL IDENTITY(1,1) CONSTRAINT PK_MonitoringWellMeasurement_MonitoringWellMeasurementID PRIMARY KEY,
	MonitoringWellID int NOT NULL constraint FK_MonitoringWell_MonitoringWellID foreign key references dbo.MonitoringWell(MonitoringWellID) ON DELETE CASCADE,
	GeographyID int NOT NULL CONSTRAINT FK_MonitoringWellMeasurement_Geography_GeographyID foreign key references dbo.Geography(GeographyID),
	ExtenalUniqueID int not null,
	Measurement decimal (10,2) not null,
	MeasurementDate DateTime not null
);
GO

CREATE INDEX IX_MonitoringWellMeasurement_GeographyID on dbo.MonitoringWellMeasurement(GeographyID);
GO

CREATE INDEX IX_MonitoringWellMeasurement_MonitoringWellID on dbo.MonitoringWellMeasurement(MonitoringWellID);
GO
