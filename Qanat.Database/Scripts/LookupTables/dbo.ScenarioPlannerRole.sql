MERGE INTO dbo.ScenarioPlannerRole AS Target
USING (VALUES
	(1, 'NoAccess',		'No Access',		NULL,	10, '{"ModelRights": 0, "ScenarioRights": 0, "ScenarioRunRights": 0}', '{}'),
	(2, 'ScenarioUser', 'Scenario User',	NULL,	20, '{"ModelRights": 15, "ScenarioRights": 15, "ScenarioRunRights": 15}', '{}') -- MK 1/23/2025: Scenario Users have full rights for now, we could add another role for just "viewer" if we wanted some users to be able to see outputs but not generate them.
)
AS Source (ScenarioPlannerRoleID, ScenarioPlannerRoleName, ScenarioPlannerRoleDisplayName, ScenarioPlannerRoleDescription, SortOrder, Rights, Flags)
ON Target.ScenarioPlannerRoleID = Source.ScenarioPlannerRoleID
WHEN MATCHED THEN
UPDATE SET
	ScenarioPlannerRoleName = Source.ScenarioPlannerRoleName,
	ScenarioPlannerRoleDisplayName = Source.ScenarioPlannerRoleDisplayName,
	ScenarioPlannerRoleDescription = Source.ScenarioPlannerRoleDescription,
	SortOrder = Source.SortOrder,
	Rights = Source.Rights,
	Flags = Source.Flags
WHEN NOT MATCHED BY TARGET THEN
	INSERT (ScenarioPlannerRoleID, ScenarioPlannerRoleName, ScenarioPlannerRoleDisplayName, ScenarioPlannerRoleDescription, SortOrder, Rights, Flags)
	VALUES (ScenarioPlannerRoleID, ScenarioPlannerRoleName, ScenarioPlannerRoleDisplayName, ScenarioPlannerRoleDescription, SortOrder, Rights, Flags)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;