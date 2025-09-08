CREATE TABLE [dbo].[UsageLocationCrop]
(
	[UsageLocationCropID]		INT				NOT NULL IDENTITY(1,1),
	[UsageLocationID]			INT				NOT NULL,
	[Name]						NVARCHAR(100)	NOT NULL,

	CONSTRAINT [PK_UsageLocationCrop_UsageLocationCropID]				PRIMARY KEY([UsageLocationCropID]),

	CONSTRAINT [FK_UsageLocationCrop_UsageLocation_UsageLocationID]		FOREIGN KEY([UsageLocationID]) REFERENCES dbo.[UsageLocation]([UsageLocationID]) ON DELETE CASCADE,
)
GO

CREATE INDEX IX_UsageLocationCrop_UsageLocationID on dbo.UsageLocationCrop(UsageLocationID);
GO