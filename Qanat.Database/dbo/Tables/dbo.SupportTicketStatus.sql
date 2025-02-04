CREATE TABLE [dbo].[SupportTicketStatus] (
    [SupportTicketStatusID]          INT           NOT NULL,
    [SupportTicketStatusName]        VARCHAR (100) NOT NULL,
    [SupportTicketStatusDisplayName] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_SupportTicketStatus_SupportTicketStatusID] PRIMARY KEY CLUSTERED ([SupportTicketStatusID] ASC),
    CONSTRAINT [AK_SupportTicketStatus_SupportTicketStatusDisplayName] UNIQUE NONCLUSTERED ([SupportTicketStatusDisplayName] ASC),
    CONSTRAINT [AK_SupportTicketStatus_SupportTicketStatusName] UNIQUE NONCLUSTERED ([SupportTicketStatusName] ASC)
);