CREATE TABLE dbo.GeographyConfiguration(
	GeographyConfigurationID int NOT NULL CONSTRAINT PK_GeographyConfiguration_GeographyConfigurationID PRIMARY KEY,
	WellRegistryEnabled bit not null default(0),
	LandingPageEnabled bit not null default(0),
	MetersEnabled bit not null default(0),
	ZonePrecipMultipliersEnabled bit not null default(0)
)