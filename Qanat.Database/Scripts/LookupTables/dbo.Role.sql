MERGE INTO dbo.Role AS Target
USING (VALUES
(1, 'SystemAdmin', 'System Administrator', '', 30, '{"CustomRichTextRights":15,"FieldDefinitionRights":15,"FileResourceRights":15,"UserRights":15,"WaterAccountRights":15,"ParcelRights":15,"TagRights":15,"WellRights":15,"WaterTransactionRights":15,"ReportingPeriodRights":15,"WaterTypeRights":15,"GeographyRights":15, "ExternalMapLayerRights":15, "WaterAccountUserRights": 15, "ZoneGroupRights": 15, "MonitoringWellRights":15, "AllocationPlanRights": 15, "FrequentlyAskedQuestionRights": 15, "CustomAttributeRights": 15, "UsageLocationRights": 15, "MeterRights": 15, "WellMeterReadingRights": 15, "StatementTemplateRights": 15}', '{"CanImpersonateUsers":true,"HasManagerDashboard":true,"IsSystemAdmin":true,"CanClaimWaterAccounts":true,"CanRegisterWells":true,"CanReviewWells":true, "CanUseScenarioPlanner":true}'),
(2, 'NoAccess', 'No Access', '', 10, '{"CustomRichTextRights":0,"FieldDefinitionRights":0,"FileResourceRights":0,"UserRights":0,"WaterAccountRights":0,"ParcelRights":0,"TagRights":0,"WellRights":0,"WaterTransactionRights":0,"ReportingPeriodRights":0,"WaterTypeRights":0,"GeographyRights":0, "ExternalMapLayerRights":0, "WaterAccountUserRights": 0, "ZoneGroupRights": 0, "MonitoringWellRights": 0, "AllocationPlanRights": 0, "FrequentlyAskedQuestionRights": 1, "CustomAttributeRights": 0, "UsageLocationRights": 0, "MeterRights": 0, "WellMeterReadingRights": 0, "StatementTemplateRights": 0}', '{"CanImpersonateUsers":false,"HasManagerDashboard":false,"IsSystemAdmin":false,"CanClaimWaterAccounts":false,"CanRegisterWells":false,"CanReviewWells":false, "CanUseScenarioPlanner":false}'),
(3, 'Normal', 'Normal', '', 20, '{"CustomRichTextRights":1,"FieldDefinitionRights":1,"FileResourceRights":1,"UserRights":1,"WaterAccountRights":0,"ParcelRights":0,"TagRights":0,"WellRights":0,"WaterTransactionRights":0,"ReportingPeriodRights":0,"WaterTypeRights":0,"GeographyRights":1, "ExternalMapLayerRights":0, "WaterAccountUserRights": 0, "ZoneGroupRights": 1, "MonitoringWellRights": 0, "AllocationPlanRights": 0, "FrequentlyAskedQuestionRights": 1, "CustomAttributeRights": 0, "UsageLocationRights": 0, "MeterRights": 0, "WellMeterReadingRights": 0, "StatementTemplateRights": 0}', '{"CanImpersonateUsers":false,"HasManagerDashboard":false,"IsSystemAdmin":false,"CanClaimWaterAccounts":true,"CanRegisterWells":true,"CanReviewWells":false, "CanUseScenarioPlanner":false}'),
(4, 'PendingLogin', 'Pending Login', '', 10, '{"CustomRichTextRights":0,"FieldDefinitionRights":0,"FileResourceRights":0,"UserRights":0,"WaterAccountRights":0,"ParcelRights":0,"TagRights":0,"WellRights":0,"WaterTransactionRights":0,"ReportingPeriodRights":0,"WaterTypeRights":0,"GeographyRights":0, "ExternalMapLayerRights":0, "WaterAccountUserRights": 0, "ZoneGroupRights": 0, "MonitoringWellRights": 0, "AllocationPlanRights": 0, "FrequentlyAskedQuestionRights": 1, "CustomAttributeRights": 0, "UsageLocationRights": 0, "MeterRights": 0, "WellMeterReadingRights": 0, "StatementTemplateRights": 0}', '{"CanImpersonateUsers":false,"HasManagerDashboard":false,"IsSystemAdmin":false,"CanClaimWaterAccounts":false,"CanRegisterWells":false,"CanReviewWells":false, "CanUseScenarioPlanner":false}')
)
AS Source (RoleID, RoleName, RoleDisplayName, RoleDescription, SortOrder, Rights, Flags)
ON Target.RoleID = Source.RoleID
WHEN MATCHED THEN
UPDATE SET
	RoleName = Source.RoleName,
	RoleDisplayName = Source.RoleDisplayName,
	RoleDescription = Source.RoleDescription,
	SortOrder = Source.SortOrder,
	Rights = Source.Rights,
	Flags = Source.Flags
WHEN NOT MATCHED BY TARGET THEN
	INSERT (RoleID, RoleName, RoleDisplayName, RoleDescription, SortOrder, Rights, Flags)
	VALUES (RoleID, RoleName, RoleDisplayName, RoleDescription, SortOrder, Rights, Flags)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
