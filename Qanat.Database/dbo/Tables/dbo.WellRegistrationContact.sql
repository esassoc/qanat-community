CREATE TABLE dbo.WellRegistrationContact
(
	WellRegistrationContactID int not null identity(1,1) constraint PK_WellRegistrationContact_WellRegistrationContactID primary key,
	WellRegistrationID int not null constraint FK_WellRegistrationContact_WellRegistration_WellRegistrationID foreign key references dbo.WellRegistration(WellRegistrationID),
	WellRegistrationContactTypeID int not null constraint FK_WellRegistrationContact_WellRegistrationContactType_WellRegistrationContactTypeID foreign key references dbo.WellRegistrationContactType(WellRegistrationContactTypeID),
	ContactName varchar(100) not null,
	BusinessName varchar(100) null,
	StreetAddress varchar(100) not null,
	City varchar(100) not null,
	StateID int not null constraint FK_WellContact_State_StateID foreign key references dbo.[State](StateID),
	ZipCode varchar(10) not null,
	Phone varchar(20) not null,
	Email varchar(100) not null,
	CONSTRAINT AK_WellRegistrationContact_WellRegistrationID_RegistrationContactTypeID UNIQUE (WellRegistrationID, WellRegistrationContactTypeID) -- possible that we might remove this later, but we were running into production issues due to duplicated contacts. 3/21/23 SMG : RIO-553
)