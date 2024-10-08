CREATE TABLE [dbo].[GeographyAllocationPlanConfiguration]
(
	GeographyAllocationPlanConfigurationID int NOT NULL identity(1,1) CONSTRAINT PK_GeographyAllocationPlanConfiguration_GeographyAllocationPlanConfigurationID PRIMARY KEY,
	GeographyID int NOT NULL CONSTRAINT FK_GeographyAllocationPlanConfiguration_Geography_GeographyID FOREIGN KEY REFERENCES [dbo].[Geography]([GeographyID]),
	ZoneGroupID int NOT NULl CONSTRAINT FK_GeographyAllocationPlanConfiguration_ZoneGroup_ZoneGroupID FOREIGN kEy REFERENCES [dbo].[ZoneGroup]([ZoneGroupID]),
	StartYear int NOT NULL,
	EndYear int NOT NULL,
	IsActive bit NOT NULL,
	IsVisibleToLandowners bit NOT NULL,
	IsVisibleToPublic bit null,
	AllocationPlansDescription [dbo].[html] NULL,
	CONSTRAINT [AK_GeographyAllocationPlanConfiguration_GeographyID] UNIQUE ([GeographyID]),
	CONSTRAINT [CHK_GeographyAllocationPlanConfiguration_EndYear_GreaterThan_StartYear] CHECK (EndYear > StartYear)
)