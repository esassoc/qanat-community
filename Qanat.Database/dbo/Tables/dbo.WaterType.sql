CREATE TABLE [dbo].[WaterType](
	[WaterTypeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_WaterType_WaterTypeID] PRIMARY KEY,
	[GeographyID] int not null constraint [FK_WaterType_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[IsActive] [bit] NOT NULL,
	[WaterTypeName] [varchar](50) NOT NULL,
	[IsAppliedProportionally] [bit] NOT NULL,
	[WaterTypeDefinition] [dbo].[html] NULL,
	[IsSourcedFromApi] [bit] NOT NULL,
	[SortOrder] [int] NOT NULL,
	[WaterTypeSlug] [varchar](50) NOT NULL,
	[WaterTypeColor] [varchar](7) NOT NULL,
	CONSTRAINT [AK_WaterType_Unique_WaterTypeID_GeographyID] unique ([WaterTypeID], [GeographyID]),
	CONSTRAINT [AK_WaterType_WaterTypeName_GeographyID] UNIQUE ([WaterTypeName], [GeographyID])
)