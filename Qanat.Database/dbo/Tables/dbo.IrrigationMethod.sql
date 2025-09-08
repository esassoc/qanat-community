CREATE TABLE [dbo].[IrrigationMethod]
(
    [IrrigationMethodID]        INT             NOT NULL IDENTITY(1,1),
    [GeographyID]   			INT             NOT NULL,        

    [Name]                      VARCHAR(255)    NOT NULL,
    [SystemType]                VARCHAR(255)    NOT NULL,
    [EfficiencyAsPercentage]    INT             NOT NULL,          
    [DisplayOrder]              INT             NOT NULL,

    CONSTRAINT [PK_IrrigationMethod_IrrigationMethodID]         PRIMARY KEY ([IrrigationMethodID]),
    CONSTRAINT [FK_IrrigationMethod_Geography_GeographyID]      FOREIGN KEY ([GeographyID])             REFERENCES dbo.[Geography]([GeographyID]),
)