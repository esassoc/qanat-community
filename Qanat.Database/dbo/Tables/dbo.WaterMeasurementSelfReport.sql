CREATE TABLE [dbo].[WaterMeasurementSelfReport]
(
    [WaterMeasurementSelfReportID]          INT         NOT NULL IDENTITY(1,1),

    [GeographyID]                           INT         NOT NULL,
    [WaterAccountID]                        INT			NOT NULL,
    [WaterMeasurementTypeID]                INT         NOT NULL,
    [ReportingPeriodID]                     INT         NOT NULL,

    [WaterMeasurementSelfReportStatusID]    INT         NOT NULL,

    [SubmittedDate]                         DATETIME    NULL,
    [ApprovedDate]                          DATETIME    NULL,
    [ReturnedDate]                          DATETIME    NULL,

    [CreateDate]                            DATETIME    NOT NULL,       
    [CreateUserID]                          INT         NOT NULL,   
    [UpdateDate]                            DATETIME    NULL,
    [UpdateUserID]                          INT         NULL,

    CONSTRAINT [PK_WaterMeasurementSelfReport_WaterMeasurementSelfReportID]     PRIMARY KEY ([WaterMeasurementSelfReportID]),
    
    CONSTRAINT [FK_WaterMeasurementSelfReport_Geography_GeographyID]                        FOREIGN KEY ([GeographyID])                         REFERENCES dbo.[Geography]([GeographyID]),
    CONSTRAINT [FK_WaterMeasurementSelfReport_WaterAccount_WaterAccountID]                  FOREIGN KEY ([WaterAccountID])  	                REFERENCES dbo.[WaterAccount]([WaterAccountID]),
    CONSTRAINT [FK_WaterMeasurementSelfReport_WaterMeasurementType_WaterMeasurementTypeID]  FOREIGN KEY ([WaterMeasurementTypeID])              REFERENCES dbo.[WaterMeasurementType]([WaterMeasurementTypeID]),
    CONSTRAINT [FK_WaterMeasurementSelfReport_ReportingPeriod_ReportingPeriodID]            FOREIGN KEY ([ReportingPeriodID])                   REFERENCES dbo.[ReportingPeriod]([ReportingPeriodID]),
    CONSTRAINT [FK_WaterMeasurementSelfReport_SelfReportStatus_SelfReportStatusID]          FOREIGN KEY ([WaterMeasurementSelfReportStatusID])  REFERENCES dbo.[SelfReportStatus]([SelfReportStatusID]),
    CONSTRAINT [FK_WaterMeasurementSelfReport_User_CreateUserID]                            FOREIGN KEY ([CreateUserID])                        REFERENCES dbo.[User]([UserID]),
    CONSTRAINT [FK_WaterMeasurementSelfReport_User_UpdateUserID]                            FOREIGN KEY ([UpdateUserID])                        REFERENCES dbo.[User]([UserID]),
        
    CONSTRAINT [AK_WaterMeasurementSelfReport_GeographyID_WaterAccountID_WaterMeasurementTypeID_ReportingPeriodID] UNIQUE ([GeographyID], [WaterAccountID], [WaterMeasurementTypeID], [ReportingPeriodID])
)