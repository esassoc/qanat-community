MERGE INTO dbo.Permission AS Target
USING (VALUES
	(2, 'CustomRichTextRights', 'CustomRichTextRights'),
	(3, 'FieldDefinitionRights', 'FieldDefinitionRights'),
	(4, 'FileResourceRights', 'FileResourceRights'),
	(5, 'UserRights', 'UserRights'),
	(6, 'WaterAccountRights', 'WaterAccountRights'),
	(7, 'ParcelRights', 'ParcelRights'),
	(8, 'TagRights', 'TagRights'),
	(9, 'WellRights', 'WellRights'),
	(10, 'WaterTransactionRights', 'WaterTransactionRights'),
	(12, 'ReportingPeriodRights', 'ReportingPeriodRights'),
	(13, 'WaterTypeRights', 'WaterTypeRights'),
	(14, 'GeographyRights', 'GeographyRights'),
	(15, 'ExternalMapLayerRights', 'ExternalMapLayerRights'),
	(16, 'WaterAccountUserRights', 'WaterAccountUserRights'),
	(17, 'ZoneGroupRights', 'ZoneGroupRights'),
	(18, 'MonitoringWellRights', 'MonitoringWellRights'),
	(19, 'AllocationPlanRights', 'AllocationPlanRights'),
	(20, 'FrequentlyAskedQuestionRights', 'FrequentlyAskedQuestionRights'),
	(21, 'CustomAttributeRights', 'CustomAttributeRights'),
	(22, 'UsageEntityRights', 'UsageEntityRights'),
	(23, 'ModelRights', 'ModelRights'),
	(24, 'ScenarioRights', 'ScenarioRights'),
	(25, 'ScenarioRunRights', 'ScenarioRunRights')
)
AS Source (PermissionID, PermissionName, PermissionDisplayName)
ON Target.PermissionID = Source.PermissionID
WHEN MATCHED THEN
UPDATE SET
	PermissionName = Source.PermissionName,
	PermissionDisplayName = Source.PermissionDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (PermissionID, PermissionName, PermissionDisplayName)
	VALUES (PermissionID, PermissionName, PermissionDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;