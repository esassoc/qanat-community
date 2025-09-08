CREATE TABLE [dbo].[User] (
    [UserID]                     INT              NOT NULL IDENTITY (1, 1),
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
    ImpersonatedUserGuid         UNIQUEIDENTIFIER NULL,
    [IsClientUser]               BIT              NOT NULL DEFAULT (0),     

    [ScenarioPlannerRoleID]      INT              NOT NULL DEFAULT(1),
    GETRunCustomerID             INT              NULL,
    GETRunUserID                 INT              NULL,
    ApiKey                       UNIQUEIDENTIFIER NULL,

    CONSTRAINT [PK_User_UserID]                                     PRIMARY KEY CLUSTERED ([UserID] ASC),
    
    CONSTRAINT [FK_User_Role_RoleID]                                FOREIGN KEY ([RoleID])                  REFERENCES [dbo].[Role] ([RoleID]),
    CONSTRAINT [FK_User_ScenarioPlannerRole_ScenarioPlannerRoleID]  FOREIGN KEY ([ScenarioPlannerRoleID])   REFERENCES [dbo].[ScenarioPlannerRole] ([ScenarioPlannerRoleID]),

    CONSTRAINT [AK_User_Email]                                      UNIQUE                                  NONCLUSTERED ([Email] ASC)
);

