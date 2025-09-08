CREATE TABLE [dbo].[Flag]
(
	FlagID int NOT NULL CONSTRAINT PK_Flag_FlagID PRIMARY KEY,
	FlagName varchar(100) NOT NULL CONSTRAINT AK_Flag_FlagName UNIQUE,
	FlagDisplayName varchar(100) NOT NULL CONSTRAINT AK_Flag_FlagDisplayName UNIQUE
)