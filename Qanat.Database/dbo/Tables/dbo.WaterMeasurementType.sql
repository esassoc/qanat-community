CREATE TABLE [dbo].[WaterMeasurementType]
(
    [WaterMeasurementTypeID]                INT IDENTITY(1,1)   NOT NULL,
    [GeographyID]                           INT                 NOT NULL,
    [WaterMeasurementCategoryTypeID]        INT                 NOT NULL,
    [IsActive]                              BIT                 NOT NULL,
    [WaterMeasurementTypeName]              VARCHAR(50)         NOT NULL,
    [SortOrder]                             INT                 NOT NULL,
    [IsUserEditable]                        BIT                 NOT NULL,
    [ShowToLandowner]                       BIT                 NOT NULL,
    
    [WaterMeasurementCalculationTypeID]     INT                 NULL,
    [CalculationJSON]                       VARCHAR(MAX)        NULL,

    CONSTRAINT [PK_WaterMeasurementType_WaterMeasurementTypeID]                                         PRIMARY KEY ([WaterMeasurementTypeID]),
    
    CONSTRAINT [FK_WaterMeasurementType_Geography_GeographyID]                                          FOREIGN KEY ([GeographyID])                         REFERENCES dbo.[Geography]([GeographyID]),
    CONSTRAINT [FK_WaterMeasurement_WaterMeasurementCategoryType_WaterMeasurementCategoryTypeID]        FOREIGN KEY ([WaterMeasurementCategoryTypeID])      REFERENCES dbo.[WaterMeasurementCategoryType]([WaterMeasurementCategoryTypeID]),
    CONSTRAINT [FK_WaterMeasurement_WaterMeasurementCalculationType_WaterMeasurementCalculationTypeID]  FOREIGN KEY ([WaterMeasurementCalculationTypeID])   REFERENCES dbo.[WaterMeasurementCalculationType]([WaterMeasurementCalculationTypeID]),
    
    CONSTRAINT [AK_WaterMeasurementType_WaterMeasurementTypeName_GeographyID]                           UNIQUE ([WaterMeasurementTypeName], [GeographyID])
)