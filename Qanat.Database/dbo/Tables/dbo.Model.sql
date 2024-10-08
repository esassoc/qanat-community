CREATE TABLE dbo.Model(
	ModelID int NOT NULL CONSTRAINT PK_Model_ModelID PRIMARY KEY,
	ModelSubbasin varchar(100) not null DEFAULT 'Merced',
    ModelName varchar(100) not null constraint AK_Model_ModelName unique,
	ModelShortName varchar(50) not null constraint AK_Model_ModelShortName unique,
	ModelDescription [dbo].[html] not null,
	ModelEngine varchar(50) not null DEFAULT 'IWFM',
	[GETModelID] int not null,
	ModelImage varchar(max) null
)