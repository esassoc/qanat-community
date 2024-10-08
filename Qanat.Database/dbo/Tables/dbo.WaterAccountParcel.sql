CREATE TABLE [dbo].[WaterAccountParcel](
	[WaterAccountParcelID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_WaterAccountParcel_WaterAccountParcelID] PRIMARY KEY,
	[GeographyID] int not null constraint [FK_WaterAccountParcel_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[WaterAccountID] [int] NOT NULL CONSTRAINT [FK_WaterAccountParcel_WaterAccount_WaterAccountID] FOREIGN KEY REFERENCES [dbo].[WaterAccount] ([WaterAccountID]),
	[ParcelID] [int] NOT NULL CONSTRAINT [FK_WaterAccountParcel_Parcel_ParcelID] FOREIGN KEY REFERENCES [dbo].[Parcel] ([ParcelID]),
	[EffectiveYear] INT NOT NULL, 
    [EndYear] INT NULL, 
    CONSTRAINT [AK_WaterAccountParcel_WaterAccountID_ParcelID_EffectiveYear] UNIQUE ([WaterAccountID], [ParcelID], [EffectiveYear]),
	constraint [FK_WaterAccountParcel_WaterAccount_WaterAccountID_GeographyID] foreign key ([WaterAccountID], [GeographyID]) references dbo.[WaterAccount]([WaterAccountID], [GeographyID]),
	constraint [FK_WaterAccountParcel_Parcel_ParcelID_GeographyID] foreign key ([ParcelID], [GeographyID]) references dbo.[Parcel]([ParcelID], [GeographyID]),
	CONSTRAINT [CHK_WaterAccountParcel_EndDate_GreaterThan_EffectiveYear] CHECK (EndYear is null or (EndYear > EffectiveYear))
)