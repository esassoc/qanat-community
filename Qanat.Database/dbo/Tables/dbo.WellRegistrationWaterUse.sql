CREATE TABLE [dbo].[WellRegistrationWaterUse]
(
	[WellRegistrationWaterUseID] int not null identity(1,1) constraint [PK_WellRegistrationWaterUse_WellRegistrationWaterUseID] primary key,
	[WellRegistrationID] int not null constraint [FK_WellRegistrationWaterUse_WellRegistration_WellRegistrationID] foreign key references dbo.WellRegistration(WellRegistrationID),
	[WellRegistrationWaterUseTypeID] int not null constraint FK_WellRegistrationWaterUse_WellRegistrationWaterUseType_WellRegistrationWaterUseTypeID foreign key references dbo.[WellRegistrationWaterUseType](WellRegistrationWaterUseTypeID),
	[WellRegistrationWaterUseDescription] varchar(200) null,

	constraint AK_WellRegistrationWaterUse_WellRegistrationID_WellRegistrationWaterUseTypeID unique (WellRegistrationID, WellRegistrationWaterUseTypeID)
)