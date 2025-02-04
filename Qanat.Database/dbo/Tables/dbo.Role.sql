CREATE TABLE [dbo].[Role] (
    [RoleID]          INT           NOT NULL,
    [RoleName]        VARCHAR (100) NOT NULL,
    [RoleDisplayName] VARCHAR (100) NOT NULL,
    [RoleDescription] VARCHAR (255) NULL,
    [SortOrder]       INT           NOT NULL,
    [Rights]          NVARCHAR(MAX) NOT NULL DEFAULT '{}',
    [Flags]           NVARCHAR(MAX) NOT NULL DEFAULT '{}',

    CONSTRAINT [PK_Role_RoleID] PRIMARY KEY CLUSTERED ([RoleID] ASC),

    CONSTRAINT [AK_Role_RoleDisplayName] UNIQUE NONCLUSTERED ([RoleDisplayName] ASC),
    CONSTRAINT [AK_Role_RoleName] UNIQUE NONCLUSTERED ([RoleName] ASC)
);

