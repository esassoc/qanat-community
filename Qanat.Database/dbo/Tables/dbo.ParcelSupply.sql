CREATE TABLE [dbo].[ParcelSupply](
	[ParcelSupplyID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ParcelSupply_ParcelSupplyID] PRIMARY KEY,
	[GeographyID] int not null constraint [FK_ParcelSupply_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[ParcelID] [int] NOT NULL CONSTRAINT [FK_ParcelSupply_Parcel_ParcelID] FOREIGN KEY REFERENCES [dbo].[Parcel] ([ParcelID]),
	[TransactionDate] [datetime] NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[TransactionAmount] [decimal](10, 4) NOT NULL,
	[WaterTypeID] [int] NULL CONSTRAINT [FK_ParcelSupply_WaterType_WaterTypeID] FOREIGN KEY REFERENCES [dbo].[WaterType] ([WaterTypeID]),
	[UserID] [int] NULL CONSTRAINT [FK_ParcelSupply_User_UserID] FOREIGN KEY REFERENCES [dbo].[User] ([UserID]),
	[UserComment] [varchar](max) NULL,
	[UploadedFileName] [varchar](100) NULL,
	constraint [FK_ParcelSupply_Parcel_ParcelID_GeographyID] foreign key ([ParcelID], [GeographyID]) references dbo.[Parcel]([ParcelID], [GeographyID]),
	constraint [FK_ParcelSupply_WaterType_WaterTypeID_GeographyID] foreign key ([WaterTypeID], [GeographyID]) references dbo.[WaterType]([WaterTypeID], [GeographyID])
)