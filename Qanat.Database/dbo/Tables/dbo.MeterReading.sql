CREATE TABLE [dbo].[MeterReading]
(
	[MeterReadingID]			INT				NOT NULL IDENTITY(1,1),

	[GeographyID]				INT				NOT NULL,
	[WellID]					INT				NOT NULL,
	[MeterID]					INT				NOT NULL,
	[MeterReadingUnitTypeID]	INT				NOT NULL,

	[ReadingDate]				DATETIME		NOT NULL,
	[PreviousReading]			DECIMAL(20, 4)	NOT NULL,
	[CurrentReading]			DECIMAL(20, 4)	NOT NULL,

	[Volume]					DECIMAL(20, 4)	NOT NULL,
	[VolumeInAcreFeet]			DECIMAL(20, 4)	NOT NULL,

	[ReaderInitials]			VARCHAR(5)		NULL,
	[Comment]					VARCHAR(MAX)	NULL,

	CONSTRAINT PK_MeterReading_MeterReadingID									PRIMARY KEY ([MeterReadingID]),
	
	CONSTRAINT FK_MeterReading_Geography_GeographyID							FOREIGN KEY([GeographyID])				REFERENCES dbo.[Geography]([GeographyID]),
	CONSTRAINT FK_MeterReading_Well_WellID										FOREIGN KEY([WellID])					REFERENCES dbo.[Well]([WellID]),
	CONSTRAINT FK_MeterReading_Meter_MeterID									FOREIGN KEY([MeterID])					REFERENCES dbo.[Meter]([MeterID]),
	CONSTRAINT FK_MeterReading_MeterReadingUnitType_MeterReadingUnitTypeID		FOREIGN KEY([MeterReadingUnitTypeID])	REFERENCES dbo.[MeterReadingUnitType]([MeterReadingUnitTypeID]),
	
	CONSTRAINT AK_MeterReading_MeterID_ReadingDate								UNIQUE ([MeterID], [ReadingDate])
);
GO

CREATE INDEX IX_MeterReading_GeographyID_WellID_MeterID_ReadingDate ON dbo.MeterReading([GeographyID], [WellID], [MeterID], [ReadingDate]);