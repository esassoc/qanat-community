CREATE TABLE [dbo].[AllocationPlanPeriod]
(
	AllocationPlanPeriodID int NOT NULL identity(1,1) CONSTRAINT PK_AllocationPlanPeriod_AllocationPlanPeriodID PRIMARY KEY,
	AllocationPlanID int NOT NULL CONSTRAINT FK_AllocationPlanPeriod_AllocationPlan_AllocationPlanID FOREIGN KEY REFERENCES [dbo].[AllocationPlan]([AllocationPlanID]),
	AllocationPeriodName varchar(250) NOT NULL,
	AllocationAcreFeetPerAcre decimal(10,2) NOT NULL,
	NumberOfYears int NOT NULL,
	StartYear int NOT NULL,
	EnableCarryOver bit NOT NULL,
	CarryOverNumberOfYears INT NULL,
	CarryOverDepreciationRate decimal(10,2) null,
	EnableBorrowForward bit NOT NULL,
	BorrowForwardNumberOfYears int NULL,
	CONSTRAINT [CK_AllocationPlanPeriod_CarryOverDepreciationRate] CHECK (CarryOverDepreciationRate is null or (CarryOverDepreciationRate >= 0 and CarryOverDepreciationRate <=1)),
	CONSTRAINT [AK_AllocationPlanPeriod_AllocationPlanID_AllocationPeriodName] UNIQUE (AllocationPlanID, AllocationPeriodName)
)