CREATE TABLE dbo.GETAction(
	GETActionID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_GETAction_GETActionID PRIMARY KEY,
	GETActionStatusID int not NULL CONSTRAINT FK_GETAction_GETActionStatus_GETActionStatusID FOREIGN KEY REFERENCES dbo.GETActionStatus(GETActionStatusID),
	ModelID int not null CONSTRAINT FK_GETAction_Model_ModelID FOREIGN KEY REFERENCES dbo.Model(ModelID),
	ScenarioID int not null CONSTRAINT FK_GETAction_Scenario_ScenarioID FOREIGN KEY REFERENCES dbo.Scenario(ScenarioID),
	UserID int NOT NULL CONSTRAINT FK_GETAction_User_UserID FOREIGN KEY REFERENCES dbo.[User](UserID),
	CreateDate datetime NOT NULL,
	LastUpdateDate datetime NULL,
	GETRunID int NULL,
	GETErrorMessage varchar(1000) null,
	ActionName varchar(500) null
)