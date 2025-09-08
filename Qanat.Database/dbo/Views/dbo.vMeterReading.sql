CREATE VIEW dbo.vMeterReading
AS
SELECT
	G.GeographyID
,	G.GeographyDisplayName
,	W.WellID
,	W.WellName
,	M.MeterID
,	M.SerialNumber
,	MRUT.MeterReadingUnitTypeID
,	MRUT.MeterReadingUnitTypeDisplayName

,	MR.ReadingDate
,	MR.PreviousReading
,	MR.CurrentReading
,	MR.Volume
,	MR.VolumeInAcreFeet
,	MR.ReaderInitials
,	MR.Comment
FROM dbo.MeterReading			MR
JOIN dbo.[Geography]			G		ON G.GeographyID = MR.GeographyID
JOIN dbo.Well					W		ON W.WellID = MR.WellID
JOIN dbo.Meter					M		ON M.MeterID = MR.MeterID
JOIN dbo.MeterReadingUnitType	MRUT	ON MRUT.MeterReadingUnitTypeID = MR.MeterReadingUnitTypeID