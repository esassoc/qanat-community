CREATE TABLE [dbo].[ParcelHistory]
(
	[ParcelHistoryID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ParcelHistory_ParcelHistoryID] PRIMARY KEY,
	[GeographyID] INT NOT NULL CONSTRAINT [FK_ParcelHistory_Geography_GeographyID] FOREIGN KEY REFERENCES dbo.[Geography]([GeographyID]),
	[ParcelID] INT NOT NULL CONSTRAINT [FK_ParcelHistory_Parcel_ParcelID] FOREIGN KEY REFERENCES [dbo].[Parcel] ([ParcelID]),
	[EffectiveYear] INT NOT NULL,
	[UpdateDate] DATETIME NOT NULL,
	[UpdateUserID] INT NOT NULL CONSTRAINT [FK_ParcelHistory_User_UpdateUserID_UserID] FOREIGN KEY REFERENCES [dbo].[User] ([UserID]),
	[ParcelArea] DECIMAL(10,2) NOT NULL,
	[OwnerName] VARCHAR(500) NULL, 
	[OwnerAddress] VARCHAR(500) NULL,
	[ParcelStatusID] INT NOT NULL CONSTRAINT [ParcelHistory_ParcelStatus_ParcelStatusID] FOREIGN KEY REFERENCES [dbo].[ParcelStatus] ([ParcelStatusID]),
	[IsReviewed] BIT NOT NULL DEFAULT 0,
	[IsManualOverride] BIT NOT NULL DEFAULT 0,
	[ReviewDate] DATETIME NULL,
	[WaterAccountID] INT NULL CONSTRAINT [ParcelHistory_WaterAccount_WaterAccountID] FOREIGN KEY REFERENCES [dbo].[WaterAccount] ([WaterAccountID])
)
GO