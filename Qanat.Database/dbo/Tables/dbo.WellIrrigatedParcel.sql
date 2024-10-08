CREATE TABLE [dbo].[WellIrrigatedParcel]
(
	[WellIrrigatedParcelID] INT NOT NULL IDENTITY(1, 1) CONSTRAINT PK_WellIrrigatedParcel_WellIrrigatedParcelID PRIMARY KEY,
	[WellID] INT NOT NULL CONSTRAINT FK_WellIrrigatedParcel_Well_WellID FOREIGN KEY REFERENCES dbo.Well(WellID),
	[ParcelID] INT NOT NULL CONSTRAINT FK_WellIrrigatedParcel_Parcel_ParcelID FOREIGN KEY REFERENCES dbo.Parcel(ParcelID),
	CONSTRAINT AK_WellIrrigatedParcel_WellID_ParcelID UNIQUE (WellID, ParcelID)
)
