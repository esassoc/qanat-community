MERGE INTO dbo.SupportTicketQuestionType AS Target
USING (VALUES
(1, 'AccessingData', 'I need help accessing my Water Account / Parcel / Usage Data'),
(2, 'PolicyQuestion', 'I have a question about policies, rules, fees, etc'),
(3, 'InterpretingDataQuestion', 'I need help interpreting my water usage or allocations'),
(4, 'LogInQuestion', 'I can''t log in or my account isn''t configured'),
(5, 'Bug', 'I ran into a bug or problem with the system'),
(6, 'Other', 'Other')
)
AS Source (SupportTicketQuestionTypeID, SupportTicketQuestionTypeName, SupportTicketQuestionTypeDisplayName)
ON Target.SupportTicketQuestionTypeID = Source.SupportTicketQuestionTypeID
WHEN MATCHED THEN
UPDATE SET
	SupportTicketQuestionTypeName = Source.SupportTicketQuestionTypeName,
	SupportTicketQuestionTypeDisplayName = Source.SupportTicketQuestionTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (SupportTicketQuestionTypeID, SupportTicketQuestionTypeName, SupportTicketQuestionTypeDisplayName)
	VALUES (SupportTicketQuestionTypeID, SupportTicketQuestionTypeName, SupportTicketQuestionTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
