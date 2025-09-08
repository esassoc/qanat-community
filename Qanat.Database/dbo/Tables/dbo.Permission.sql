CREATE TABLE [dbo].[Permission]
(
	PermissionID int NOT NULL CONSTRAINT PK_Permission_PermissionID PRIMARY KEY,
	PermissionName varchar(100) NOT NULL CONSTRAINT AK_Permission_PermissionName UNIQUE,
	PermissionDisplayName varchar(100) NOT NULL CONSTRAINT AK_Permission_PermissionDisplayName UNIQUE
)
