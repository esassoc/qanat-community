CREATE TABLE [dbo].[MeterReadingMonthlyInterpolation]
(
	[MeterReadingMonthlyInterpolationID]	INT				NOT NULL IDENTITY(1, 1),
	[GeographyID]							INT				NOT NULL,
	[WellID]								INT				NOT NULL,
	[MeterID]								INT				NOT NULL,
	[MeterReadingUnitTypeID]				INT				NOT NULL,

	[Date]									DATETIME		NOT NULL,
	[InterpolatedVolume]					DECIMAL(20, 4)	NOT NULL,
	[InterpolatedVolumeInAcreFeet]			DECIMAL(20, 4)	NOT NULL,

	CONSTRAINT PK_MeterReadingMonthlyInterpolation_MeterReadingMonthlyInterpolationID			PRIMARY KEY ([MeterReadingMonthlyInterpolationID]),

	CONSTRAINT FK_MeterReadingMonthlyInterpolation_Geography_GeographyID						FOREIGN KEY ([GeographyID])				REFERENCES dbo.[Geography]([GeographyID]),
	CONSTRAINT FK_MeterReadingMonthlyInterpolation_Well_WellID									FOREIGN KEY ([WellID])					REFERENCES dbo.[Well]([WellID]),
	CONSTRAINT FK_MeterReadingMonthlyInterpolation_Meter_MeterID								FOREIGN KEY ([MeterID])					REFERENCES dbo.[Meter]([MeterID]),
	CONSTRAINT FK_MeterReadingMonthlyInterpolation_MeterReadingUnitType_MeterReadingUnitTypeID	FOREIGN KEY ([MeterReadingUnitTypeID])	REFERENCES dbo.[MeterReadingUnitType]([MeterReadingUnitTypeID]),

	CONSTRAINT AK_MeterReadingMonthlyInterpolation_MeterID_Date									UNIQUE ([MeterID], [Date])
);
GO

CREATE INDEX IX_MeterReadingMonthlyInterpolation_GeographyID_WellID_MeterID_Date ON dbo.MeterReadingMonthlyInterpolation([GeographyID], [WellID], [MeterID], [Date]);