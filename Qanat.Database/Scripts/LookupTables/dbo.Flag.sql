MERGE INTO dbo.Flag AS Target
USING (VALUES
(1, 'CanImpersonateUsers', 'CanImpersonateUsers'),
(2, 'HasManagerDashboard', 'HasManagerDashboard'),
(3, 'HasAdminDashboard', 'HasAdminDashboard'),
(4, 'CanClaimWaterAccounts', 'CanClaimWaterAccounts'),
(5, 'CanRegisterWells', 'CanRegisterWells'),
(6, 'CanReviewWells', 'CanReviewWells'),
(7, 'CanUseScenarioPlanner', 'CanUseScenarioPlanner')
)
AS Source (FlagID, FlagName, FlagDisplayName)
ON Target.FlagID = Source.FlagID
WHEN MATCHED THEN
UPDATE SET
	FlagName = Source.FlagName,
	FlagDisplayName = Source.FlagDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (FlagID, FlagName, FlagDisplayName)
	VALUES (FlagID, FlagName, FlagDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
