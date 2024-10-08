CREATE TABLE dbo.GETActionStatus
(
	GETActionStatusID int NOT NULL CONSTRAINT PK_GETActionStatus_GETActionStatusID PRIMARY KEY,
	GETActionStatusName varchar(50) NOT NULL CONSTRAINT AK_GETActionStatus_GETActionStatusName UNIQUE,
	GETActionStatusDisplayName varchar(50) NOT NULL CONSTRAINT AK_GETActionStatus_GETActionStatusDisplayName UNIQUE,
	GETRunStatusID int null, 
    IsTerminal bit NOT NULL
)