CREATE TABLE [dbo].[ParcelCustomAttribute]
(
	[ParcelCustomAttributeID] INT NOT NULL IDENTITY(1,1) CONSTRAINT [PK_ParcelCustomAttribute_ParcelCustomAttributeID] PRIMARY KEY,
	[ParcelID] INT NOT NULL CONSTRAINT [FK_ParcelCustomAttribute_Parcel_ParcelID] FOREIGN KEY REFERENCES dbo.Parcel(ParcelID) CONSTRAINT [AK_ParcelCustomAttribute_Parcel] UNIQUE,
	[CustomAttributes] VARCHAR(MAX) NOT NULL DEFAULT ('{}')
)
