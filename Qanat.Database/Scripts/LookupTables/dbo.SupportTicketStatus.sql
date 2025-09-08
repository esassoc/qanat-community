MERGE INTO dbo.SupportTicketStatus AS Target
USING (VALUES
(1, 'Unassigned', 'Unassigned'),
(2, 'Active', 'Active'),
(3, 'Closed', 'Closed'),
(4, 'OnHold', 'On Hold')
)
AS Source (SupportTicketStatusID, SupportTicketStatusName, SupportTicketStatusDisplayName)
ON Target.SupportTicketStatusID = Source.SupportTicketStatusID
WHEN MATCHED THEN
UPDATE SET
	SupportTicketStatusName = Source.SupportTicketStatusName,
	SupportTicketStatusDisplayName = Source.SupportTicketStatusDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (SupportTicketStatusID, SupportTicketStatusName, SupportTicketStatusDisplayName)
	VALUES (SupportTicketStatusID, SupportTicketStatusName, SupportTicketStatusDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
