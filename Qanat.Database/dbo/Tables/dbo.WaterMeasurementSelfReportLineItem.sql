CREATE TABLE [dbo].[WaterMeasurementSelfReportLineItem]
(
    [WaterMeasurementSelfReportLineItemID]  INT             NOT NULL IDENTITY(1,1),

    [WaterMeasurementSelfReportID]          INT             NOT NULL,
    [ParcelID]						        INT             NOT NULL, 
    [IrrigationMethodID]                    INT             NOT NULL,

    [JanuaryOverrideValueInAcreFeet]        DECIMAL(20, 4)  NULL,
    [FebruaryOverrideValueInAcreFeet]       DECIMAL(20, 4)  NULL,
    [MarchOverrideValueInAcreFeet]          DECIMAL(20, 4)  NULL,
    [AprilOverrideValueInAcreFeet]          DECIMAL(20, 4)  NULL,
    [MayOverrideValueInAcreFeet]            DECIMAL(20, 4)  NULL,
    [JuneOverrideValueInAcreFeet]           DECIMAL(20, 4)  NULL,
    [JulyOverrideValueInAcreFeet]           DECIMAL(20, 4)  NULL,
    [AugustOverrideValueInAcreFeet]         DECIMAL(20, 4)  NULL,
    [SeptemberOverrideValueInAcreFeet]      DECIMAL(20, 4)  NULL,
    [OctoberOverrideValueInAcreFeet]        DECIMAL(20, 4)  NULL,
    [NovemberOverrideValueInAcreFeet]       DECIMAL(20, 4)  NULL,
    [DecemberOverrideValueInAcreFeet]       DECIMAL(20, 4)  NULL,

    [CreateDate]                            DATETIME        NOT NULL,       
    [CreateUserID]                          INT             NOT NULL,   
    [UpdateDate]                            DATETIME        NULL,
    [UpdateUserID]                          INT             NULL,

    CONSTRAINT [PK_WaterMeasurementSelfReportLineItem_WaterMeasurementSelfReportLineItemID]                     PRIMARY KEY ([WaterMeasurementSelfReportLineItemID]),

    CONSTRAINT [FK_WaterMeasurementSelfReportLineItem_WaterMeasurementSelfReport_WaterMeasurementSelfReportID]  FOREIGN KEY ([WaterMeasurementSelfReportID])    REFERENCES dbo.[WaterMeasurementSelfReport]([WaterMeasurementSelfReportID]),
    CONSTRAINT [FK_WaterMeasurementSelfReportLineItem_Parcel_ParcelID]                                          FOREIGN KEY ([ParcelID])                        REFERENCES dbo.[Parcel]([ParcelID]),
    CONSTRAINT [FK_WaterMeasurementSelfReportLineItem_IrrigationMethod_IrrigationMethodID]                      FOREIGN KEY ([IrrigationMethodID])              REFERENCES dbo.[IrrigationMethod]([IrrigationMethodID]),
    CONSTRAINT [FK_WaterMeasurementSelfReportLineItem_User_CreateUserID]                                        FOREIGN KEY ([CreateUserID])                    REFERENCES dbo.[User]([UserID]),
    CONSTRAINT [FK_WaterMeasurementSelfReportLineItem_User_UpdateUserID]                                        FOREIGN KEY ([UpdateUserID])                    REFERENCES dbo.[User]([UserID]),
    
    CONSTRAINT [AK_WaterMeasurementSelfReportLineItem_WaterMeasurementSelfReportID_Parcel]                      UNIQUE ([WaterMeasurementSelfReportID], [ParcelID])
)