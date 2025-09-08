CREATE TABLE [dbo].[FileResource] (
    [FileResourceID]         INT              IDENTITY (1, 1) NOT NULL,
    [OriginalBaseFilename]   VARCHAR (255)    NOT NULL,
    [OriginalFileExtension]  VARCHAR (255)    NOT NULL,
    [FileResourceGUID]       UNIQUEIDENTIFIER NOT NULL,
    [FileResourceCanonicalName] VARCHAR(100)  NOT NULL,
    [CreateUserID]           INT              NOT NULL,
    [CreateDate]             DATETIME         NOT NULL,
    CONSTRAINT [PK_FileResource_FileResourceID] PRIMARY KEY CLUSTERED ([FileResourceID] ASC),
    CONSTRAINT [FK_FileResource_User_CreateUserID_UserID] FOREIGN KEY ([CreateUserID]) REFERENCES [dbo].[User] ([UserID]),
    CONSTRAINT [AK_FileResource_FileResourceGUID] UNIQUE NONCLUSTERED ([FileResourceGUID] ASC)
);

