MERGE INTO dbo.[WaterAccountRole] AS Target
USING (VALUES
(1, 'WaterAccountHolder', 'Account Holder', '', 10, '{"WaterAccountRights":15,"ParcelRights":15,"WaterTransactionRights":15,"WaterAccountUserRights":15,"AllocationPlanRights":1, "UsageLocationRights": 1, "MeterRights": 1, "WellMeterReadingRights": 1}', '{}'),
(2, 'WaterAccountViewer', 'Viewer', '', 20, '{"WaterAccountRights":1,"ParcelRights":1,"WaterTransactionRights":1,"WaterAccountUserRights":1,"AllocationPlanRights":1, "UsageLocationRights": 1, "MeterRights": 1, "WellMeterReadingRights": 1}', '{}')
)
AS Source (WaterAccountRoleID, WaterAccountRoleName, WaterAccountRoleDisplayName, WaterAccountRoleDescription, SortOrder, Rights, Flags)
ON Target.[WaterAccountRoleID] = Source.WaterAccountRoleID
WHEN MATCHED THEN
UPDATE SET
	[WaterAccountRoleName] = Source.WaterAccountRoleName,
	[WaterAccountRoleDisplayName] = Source.WaterAccountRoleDisplayName,
	[WaterAccountRoleDescription] = Source.WaterAccountRoleDescription,
	SortOrder = Source.SortOrder,
	Rights = Source.Rights,
	Flags = Source.Flags
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([WaterAccountRoleID], [WaterAccountRoleName], [WaterAccountRoleDisplayName], [WaterAccountRoleDescription], SortOrder, Rights, Flags)
	VALUES (WaterAccountRoleID, WaterAccountRoleName, WaterAccountRoleDisplayName, WaterAccountRoleDescription, SortOrder, Rights, Flags)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
