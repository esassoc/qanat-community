CREATE TABLE [dbo].[OpenETSyncHistory](
	[OpenETSyncHistoryID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_OpenETSyncHistory_OpenETSyncHistoryID] PRIMARY KEY,
	OpenETSyncID int not null constraint [FK_OpenETSyncHistory_OpenETSync_OpenETSyncID] foreign key references dbo.OpenETSync(OpenETSyncID),
	[OpenETSyncResultTypeID] [int] NOT NULL CONSTRAINT [FK_OpenETSyncHistory_OpenETSyncResultType_OpenETSyncResultTypeID] FOREIGN KEY REFERENCES [dbo].[OpenETSyncResultType] ([OpenETSyncResultTypeID]),
	[OpenETRasterCalculationResultTypeID] [int] NOT NULL CONSTRAINT [FK_OpenETSyncHistory_OpenETRasterCalculationResultType_OpenETRasterCalculationResultTypeID] FOREIGN KEY REFERENCES [dbo].[OpenETRasterCalculationResultType] ([OpenETRasterCalculationResultTypeID]) DEFAULT (1),
	[LastCalculationDate] [datetime] NULL,
	[LastSuccessfulCalculationDate] [datetime] NULL,
	[LastCalculationErrorMessage] VARCHAR(MAX) NULL,
	[CreateDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NOT NULL,
	[ErrorMessage] [varchar](max) NULL,

	[GoogleDriveRasterFileID]		VARCHAR (33)	NULL,
	[RasterFileResourceID]			INT				NULL CONSTRAINT [FK_OpenETSyncHistory_FileResource_RasterFileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource] ([FileResourceID]),
)