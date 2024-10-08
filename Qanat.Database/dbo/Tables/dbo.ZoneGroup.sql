CREATE TABLE [dbo].[ZoneGroup](
	[ZoneGroupID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ZoneGroup_ZoneGroupID] PRIMARY KEY,
	[GeographyID] int not null constraint [FK_ZoneGroup_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[ZoneGroupName] varchar(100) not null,
	[ZoneGroupSlug] varchar(100) not null,
	[ZoneGroupDescription] varchar(500) null,
	[SortOrder] int not null,
	CONSTRAINT [AK_ZoneGroup_Unique_ZoneGroupName_GeographyID] unique ([ZoneGroupName], [GeographyID]),
	CONSTRAINT [AK_ZoneGroup_Unique_ZoneGroupSlug_GeographyID] unique ([ZoneGroupSlug], [GeographyID])
)