CREATE TABLE [dbo].[Well]
(
	[WellID] int IDENTITY(1,1) NOT NULL constraint [PK_Well_WellID] primary key,
	[GeographyID] int not null constraint [FK_Well_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[ParcelID] int null constraint [FK_Well_Parcel_ParcelID] foreign key references dbo.Parcel(ParcelID),
	[WellName] varchar(100) null,
	[LocationPoint] geometry null,
	[LocationPoint4326] geometry null,
	[StateWCRNumber] varchar(100) null,
	[CountyWellPermitNumber] varchar(100) null,
	[DateDrilled] date null,
	[WellDepth] int null,
	[CreateDate] datetime null,
	[WellStatusID] int not null default 1 constraint [FK_Well_WellStatus_WellStatusID] foreign key references dbo.[WellStatus]([WellStatusID]),
	[Notes] varchar(500) null
)
