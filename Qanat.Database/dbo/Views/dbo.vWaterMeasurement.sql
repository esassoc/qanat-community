-- DROP VIEW dbo.vWaterMeasurement
CREATE VIEW dbo.vWaterMeasurement
AS

SELECT 
	WM.[WaterMeasurementID]
,	WM.[GeographyID]
,	WM.[ReportedDate]
,	WM.[ReportedValueInNativeUnits]
,	WM.[ReportedValueInAcreFeet]
,	WM.[ReportedValueInFeet]
,	WM.[LastUpdateDate]
,	WM.[FromManualUpload]
,	WM.[Comment]
,	WMT.WaterMeasurementTypeID
,	WMT.WaterMeasurementTypeName
,	UT.UnitTypeID
,	UT.UnitTypeDisplayName
,	UL.UsageLocationID
,	UL.[Name] AS UsageLocationName
,	UL.[Area] AS UsageLocationArea
,	UL.ReportingPeriodID
,	ULT.UsageLocationTypeID
,	ULT.[Name] AS UsageLocationTypeName
,	P.ParcelID
,	P.ParcelNumber
,	WA.WaterAccountID
,	WA.WaterAccountNumber
,	WA.WaterAccountName
,	CCS_SRS.SelfReportStatusDisplayName AS CoverCropStatus
,	FS_SRS.SelfReportStatusDisplayName AS FallowStatus
FROM dbo.WaterMeasurement					WM
JOIN dbo.WaterMeasurementType				WMT		ON WM.WaterMeasurementTypeID = WMT.WaterMeasurementTypeID
LEFT JOIN dbo.UnitType						UT		ON WM.UnitTypeID = UT.UnitTypeID 
JOIN dbo.UsageLocation						UL		ON WM.UsageLocationID = UL.UsageLocationID AND WM.GeographyID = UL.GeographyID
JOIN dbo.UsageLocationType					ULT		ON UL.UsageLocationTypeID = ULT.UsageLocationTypeID
JOIN dbo.Parcel								P		ON UL.ParcelID = P.ParcelID
LEFT JOIN dbo.WaterAccountParcel			WAP		ON WAP.ReportingPeriodID = UL.ReportingPeriodID AND WAP.ParcelID = P.ParcelID
LEFT JOIN dbo.WaterAccount					WA		ON WAP.WaterAccountID = WA.WaterAccountID
LEFT JOIN dbo.WaterAccountCoverCropStatus	CCS		ON WA.WaterAccountID = CCS.WaterAccountID AND UL.ReportingPeriodID = CCS.ReportingPeriodID
LEFT JOIN dbo.SelfReportStatus				CCS_SRS ON CCS.SelfReportStatusID = CCS_SRS.SelfReportStatusID
LEFT JOIN dbo.WaterAccountFallowStatus		FS		ON WA.WaterAccountID = FS.WaterAccountID AND UL.ReportingPeriodID = FS.ReportingPeriodID
LEFT JOIN dbo.SelfReportStatus				FS_SRS	ON FS.SelfReportStatusID = FS_SRS.SelfReportStatusID

GO

/*
SELECT * FROM dbo.vWaterMeasurement WM WHERE WM.GeographyID = 5  AND WM.WaterMeasurementTypeID = 1 
*/