CREATE TABLE [dbo].[User] (
    [UserID]                     INT              IDENTITY (1, 1) NOT NULL,
    [UserGuid]                   UNIQUEIDENTIFIER NULL,
    [FirstName]                  VARCHAR (100)    NOT NULL,
    [LastName]                   VARCHAR (100)    NOT NULL,
    [Email]                      VARCHAR (255)    NOT NULL,
    [Phone]                      VARCHAR (30)     NULL,
    [RoleID]                     INT              NOT NULL,
    [CreateDate]                 DATETIME         NOT NULL,
    [UpdateDate]                 DATETIME         NULL,
    [LastActivityDate]           DATETIME         NULL,
    [IsActive]                   BIT              NOT NULL,
    [ReceiveSupportEmails]       BIT              NOT NULL,
    [LoginName]                  VARCHAR (128)    NULL,
    [Company]                    VARCHAR (100)    NULL,
    ImpersonatedUserGuid         uniqueidentifier NULL,
    [IsClientUser]               BIT              NOT NULL DEFAULT (0), 
    GETRunCustomerID             int null,
    GETRunUserID                 int null,
    CONSTRAINT [PK_User_UserID] PRIMARY KEY CLUSTERED ([UserID] ASC),
    CONSTRAINT [FK_User_Role_RoleID] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Role] ([RoleID]),
    CONSTRAINT [AK_User_Email] UNIQUE NONCLUSTERED ([Email] ASC)
);

