CREATE TABLE [dbo].[SupportTicketQuestionType] (
    [SupportTicketQuestionTypeID]          INT           NOT NULL,
    [SupportTicketQuestionTypeName]        VARCHAR (100) NOT NULL,
    [SupportTicketQuestionTypeDisplayName] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_SupportTicketQuestionType_SupportTicketQuestionTypeID] PRIMARY KEY CLUSTERED ([SupportTicketQuestionTypeID] ASC),
    CONSTRAINT [AK_SupportTicketQuestionType_SupportTicketQuestionTypeDisplayName] UNIQUE NONCLUSTERED ([SupportTicketQuestionTypeDisplayName] ASC),
    CONSTRAINT [AK_SupportTicketQuestionType_SupportTicketQuestionTypeName] UNIQUE NONCLUSTERED ([SupportTicketQuestionTypeName] ASC),
);