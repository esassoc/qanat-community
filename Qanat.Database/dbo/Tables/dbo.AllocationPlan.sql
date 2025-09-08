CREATE TABLE [dbo].[AllocationPlan]
(
	AllocationPlanID int NOT NULL identity(1,1) CONSTRAINT PK_AllocationPlan_AllocationPlanID PRIMARY KEY,
	GeographyID int NOT NULL CONSTRAINT FK_AllocationPlan_Geography_GeographyID FOREIGN KEY REFERENCES [dbo].[Geography]([GeographyID]),
	GeographyAllocationPlanConfigurationID int NOT NULL CONSTRAINT FK_AllocationPlan_GeographyAllocationPlanConfiguration_GeographyAllocationPlanConfigurationID FOREIGN KEY REFERENCES [dbo].[GeographyAllocationPlanConfiguration]([GeographyAllocationPlanConfigurationID]),
	ZoneID int NOT NULL CONSTRAINT FK_AllocationPlan_Zone_ZoneID FOREIGN KEY REFERENCES [dbo].[Zone]([ZoneID]),
	WaterTypeID int NOT NULL CONSTRAINT FK_AllocationPlan_WaterType_WaterTypeID FOREIGN KEY REFERENCES [dbo].[WaterType]([WaterTypeID]),
	LastUpdated datetime not null default(GETUTCDATE()),
	CONSTRAINT [AK_AllocationPlan_ZoneID_WaterTypeID] UNIQUE ([ZoneID], [WaterTypeID]),
)