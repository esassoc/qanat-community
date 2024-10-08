CREATE TABLE [dbo].[ReferenceWell]
(
	[ReferenceWellID] INT NOT NULL IDENTITY(1, 1) CONSTRAINT PK_ReferenceWell_ReferenceWellID PRIMARY KEY,
	[GeographyID] int not null constraint [FK_ReferenceWell_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[LocationPoint4326] GEOMETRY NOT NULL,
	[WellName] varchar(100) not null CONSTRAINT [AK_ReferenceWell_GeographyID_WellName] UNIQUE (GeographyID, WellName),
	[CountyWellPermitNo] VARCHAR(10) NULL,
	[WellDepth] INT NULL,
	[StateWCRNumber] VARCHAR(15) NULL,
	[DateDrilled] date null
)