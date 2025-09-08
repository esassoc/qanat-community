CREATE TABLE [dbo].[UsageLocationHistory]
(
	[UsageLocationHistoryID]		INT				NOT NULL IDENTITY(1,1),
	[GeographyID]					INT				NOT NULL, 
	[UsageLocationID]				INT				NOT NULL,

	[UsageLocationTypeID]			INT				NULL,
	[Note]							VARCHAR(MAX)	NULL,
															 
    [CreateDate]					DATETIME		NOT NULL DEFAULT(GETUTCDATE()),       
    [CreateUserID]					INT				NOT NULL,

	CONSTRAINT [PK_UsageLocationHistory_UsageLocationHistoryID]					PRIMARY KEY ([UsageLocationHistoryID]),

	CONSTRAINT [FK_UsageLocationHistory_Geography_GeographyID]					FOREIGN KEY ([GeographyID])				REFERENCES dbo.[Geography]([GeographyID]),
	CONSTRAINT [FK_UsageLocationHistory_UsageLocation_UsageLocationID]			FOREIGN KEY ([UsageLocationID])			REFERENCES dbo.[UsageLocation]([UsageLocationID]),
	CONSTRAINT [FK_UsageLocationHistory_UsageLocationType_UsageLocationTypeID]	FOREIGN KEY ([UsageLocationTypeID])		REFERENCES dbo.[UsageLocationType]([UsageLocationTypeID]),
	CONSTRAINT [FK_UsageLocationHistory_User_CreateUserID]						FOREIGN KEY ([CreateUserID])			REFERENCES dbo.[User]([UserID]),
)
GO

CREATE INDEX IX_UsageLocationHistory_GeographyID ON dbo.UsageLocationHistory(GeographyID);
GO

CREATE INDEX IX_UsageLocationHistory_UsageLocationID ON dbo.UsageLocationHistory(UsageLocationID);
GO

CREATE INDEX IX_UsageLocationHistory_UsageLocationTypeID on dbo.UsageLocationHistory(UsageLocationTypeID);
GO

CREATE INDEX IX_UsageLocationHistory_CreateUserID on dbo.UsageLocationHistory(CreateUserID);
GO