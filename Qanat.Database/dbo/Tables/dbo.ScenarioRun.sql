CREATE TABLE dbo.ScenarioRun(
	ScenarioRunID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ScenarioRun_ScenarioRunID PRIMARY KEY,
	ScenarioRunStatusID int not NULL CONSTRAINT FK_ScenarioRun_ScenarioRunStatus_ScenarioRunStatusID FOREIGN KEY REFERENCES dbo.ScenarioRunStatus(ScenarioRunStatusID),
	ModelID int not null CONSTRAINT FK_ScenarioRun_Model_ModelID FOREIGN KEY REFERENCES dbo.Model(ModelID),
	ScenarioID int not null CONSTRAINT FK_ScenarioRun_Scenario_ScenarioID FOREIGN KEY REFERENCES dbo.Scenario(ScenarioID),
	UserID int NOT NULL CONSTRAINT FK_ScenarioRun_User_UserID FOREIGN KEY REFERENCES dbo.[User](UserID),
	CreateDate datetime NOT NULL,
	LastUpdateDate datetime NULL,
	GETRunID int NULL,
	GETErrorMessage varchar(1000) null,
	ActionName varchar(500) null
)