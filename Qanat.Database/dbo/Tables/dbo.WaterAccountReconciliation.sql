CREATE TABLE [dbo].[WaterAccountReconciliation](
	[WaterAccountReconciliationID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_WaterAccountReconciliation_WaterAccountReconciliationID] PRIMARY KEY,
	[GeographyID] int not null constraint [FK_WaterAccountReconciliation_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[ParcelID] [int] NOT NULL CONSTRAINT [FK_WaterAccountReconciliation_Parcel_ParcelID] FOREIGN KEY REFERENCES [dbo].[Parcel] ([ParcelID]),
	[WaterAccountID] [int] NOT NULL CONSTRAINT [FK_WaterAccountReconciliation_WaterAccount_WaterAccountID] FOREIGN KEY REFERENCES [dbo].[WaterAccount] ([WaterAccountID]),
	CONSTRAINT [AK_WaterAccountReconciliation_ParcelID_WaterAccountID] UNIQUE ([ParcelID], [WaterAccountID]),
	constraint [FK_WaterAccountReconciliation_WaterAccount_WaterAccountID_GeographyID] foreign key ([WaterAccountID], [GeographyID]) references dbo.[WaterAccount]([WaterAccountID], [GeographyID]),
	constraint [FK_WaterAccountReconciliation_Parcel_ParcelID_GeographyID] foreign key ([ParcelID], [GeographyID]) references dbo.[Parcel]([ParcelID], [GeographyID]),
)
