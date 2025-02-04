CREATE TABLE [dbo].[SupportTicketNote](
	[SupportTicketNoteID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_SupportTicketNote_SupportTicketNoteID] PRIMARY KEY,
	[SupportTicketID] int not null constraint [FK_SupportTicketNote_SupportTicket_SupportTicketID] foreign key references dbo.[SupportTicket]([SupportTicketID]),
	[Message] dbo.html not NULL,
	[InternalNote] bit not null,
	[CreateUserID] int not null constraint [FK_SupportTicketNote_User_UserID] foreign key references dbo.[User](UserID),
	[CreateDate] [datetime] NOT NULL,
)