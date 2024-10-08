CREATE TABLE [dbo].[GeographyRole] (
    [GeographyRoleID]          INT           NOT NULL,
    [GeographyRoleName]        VARCHAR (100) NOT NULL,
    [GeographyRoleDisplayName] VARCHAR (100) NOT NULL,
    [GeographyRoleDescription] VARCHAR (255) NULL,
    [SortOrder]       INT           NOT NULL,
    [Rights]          NVARCHAR(MAX) NOT NULL DEFAULT '{}',
    [Flags]           NVARCHAR(MAX) NOT NULL DEFAULT '{}',
    CONSTRAINT [PK_GeographyRole_GeographyRoleID] PRIMARY KEY CLUSTERED ([GeographyRoleID] ASC),
    CONSTRAINT [AK_GeographyRole_GeographyRoleDisplayName] UNIQUE NONCLUSTERED ([GeographyRoleDisplayName] ASC),
    CONSTRAINT [AK_GeographyRole_GeographyRoleName] UNIQUE NONCLUSTERED ([GeographyRoleName] ASC)
);

