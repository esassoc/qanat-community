CREATE TABLE [dbo].[ParcelZone](
	[ParcelZoneID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ParcelZone_ParcelZoneID] PRIMARY KEY,
	[ZoneID] [int] NOT NULL CONSTRAINT [FK_ParcelZone_Zone_ZoneID] foreign key references dbo.[Zone]([ZoneID]),
	[ParcelID] [int] NOT NULL CONSTRAINT [FK_ParcelZone_Parcel_ParcelID] foreign key references dbo.[Parcel]([ParcelID]),
	CONSTRAINT [AK_ParcelZone_Unique_ParcelID_ZoneID] UNIQUE NONCLUSTERED ([ParcelID] ASC, [ZoneID]),
)