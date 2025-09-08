CREATE TABLE [dbo].[SupportTicket]
(
	SupportTicketID int NOT NULL identity(1,1) CONSTRAINT PK_SupportTicket_SupportTicketID PRIMARY KEY,
	GeographyID int NOT NULL CONSTRAINT FK_SupportTicket_Geography_GeographyID FOREIGN KEY REFERENCES [dbo].[Geography]([GeographyID]),
	[Description] [dbo].[html] Not null,
	SupportTicketStatusID int Not null CONSTRAINT FK_SupportTicket_SupportTicketStatus_SupportTicketStatusID FOREIGN KEY REFERENCES [dbo].[SupportTicketStatus]([SupportTicketStatusID]),
	[SupportTicketPriorityID] int not null Constraint FK_SupportTicket_SupportTicketPriority_SupportTicketPriorityID Foreign Key References [dbo].[SupportTicketPriority]([SupportTicketPriorityID]),
	[SupportTicketQuestionTypeID] int null Constraint FK_SupportTicket_SupportTicketQuestionType_SupportTicketQuestionTypeID Foreign Key References [dbo].[SupportTicketQuestionType]([SupportTicketQuestionTypeID]),
	WaterAccountID int null CONSTRAINT FK_SupportTicket_WaterAccount_WaterAccountID FOREIGN KEY REFERENCES [dbo].[WaterAccount]([WaterAccountID]),
	DateCreated datetime not null,
	DateUpdated datetime null,
	DateClosed datetime null,
	CreateUserID int null CONSTRAINT FK_SupportTicket_User_CreateUserID FOREIGN KEY REFERENCES [dbo].[User]([UserID]),
	AssignedUserID int null CONSTRAINT FK_SupportTicket_User_AssignedUserID FOREIGN KEY REFERENCES [dbo].[User]([UserID]),
	ContactFirstName varchar(50) not null,
	ContactLastName varchar(50) null,
	ContactEmail varchar(50) null,
	ContactPhoneNumber varchar(30) null
)