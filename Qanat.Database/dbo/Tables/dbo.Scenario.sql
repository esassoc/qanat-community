CREATE TABLE dbo.Scenario(
	ScenarioID int NOT NULL CONSTRAINT PK_Scenario_ScenarioID PRIMARY KEY,
    ScenarioName varchar(100) not null constraint AK_Scenario_ScenarioName unique,
	ScenarioShortName varchar(50) not null constraint AK_Scenario_ScenarioShortName unique,
	ScenarioDescription varchar(1000) not null,
	ScenarioImage varchar(max) null
)