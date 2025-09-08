CREATE TABLE [dbo].[ZoneGroup]
(
	[ZoneGroupID]				INT IDENTITY(1,1)	NOT NULL CONSTRAINT [PK_ZoneGroup_ZoneGroupID] PRIMARY KEY,
	[GeographyID]				INT					NOT NULL CONSTRAINT [FK_ZoneGroup_Geography_GeographyID] FOREIGN KEY REFERENCES dbo.[Geography]([GeographyID]),
	[ZoneGroupName]				VARCHAR(100)		NOT NULL,
	[ZoneGroupSlug]				VARCHAR(100)		NOT NULL,
	[ZoneGroupDescription]		VARCHAR(500)		NULL,
	[SortOrder]					INT					NOT NULL,
	[DisplayToAccountHolders]	BIT					NOT NULL DEFAULT(1),
	CONSTRAINT [AK_ZoneGroup_Unique_ZoneGroupName_GeographyID] UNIQUE ([ZoneGroupName], [GeographyID]),
	CONSTRAINT [AK_ZoneGroup_Unique_ZoneGroupSlug_GeographyID] UNIQUE ([ZoneGroupSlug], [GeographyID])
)