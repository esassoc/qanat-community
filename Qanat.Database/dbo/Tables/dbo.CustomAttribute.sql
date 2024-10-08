CREATE TABLE dbo.CustomAttribute (
	CustomAttributeID int NOT NULL identity(1, 1) CONSTRAINT PK_CustomAttribute_CustomAttributeID PRIMARY KEY,
	GeographyID int NOT NULL CONSTRAINT FK_CustomAttribute_Geography_GeographyID FOREIGN KEY REFERENCES dbo.[Geography](GeographyID),
	CustomAttributeTypeID int NOT NULL CONSTRAINT FK_CustomAttribute_CustomAttributeType_CustomAttributeTypeID FOREIGN KEY REFERENCES dbo.CustomAttributeType([CustomAttributeTypeID]),
	CustomAttributeName varchar(60) NOT NULL
)