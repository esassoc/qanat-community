CREATE TABLE dbo.Meter(
	MeterID int NOT NULL IDENTITY(1,1) CONSTRAINT PK_Meter_MeterID PRIMARY KEY,
	SerialNumber varchar(25) NOT NULL,
	DeviceName varchar(255) NULL,
    Make varchar(100) null,
	ModelNumber varchar(25) NULL,
	GeographyID int NOT NULL CONSTRAINT FK_Meter_Geography_GeographyID FOREIGN KEY REFERENCES dbo.[Geography](GeographyID),
	MeterStatusID int NOT NULL CONSTRAINT FK_Meter_MeterStatus_MeterStatusID foreign key references dbo.MeterStatus(MeterStatusID)
)