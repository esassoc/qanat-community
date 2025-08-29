CREATE TABLE [dbo].[WaterAccountContact](
	[WaterAccountContactID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_WaterAccountContact_WaterAccountContactID] PRIMARY KEY,
	[GeographyID] INT NOT NULL CONSTRAINT [FK_WaterAccountContact_Geography_GeographyID] FOREIGN KEY REFERENCES [dbo].[Geography]([GeographyID]),
	[ContactName] VARCHAR(255) NOT NULL,
	[ContactEmail] VARCHAR(100) NULL,
	[ContactPhoneNumber] VARCHAR (30) NULL,
	[Address] VARCHAR(500) NOT NULL,
	[SecondaryAddress] VARCHAR(100) NULL,
	[City] VARCHAR(100) NULL,
	[State] VARCHAR(20) NULL,
	[ZipCode] VARCHAR(20) NULL,
	FullAddress AS (case when datalength(City) > 0 and datalength([State]) > 0 and datalength(ZipCode) > 0 then concat([Address], iif(datalength(SecondaryAddress) = 0, '', ', ' + SecondaryAddress), ', ', City, ', ', [State], ' ', ZipCode) else [Address] end),
	[PrefersPhysicalCommunication] BIT NOT NULL DEFAULT 1,
	[AddressValidated] BIT NOT NULL DEFAULT 0,
	[AddressValidationJson] VARCHAR(MAX) NULL
)
GO;

CREATE INDEX IX_WaterAccountContact_GeographyID on dbo.WaterAccountContact(GeographyID);

GO;