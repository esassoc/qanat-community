MERGE INTO dbo.GeographyRole AS Target
USING (VALUES
(1, 'WaterManager', 'Water Manager', '', 10, '{"UserRights":15,"WaterAccountRights":15,"ParcelRights":15,"WellRights":15,"WaterTransactionRights":15,"ReportingPeriodRights":15,"WaterTypeRights":15, "ExternalMapLayerRights": 15, "WaterAccountUserRights": 15, "ZoneGroupRights": 15, "MonitoringWellRights":15, "AllocationPlanRights": 15, "GeographyRights": 15, "CustomAttributeRights": 15, "UsageLocationRights": 15, "MeterRights": 15, "WellMeterReadingRights": 15, "StatementTemplateRights": 15}', '{"HasManagerDashboard":true,"CanReviewWells":true, "CanUseScenarioPlanner":true}'),
(2, 'Normal', 'Normal', '', 20, '{"UserRights":1,"WaterAccountRights":0,"ParcelRights":0,"WellRights":1,"WaterTransactionRights":0,"ReportingPeriodRights":1,"WaterTypeRights":1, "ExternalMapLayerRights": 1, "WaterAccountUserRights": 0, "ZoneGroupRights": 1, "MonitoringWellRights":1, "AllocationPlanRights": 1, "CustomAttributeRights": 0, "UsageLocationRights": 0, "MeterRights": 0, "WellMeterReadingRights": 0, "StatementTemplateRights": 0}', '{"HasManagerDashboard":false,"CanReviewWells":false, "CanUseScenarioPlanner":false}')
)
AS Source (GeographyRoleID, GeographyRoleName, GeographyRoleDisplayName, GeographyRoleDescription, SortOrder, Rights, Flags)
ON Target.GeographyRoleID = Source.GeographyRoleID
WHEN MATCHED THEN
UPDATE SET
	GeographyRoleName = Source.GeographyRoleName,
	GeographyRoleDisplayName = Source.GeographyRoleDisplayName,
	GeographyRoleDescription = Source.GeographyRoleDescription,
	SortOrder = Source.SortOrder,
	Rights = Source.Rights,
	Flags = Source.Flags
WHEN NOT MATCHED BY TARGET THEN
	INSERT (GeographyRoleID, GeographyRoleName, GeographyRoleDisplayName, GeographyRoleDescription, SortOrder, Rights, Flags)
	VALUES (GeographyRoleID, GeographyRoleName, GeographyRoleDisplayName, GeographyRoleDescription, SortOrder, Rights, Flags)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
