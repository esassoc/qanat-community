CREATE TABLE dbo.ScenarioRunStatus
(
	ScenarioRunStatusID int NOT NULL CONSTRAINT PK_ScenarioRunStatus_ScenarioRunStatusID PRIMARY KEY,
	ScenarioRunStatusName varchar(50) NOT NULL CONSTRAINT AK_ScenarioRunStatus_ScenarioRunStatusName UNIQUE,
	ScenarioRunStatusDisplayName varchar(50) NOT NULL CONSTRAINT AK_ScenarioRunStatus_ScenarioRunStatusDisplayName UNIQUE,
	GETRunStatusID int null, 
    IsTerminal bit NOT NULL
)