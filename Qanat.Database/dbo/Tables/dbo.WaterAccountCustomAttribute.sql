CREATE TABLE [dbo].[WaterAccountCustomAttribute]
(
	[WaterAccountCustomAttributeID] INT NOT NULL IDENTITY(1,1) CONSTRAINT [PK_WaterAccountCustomAttribute_WaterAccountCustomAttributeID] PRIMARY KEY,
	[WaterAccountID] INT NOT NULL CONSTRAINT [FK_WaterAccountCustomAttribute_WaterAccount_WaterAccountID] FOREIGN KEY REFERENCES dbo.WaterAccount(WaterAccountID) CONSTRAINT [AK_WaterAccountCustomAttribute_WaterAccount] UNIQUE,
	[CustomAttributes] VARCHAR(MAX) NOT NULL DEFAULT ('{}')
)
