CREATE TABLE dbo.ModelUser
(
    [ModelUserID]   INT     NOT NULL IDENTITY(1,1),
    [ModelID]       INT     NOT NULL,
    [UserID]        INT     NOT NULL,

    CONSTRAINT [PK_ModelUser_ModelUserID]       PRIMARY KEY ([ModelUserID]),

    CONSTRAINT [FK_ModelUser_Model_ModelID]     FOREIGN KEY ([ModelID])     REFERENCES dbo.[Model]([ModelID]),
    CONSTRAINT [FK_ModelUser_User_UserID]       FOREIGN KEY ([UserID])      REFERENCES dbo.[User]([UserID]),

    CONSTRAINT [AK_ModelUser_ModelID_UserID]    UNIQUE ([ModelID], [UserID])
)