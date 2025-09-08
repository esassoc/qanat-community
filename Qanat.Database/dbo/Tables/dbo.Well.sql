CREATE TABLE [dbo].[Well]
(
    [WellID]                    INT             NOT NULL IDENTITY(1, 1),
    [GeographyID]               INT             NOT NULL,
    [ParcelID]                  INT             NULL,
    
    [WellTypeID]                INT             NULL, --TODO: Make not nullable after Prod release. 
    [WellStatusID]              INT             NOT NULL DEFAULT(1),

    [WellName]                  VARCHAR(100)    NULL,
    [LocationPoint]             GEOMETRY        NULL,
    [LocationPoint4326]         GEOMETRY        NULL,
    [StateWCRNumber]            VARCHAR(100)    NULL,
    [CountyWellPermitNumber]    VARCHAR(100)    NULL,
    [DateDrilled]               DATE            NULL,
    [ParcelIsManualOverride]    BIT             NOT NULL,
    [Notes]                     VARCHAR(500)    NULL,

    -- Technical Info
    [WellDepth]                 INT             NULL,
    [CasingDiameter]            INT             NULL,
    [TopOfPerforations]         INT             NULL,
    [BottomOfPerforations]      INT             NULL,
    [ElectricMeterNumber]       VARCHAR(100)    NULL,

    [SchemotoInstance]		    VARCHAR(MAX)    NULL,

    [CreateDate]                DATETIME        NULL,

    CONSTRAINT [PK_Well_WellID]                     PRIMARY KEY ([WellID]),
    CONSTRAINT [FK_Well_Geography_GeographyID]      FOREIGN KEY ([GeographyID])     REFERENCES dbo.[Geography]([GeographyID]),
    CONSTRAINT [FK_Well_Parcel_ParcelID]            FOREIGN KEY ([ParcelID])        REFERENCES dbo.[Parcel]([ParcelID]),
    CONSTRAINT [FK_Well_WellType_WellTypeID]        FOREIGN KEY ([WellTypeID])      REFERENCES dbo.[WellType]([WellTypeID]),
    CONSTRAINT [FK_Well_WellStatus_WellStatusID]    FOREIGN KEY ([WellStatusID])    REFERENCES dbo.[WellStatus]([WellStatusID])
)
GO
