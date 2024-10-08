CREATE TABLE [dbo].[WellRegistrationIrrigatedParcel]
(
	[WellRegistrationIrrigatedParcelID] INT NOT NULL IDENTITY(1, 1) CONSTRAINT PK_WellRegistrationIrrigatedParcel_WellRegistrationIrrigatedParcelID PRIMARY KEY,
	[WellRegistrationID] INT NOT NULL CONSTRAINT FK_WellRegistrationIrrigatedParcel_WellRegistration_WellRegistrationID FOREIGN KEY REFERENCES dbo.WellRegistration(WellRegistrationID),
	[ParcelID] INT NOT NULL CONSTRAINT FK_WellRegistrationIrrigatedParcel_Parcel_ParcelID FOREIGN KEY REFERENCES dbo.Parcel(ParcelID),
	CONSTRAINT AK_WellRegistrationIrrigatedParcel_WellRegistrationID_ParcelID UNIQUE (WellRegistrationID, ParcelID)
)
