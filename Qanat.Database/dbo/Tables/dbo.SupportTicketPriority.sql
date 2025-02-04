CREATE TABLE [dbo].[SupportTicketPriority] (
    [SupportTicketPriorityID]          INT           NOT NULL,
    [SupportTicketPriorityName]        VARCHAR (100) NOT NULL,
    [SupportTicketPriorityDisplayName] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_SupportTicketPriority_SupportTicketPriorityID] PRIMARY KEY CLUSTERED ([SupportTicketPriorityID] ASC),
    CONSTRAINT [AK_SupportTicketPriority_SupportTicketPriorityDisplayName] UNIQUE NONCLUSTERED ([SupportTicketPriorityDisplayName] ASC),
    CONSTRAINT [AK_SupportTicketPriority_SupportTicketPriorityName] UNIQUE NONCLUSTERED ([SupportTicketPriorityName] ASC)
);