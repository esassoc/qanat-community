CREATE TABLE [dbo].[WaterAccount](
	[WaterAccountID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_WaterAccount_WaterAccountID] PRIMARY KEY,
	[GeographyID] int not null constraint [FK_Account_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[WaterAccountNumber]  AS (isnull([WaterAccountID]+(10000),(0))) CONSTRAINT [AK_Account_AccountNumber] UNIQUE,
	[WaterAccountName] [varchar](255) NULL,
	[Notes] [varchar](max) NULL,
	[UpdateDate] [datetime] NULL,
	[WaterAccountPIN] varchar(7) null,
	[CreateDate] [datetime] NOT NULL,
	[ContactName] varchar(255) null,
	[ContactAddress] varchar(500) null,
	CONSTRAINT [AK_WaterAccount_WaterAccountID_GeographyID] unique ([WaterAccountID], [GeographyID])
)