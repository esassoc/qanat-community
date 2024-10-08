MERGE INTO dbo.WaterMeasurementCalculationType AS Target
USING (VALUES
	(1, 'CalculateEffectivePrecip', 'Calculate Effective Precip'),
	(2, 'CalculateSurfaceWaterConsumption', 'Calculate SurfaceWater Consumption'),
	(3, 'ETMinusPrecipMinusTotalSurfaceWater', 'ET - Precip - TotalSurfaceWater'),
	(4, 'CalculatePrecipitationCreditOffset', 'Calculate Precipitation Credit Offset'),
	(5, 'CalculatePositiveConsumedGroundwater', 'Calculate Positive Consumed Groundwater'),
	(6, 'CalculateUnadjustedExtractedGroundwater', 'Calculate Unadjusted Extracted Groundwater'),
	(7, 'CalculateExtractedGroundwater', 'Calculate Extracted Groundwater'),
	(8, 'CalculateExtractedAgainstSupply', 'Calculate Extracted Against Supply'),
	(9, 'CalculateOpenETConsumptiveUse', 'Calculate Open ET Consumptive Use')
)
AS Source (WaterMeasurementCalculationTypeID, WaterMeasurementCalculationTypeName, WaterMeasurementCalculationTypeDisplayName)
ON Target.WaterMeasurementCalculationTypeID = Source.WaterMeasurementCalculationTypeID
WHEN MATCHED THEN
UPDATE SET
	WaterMeasurementCalculationTypeName = Source.WaterMeasurementCalculationTypeName,
	WaterMeasurementCalculationTypeDisplayName = Source.WaterMeasurementCalculationTypeDisplayName
WHEN NOT MATCHED BY TARGET THEN
	INSERT (WaterMeasurementCalculationTypeID, WaterMeasurementCalculationTypeName, WaterMeasurementCalculationTypeDisplayName)
	VALUES (WaterMeasurementCalculationTypeID, WaterMeasurementCalculationTypeName, WaterMeasurementCalculationTypeDisplayName)
WHEN NOT MATCHED BY SOURCE THEN
	DELETE;
