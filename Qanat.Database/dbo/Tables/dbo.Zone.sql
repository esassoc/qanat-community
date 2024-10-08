CREATE TABLE [dbo].[Zone](
	[ZoneID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Zone_ZoneID] PRIMARY KEY,
	[ZoneGroupID] int not null constraint [FK_Zone_ZoneGroup_ZoneGroupID] foreign key references dbo.[ZoneGroup]([ZoneGroupID]),
	[ZoneName] varchar(100) not null,
	[ZoneSlug] varchar(100) not null,
	[ZoneDescription] varchar(200) null,
	[ZoneColor] varchar(7) not null,
	[ZoneAccentColor] varchar(7) not null,
	[PrecipMultiplier] [decimal](4, 2) null,
	[SortOrder] int not null,
	CONSTRAINT [AK_Zone_Unique_ZoneName_ZoneGroupID] unique ([ZoneName], [ZoneGroupID]),
	CONSTRAINT [AK_Zone_Unique_ZoneSlug_ZoneGroupID] unique ([ZoneSlug], [ZoneGroupID])
)