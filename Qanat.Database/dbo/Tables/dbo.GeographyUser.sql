CREATE TABLE [dbo].[GeographyUser] (
    [GeographyUserID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_GeographyUser_GeographyUserID] PRIMARY KEY,
    [GeographyID] INT NOT NULL CONSTRAINT [FK_GeographyUser_Geography_GeographyID] FOREIGN KEY REFERENCES [dbo].[Geography] ([GeographyID]),
    [UserID] INT NOT NULL CONSTRAINT [FK_GeographyUser_User_UserID] FOREIGN KEY REFERENCES [dbo].[User] ([UserID]),
    [GeographyRoleID] INT NOT NULL CONSTRAINT [FK_GeographyUser_GeographyRole_GeographyRoleID] FOREIGN KEY REFERENCES [dbo].[GeographyRole] ([GeographyRoleID]),
    CONSTRAINT [AK_GeographyUser_Unique_GeographyID_UserID] UNIQUE NONCLUSTERED ([GeographyID] ASC, [UserID]),
);

