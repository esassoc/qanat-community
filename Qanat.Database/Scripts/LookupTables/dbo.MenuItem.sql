MERGE INTO dbo.MenuItem AS Target
USING (VALUES
(1, 'View', 'View'),
(2, 'LearnMore', 'Learn More')
)
AS Source (MenuItemID, MenuItemName, MenuItemDisplayName)
ON Target.MenuItemID = Source.MenuItemID
WHEN MATCHED THEN
UPDATE SET
	MenuItemName = Source.MenuItemName,
	MenuItemDisplayName = Source.MenuItemDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (MenuItemID, MenuItemName, MenuItemDisplayName)
	VALUES (MenuItemID, MenuItemName, MenuItemDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;