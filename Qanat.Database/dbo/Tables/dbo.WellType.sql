CREATE TABLE [dbo].[WellType]
(
    [WellTypeID]                INT             NOT NULL IDENTITY(1, 1),
    [GeographyID]               INT             NOT NULL,

    [Name]                      VARCHAR(256)    NOT NULL,

    [SchemotoSchema]            VARCHAR(MAX)    NULL,

    [CreateDate]                DATETIME        NULL,
    [CreateUserID]              INT             NOT NULL,
    [UpdateDate]                DATETIME        NULL,
    [UpdateUserID]              INT             NULL,

    CONSTRAINT [PK_WellType_WellTypeID]             PRIMARY KEY ([WellTypeID]),
    CONSTRAINT [FK_WellType_Geography_GeographyID]  FOREIGN KEY ([GeographyID])     REFERENCES dbo.[Geography]([GeographyID]),
    CONSTRAINT [FK_WellType_User_CreateUserID]      FOREIGN KEY ([CreateUserID])    REFERENCES dbo.[User]([UserID]),
    CONSTRAINT [FK_WellType_User_UpdateUserID]      FOREIGN KEY ([UpdateUserID])    REFERENCES dbo.[User]([UserID]),
)
GO
