CREATE TABLE [dbo].[Parcel]
(
	[ParcelID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Parcel_ParcelID] PRIMARY KEY,
	[GeographyID] int not null constraint [FK_Parcel_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[WaterAccountID] [int] NULL CONSTRAINT [FK_Parcel_WaterAccount_WaterAccountID] FOREIGN KEY REFERENCES [dbo].[WaterAccount] ([WaterAccountID]),
	[ParcelNumber] [varchar](20) NOT NULL,
	[ParcelArea] [float] NOT NULL,
	[ParcelStatusID] [int] NOT NULL CONSTRAINT [FK_Parcel_ParcelStatus_ParcelStatusID] FOREIGN KEY REFERENCES [dbo].[ParcelStatus] ([ParcelStatusID]),
	[OwnerAddress] [varchar](500) NOT NULL,
	[OwnerName] VARCHAR(500) NOT NULL,
	CONSTRAINT [AK_Parcel_ParcelID_GeographyID] unique ([ParcelID], [GeographyID]),
	CONSTRAINT AK_Parcel_ParcelNumber_GeographyID unique ([ParcelNumber], [GeographyID]),
	CONSTRAINT CK_Parcel_WaterAccountID_ParcelStatusID CHECK (([ParcelStatusID] = 1 and [WaterAccountID] is not null) or ([ParcelStatusID] = 2 and [WaterAccountID] is null) or ([ParcelStatusID] = 3 or [ParcelStatusID] = 4) and [WaterAccountID] is null)
)
GO