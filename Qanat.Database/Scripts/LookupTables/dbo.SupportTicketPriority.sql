MERGE INTO dbo.SupportTicketPriority AS Target
USING (VALUES
(1, 'High', 'High'),
(2, 'Medium', 'Medium'),
(3, 'Low', 'Low'),
(4, 'NotPrioritized', 'Not Prioritized')
)
AS Source (SupportTicketPriorityID, SupportTicketPriorityName, SupportTicketPriorityDisplayName)
ON Target.SupportTicketPriorityID = Source.SupportTicketPriorityID
WHEN MATCHED THEN
UPDATE SET
	SupportTicketPriorityName = Source.SupportTicketPriorityName,
	SupportTicketPriorityDisplayName = Source.SupportTicketPriorityDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (SupportTicketPriorityID, SupportTicketPriorityName, SupportTicketPriorityDisplayName)
	VALUES (SupportTicketPriorityID, SupportTicketPriorityName, SupportTicketPriorityDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
