CREATE TABLE dbo.MenuItem(
	MenuItemID int NOT NULL CONSTRAINT PK_MenuItem_MenuItemID PRIMARY KEY,
	MenuItemName varchar(100) NOT NULL CONSTRAINT AK_MenuItem_MenuItemName UNIQUE,
	MenuItemDisplayName varchar(100) NOT NULL CONSTRAINT AK_MenuItem_MenuItemDisplayName UNIQUE
);