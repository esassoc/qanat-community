CREATE TABLE [dbo].[ScenarioPlannerRole] (
    [ScenarioPlannerRoleID]             INT           NOT NULL,
    [ScenarioPlannerRoleName]           VARCHAR (100) NOT NULL,
    [ScenarioPlannerRoleDisplayName]    VARCHAR (100) NOT NULL,
    [ScenarioPlannerRoleDescription]    VARCHAR (255) NULL,
    [SortOrder]                         INT           NOT NULL,
    [Rights]                            NVARCHAR(MAX) NOT NULL DEFAULT '{}',
    [Flags]                             NVARCHAR(MAX) NOT NULL DEFAULT '{}',

    CONSTRAINT [PK_ScenarioPlannerRole_ScenarioPlannerRoleID]             PRIMARY KEY CLUSTERED ([ScenarioPlannerRoleID] ASC),

    CONSTRAINT [AK_ScenarioPlannerRole_ScenarioPlannerRoleDisplayName]    UNIQUE NONCLUSTERED ([ScenarioPlannerRoleDisplayName] ASC),
    CONSTRAINT [AK_ScenarioPlannerRole_ScenarioPlannerRoleName]           UNIQUE NONCLUSTERED ([ScenarioPlannerRoleName] ASC)
);

