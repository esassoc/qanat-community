CREATE TABLE [dbo].[WaterMeasurementTypeDependency]
(    
    [WaterMeasurementTypeDependencyID]		INT NOT NULL IDENTITY(1, 1),
    [GeographyID]				            INT NOT NULL,
    [WaterMeasurementTypeID]				INT NOT NULL,
    [DependsOnWaterMeasurementTypeID]		INT NOT NULL,

    CONSTRAINT [PK_WaterMeasurementTypeDependency_WaterMeasurementTypeDependencyID]		PRIMARY KEY CLUSTERED ([WaterMeasurementTypeDependencyID]),
    
	CONSTRAINT [FK_WaterMeasurementTypeDependency_Geography_GeographyID]                                            FOREIGN KEY ([GeographyID])	                    REFERENCES [dbo].[Geography] ([GeographyID]),
	CONSTRAINT [FK_WaterMeasurementTypeDependency_WaterMeasurementType_WaterMeasurementTypeID]                      FOREIGN KEY ([WaterMeasurementTypeID])	        REFERENCES [dbo].[WaterMeasurementType] ([WaterMeasurementTypeID]),
	CONSTRAINT [FK_WaterMeasurementTypeDependency_DependsOnWaterMeasurementType_DependsOnWaterMeasurementTypeID]    FOREIGN KEY ([DependsOnWaterMeasurementTypeID])	REFERENCES [dbo].[WaterMeasurementType] ([WaterMeasurementTypeID]),
)
