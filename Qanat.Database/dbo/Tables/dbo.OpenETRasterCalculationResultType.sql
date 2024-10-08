CREATE TABLE [dbo].[OpenETRasterCalculationResultType](
	[OpenETRasterCalculationResultTypeID] [int] NOT NULL CONSTRAINT [PK_OpenETRasterCalculationResultType_OpenETRasterCalculationResultTypeID] PRIMARY KEY,
	[OpenETRasterCalculationResultTypeName] [varchar](100) NOT NULL CONSTRAINT [AK_OpenETRasterCalculationResultType_AK_OpenETRasterCalculationResultTypeName] UNIQUE,
	[OpenETRasterCalculationResultTypeDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_OpenETRasterCalculationResultType_OpenETRasterCalculationResultTypeDisplayName] UNIQUE
)
