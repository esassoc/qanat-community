CREATE TABLE dbo.ModelScenario(
	ModelScenarioID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ModelScenario_ModelScenarioID PRIMARY KEY,
    ModelID int not null constraint FK_ModelScenario_Model_ModelID foreign key references dbo.Model(ModelID),
    ScenarioID int not null constraint FK_ModelScenario_Scenario_ScenarioID foreign key references dbo.Scenario(ScenarioID),
    GETScenarioID int not null
)