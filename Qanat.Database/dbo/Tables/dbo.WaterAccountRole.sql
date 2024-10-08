CREATE TABLE [dbo].[WaterAccountRole] (
    [WaterAccountRoleID]          INT           NOT NULL,
    [WaterAccountRoleName]        VARCHAR (100) NOT NULL,
    [WaterAccountRoleDisplayName] VARCHAR (100) NOT NULL,
    [WaterAccountRoleDescription] VARCHAR (255) NULL,
    [SortOrder]       INT           NOT NULL,
    [Rights]          NVARCHAR(MAX) NOT NULL DEFAULT '{}',
    [Flags]           NVARCHAR(MAX) NOT NULL DEFAULT '{}',
    CONSTRAINT [PK_WaterAccountRole_WaterAccountRoleID] PRIMARY KEY CLUSTERED ([WaterAccountRoleID] ASC),
    CONSTRAINT [AK_WaterAccountRole_WaterAccountRoleDisplayName] UNIQUE NONCLUSTERED ([WaterAccountRoleDisplayName] ASC),
    CONSTRAINT [AK_WaterAccountRole_WaterAccountRoleName] UNIQUE NONCLUSTERED ([WaterAccountRoleName] ASC)
);

