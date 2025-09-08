CREATE TABLE [dbo].[WaterAccountUserStaging](
	[WaterAccountUserStagingID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_WaterAccountUserStaging_WaterAccountUserStagingID] PRIMARY KEY,
	[UserID] [int] NOT NULL CONSTRAINT [FK_WaterAccountUserStaging_User_UserID] FOREIGN KEY REFERENCES [dbo].[User] ([UserID]),
	[WaterAccountID] [int] NOT NULL CONSTRAINT [FK_WaterAccountUserStaging_WaterAccount_WaterAccountID] FOREIGN KEY REFERENCES [dbo].[WaterAccount] ([WaterAccountID]),
	[ClaimDate] [DateTime] NOT NULL
)