CREATE TABLE [dbo].[UsageEntityCrop]
(
	[UsageEntityCropID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_UsageEntityCrop_UsageEntityCropID] PRIMARY KEY,
	[UsageEntityID] int not null constraint [FK_UsageEntityCrop_UsageEntity_UsageEntityID] foreign key references dbo.[UsageEntity]([UsageEntityID]) ON DELETE CASCADE,
	[UsageEntityCropName] [nvarchar](100) NOT NULL,
)
GO

