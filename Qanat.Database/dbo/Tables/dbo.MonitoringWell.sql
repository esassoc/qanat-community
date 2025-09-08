CREATE TABLE dbo.MonitoringWell(
	MonitoringWellID int NOT NULL IDENTITY(1,1) CONSTRAINT PK_MonitoringWell_MonitoringWellID PRIMARY KEY,
	GeographyID int NOT NULL CONSTRAINT FK_MonitoringWell_Geography_GeographyID foreign key references dbo.Geography(GeographyID),
	SiteCode varchar(255) NOT NULL,
    MonitoringWellName varchar(100) null,
	MonitoringWellSourceTypeID int NOT NULL CONSTRAINT FK_MonitoringWell_MonitoringWellSourceType_MonitoringWellSourceTypeID foreign key references dbo.MonitoringWellSourceType(MonitoringWellSourceTypeID),
	[Geometry] geometry not null
)