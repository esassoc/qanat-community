CREATE TABLE [dbo].[WaterAccountUser](
	[WaterAccountUserID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_WaterAccountUser_WaterAccountUserID] PRIMARY KEY,
	[UserID] [int] NOT NULL CONSTRAINT [FK_WaterAccountUser_User_UserID] FOREIGN KEY REFERENCES [dbo].[User] ([UserID]),
	[WaterAccountID] [int] NOT NULL CONSTRAINT [FK_WaterAccountUser_WaterAccount_WaterAccountID] FOREIGN KEY REFERENCES [dbo].[WaterAccount] ([WaterAccountID]),
	[WaterAccountRoleID] [int] NOT NULL CONSTRAINT FK_WaterAccountUser_WaterAccountRole_WaterAccountRoleID FOREIGN KEY REFERENCES [dbo].[WaterAccountRole] ([WaterAccountRoleID]),
	[ClaimDate] [DateTime] NOT NULL,
	CONSTRAINT [AK_WaterAccountUser_Unique_WaterAccountID_UserID] unique ([WaterAccountID], [UserID])
)