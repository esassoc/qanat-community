MERGE INTO dbo.SupportTicketStatus AS Target
USING (VALUES
(1, 'Unassigned', 'Unassigned'),
(2, 'Assigned', 'Assigned'),
(3, 'Closed', 'Closed')
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
