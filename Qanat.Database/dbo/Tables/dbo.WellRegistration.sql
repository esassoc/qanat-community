CREATE TABLE [dbo].[WellRegistration]
(
	[WellRegistrationID] int not null identity(1,1) constraint [PK_WellRegistration_WellRegistrationID] primary key,
	[GeographyID] int not null constraint [FK_WellRegistration_Geography_GeographyID] foreign key references dbo.[Geography]([GeographyID]),
	[WellID] int null constraint [FK_WellRegistration_Well_WellID] foreign key references dbo.[Well]([WellID]),
	[WellName] varchar(100) null,
	[LocationPoint] geometry null,
	[LocationPoint4326] geometry null,
	[WellRegistrationStatusID] int not null constraint [FK_WellRegistration_WellRegistrationStatus_WellRegistrationStatusID] foreign key references dbo.WellRegistrationStatus(WellRegistrationStatusID),
	[ParcelID] int null constraint [FK_WellRegistration_Parcel_ParcelID] foreign key references dbo.Parcel(ParcelID),
	[StateWCRNumber] varchar(100) null,
	[CountyWellPermitNumber] varchar(100) null,
	[DateDrilled] date null,
	[WellDepth] int null,
	[SubmitDate] datetime null,
	[ApprovalDate] datetime null,
	[CreateUserID] int null constraint [FK_WellRegistration_User_UserID] foreign key references dbo.[User](UserID),
	[CreateUserGuid] uniqueidentifier null,
	[CreateUserEmail] varchar (255) null,
	[ReferenceWellID] int null constraint [FK_WellRegistration_ReferenceWell_ReferenceWellID] foreign key references dbo.ReferenceWell(ReferenceWellID),
	[FairyshrimpWellID] int null,
	[ConfirmedWellLocation] bit not null default(0),
	[SelectedIrrigatedParcels] bit not null default(0)
)