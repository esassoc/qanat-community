CREATE TABLE dbo.Meter(
	MeterID				INT				NOT NULL IDENTITY(1,1),
	
	GeographyID			INT				NOT NULL,
	MeterStatusID		INT				NOT NULL,
	
	SerialNumber		VARCHAR(255)	NOT NULL,
	DeviceName			VARCHAR(255)	NULL,
    Make				VARCHAR(255)	NULL,
	ModelNumber			VARCHAR(255)	NULL,
	
	CONSTRAINT PK_Meter_MeterID						PRIMARY KEY([MeterID]),

	CONSTRAINT FK_Meter_Geography_GeographyID		FOREIGN KEY([GeographyID])		REFERENCES dbo.[Geography]([GeographyID]),
	CONSTRAINT FK_Meter_MeterStatus_MeterStatusID	FOREIGN KEY([MeterStatusID])	REFERENCES dbo.[MeterStatus]([MeterStatusID]),

	CONSTRAINT AK_Meter_GeographyID_SerialNumber	UNIQUE([GeographyID], [SerialNumber])
);
GO

CREATE INDEX IX_Meter_GeographyID ON dbo.Meter([GeographyID]);