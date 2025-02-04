using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Qanat.Common.Util;

namespace Qanat.EFModels.Entities;

public static class WaterMeasurementCalculations
{
    public static async Task RunAllMeasurementTypesForGeography(QanatDbContext dbContext, int geographyID, DateTime effectiveDate)
    {
        var allWaterMeasurementTypes = await dbContext.WaterMeasurementTypes
            .Include(x => x.WaterMeasurementTypeDependencyWaterMeasurementTypes).ThenInclude(x => x.DependsOnWaterMeasurementType)
            .AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToListAsync();

        var sortedTypesBasedOnDependencies = TopologicalSort.Sort(
            allWaterMeasurementTypes,
            wmt => wmt.WaterMeasurementTypeDependencyWaterMeasurementTypes.Select(d => d.DependsOnWaterMeasurementType.WaterMeasurementTypeName.ToString()).AsEnumerable(),
            x => x.WaterMeasurementTypeName
        ).ToList();

        var sortedWaterMeasurementTypesWithCalculations = sortedTypesBasedOnDependencies.Where(x => x.WaterMeasurementCalculationTypeID.HasValue && x.IsActive);
        foreach (var waterMeasurementType in sortedWaterMeasurementTypesWithCalculations)
        {
            await RunCalculation(dbContext, geographyID, effectiveDate, waterMeasurementType);
        }
    }

    public static async Task RunMeasurementTypeForGeography(QanatDbContext dbContext, int geographyID, int waterMeasurementTypeID, DateTime effectiveDate)
    {
        var waterMeasurementType = await dbContext.WaterMeasurementTypes
            .Include(x => x.WaterMeasurementTypeDependencyWaterMeasurementTypes).ThenInclude(x => x.DependsOnWaterMeasurementType) //MK 8/15/2024 -- Need dependencies to run calculations.
            .Include(x => x.WaterMeasurementTypeDependencyDependsOnWaterMeasurementTypes).ThenInclude(x => x.WaterMeasurementType) //MK 8/15/2024 -- Need the inverse relationship to crawl the dependency tree, wish the generated names were a bit more friendly.
            .AsNoTracking()
            .SingleAsync(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == waterMeasurementTypeID && x.IsActive);

        //MK 8/15/2024 -- If the water measurement type has a calculation, run the calculation and then recalculate the dependants. Otherwise, skip it and just recalculate the dependants.
        if (waterMeasurementType.WaterMeasurementCalculationTypeID.HasValue)
        {
            await RunCalculation(dbContext, geographyID, effectiveDate, waterMeasurementType);
        }

        var dependantTypes = await WaterMeasurementTypes.GetDependencyChainForWaterMeasurementType(dbContext, geographyID, waterMeasurementType.WaterMeasurementTypeID);

        var dependantTypesSorted = TopologicalSort.Sort(
            dependantTypes,
            wmt => wmt.WaterMeasurementTypeDependencyWaterMeasurementTypes.Select(d => d.DependsOnWaterMeasurementType.WaterMeasurementTypeName.ToString()).AsEnumerable(),
            x => x.WaterMeasurementTypeName,
            false //MK 8/15/2024 -- Don't throw on other missing dependencies, we just want to recalculate the dependants of what has changed. We need them in WaterMeasurementTypeDependencyWaterMeasurementTypes though so they are included in the calculation.
        ).ToList();

        foreach (var dependantType in dependantTypesSorted)
        {
            await RunCalculation(dbContext, geographyID, effectiveDate, dependantType);
        }
    }

    private static async Task RunCalculation(QanatDbContext dbContext, int geographyID, DateTime effectiveDate, WaterMeasurementType waterMeasurementType)
    {
        if (waterMeasurementType.WaterMeasurementCalculationType == null)
        {
            throw new ArgumentException($"Calculation method not found for {waterMeasurementType.WaterMeasurementTypeName}");
        }

        switch (waterMeasurementType.WaterMeasurementCalculationType!.ToEnum)
        {
            case WaterMeasurementCalculationTypeEnum.CalculateEffectivePrecip:
                //CalculateEffectivePrecip assumes one dependency that is of Precip category.
                var precipDependency = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.Precip.WaterMeasurementCategoryTypeID);

                var precipMeasurementType = precipDependency?.DependsOnWaterMeasurementType;
                if (precipMeasurementType != null)
                {
                    await CalculateEffectivePrecip(dbContext, geographyID, waterMeasurementType, precipMeasurementType, effectiveDate);
                }
                else
                {
                    throw new ArgumentException("Missing dependency for CalculateEffectivePrecip.");
                }
                break;

            case WaterMeasurementCalculationTypeEnum.CalculateSurfaceWaterConsumption:
                //CalculateSurfaceWaterConsumption assumes one dependency that is of SurfaceWater category.
                var surfaceWaterDependency = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.SurfaceWater.WaterMeasurementCategoryTypeID);

                var surfaceWaterMeasurementType = surfaceWaterDependency?.DependsOnWaterMeasurementType;
                if (surfaceWaterMeasurementType != null)
                {
                    await CalculateSurfaceWaterConsumption(dbContext, geographyID, waterMeasurementType, surfaceWaterMeasurementType, effectiveDate);
                }
                else
                {
                    throw new ArgumentException("Missing dependency for CalculateSurfaceWaterConsumption.");
                }
                break;

            case WaterMeasurementCalculationTypeEnum.ETMinusPrecipMinusTotalSurfaceWater:
                //ETMinusPrecipMinusTotalSurfaceWater assumes one dependency that has a CalculateEffectivePrecip and 1 to N that are of category SurfaceWater or have a CalculationName of CalculateSurfaceWaterConsumption.
                var precipitationDependency = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.CalculateEffectivePrecip.WaterMeasurementCalculationTypeID);

                var precipitationMeasurementType = precipitationDependency?.DependsOnWaterMeasurementType;

                var surfaceWaterMeasurementTypes = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .Where(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.SurfaceWater.WaterMeasurementCategoryTypeID || x.DependsOnWaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.CalculateSurfaceWaterConsumption.WaterMeasurementCalculationTypeID)
                    .Select(x => x.DependsOnWaterMeasurementType)
                    .ToList();

                var surfaceWaterMeasurementTypeNames = surfaceWaterMeasurementTypes.Select(x => x.WaterMeasurementTypeName).ToList();
                if (precipitationMeasurementType != null && surfaceWaterMeasurementTypeNames.Any())
                {
                    await ETMinusPrecipMinusTotalSurfaceWater(dbContext, geographyID, effectiveDate, waterMeasurementType, precipitationMeasurementType.WaterMeasurementTypeName, surfaceWaterMeasurementTypeNames);
                }
                else
                {
                    throw new ArgumentException("Missing dependency for ETMinusPrecipMinusTotalSurfaceWater.");
                }
                break;

            case WaterMeasurementCalculationTypeEnum.CalculatePrecipitationCreditOffset:
                //CalculatePositiveConsumedGroundwaterWithPrecipitationCreditOffset assumes one dependency that has a ETMinusPrecipMinusTotalSurfaceWater calculation.
                var etMinusPrecipMinusTotalSurfaceWaterDependencyForCredit = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.ETMinusPrecipMinusTotalSurfaceWater.WaterMeasurementCalculationTypeID);

                var etMinusPrecipMinusTotalSurfaceWaterMeasurementTypeForCredit = etMinusPrecipMinusTotalSurfaceWaterDependencyForCredit?.DependsOnWaterMeasurementType;
                
                if (etMinusPrecipMinusTotalSurfaceWaterMeasurementTypeForCredit != null)
                {
                    await CalculatePrecipitationCreditOffset(dbContext, geographyID, waterMeasurementType, etMinusPrecipMinusTotalSurfaceWaterMeasurementTypeForCredit, effectiveDate);
                }
                else
                {
                    throw new ArgumentException("Missing dependency for CalculatePrecipitationCreditOffset.");
                }
                break;

            case WaterMeasurementCalculationTypeEnum.CalculatePositiveConsumedGroundwater:
                //CalculatePositiveConsumedGroundwaterWithPrecipitationCreditOffset assumes one dependency that has a ETMinusPrecipMinusTotalSurfaceWater calculation.
                var etMinusPrecipMinusTotalSurfaceWaterDependency = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.ETMinusPrecipMinusTotalSurfaceWater.WaterMeasurementCalculationTypeID);

                var etMinusPrecipMinusTotalSurfaceWaterMeasurementType = etMinusPrecipMinusTotalSurfaceWaterDependency?.DependsOnWaterMeasurementType;

                if (etMinusPrecipMinusTotalSurfaceWaterMeasurementType != null)
                {
                    await CalculatePositiveConsumedGroundwater(dbContext, geographyID, waterMeasurementType, etMinusPrecipMinusTotalSurfaceWaterMeasurementType, effectiveDate);
                }
                else
                {
                    throw new ArgumentException("Missing dependency for CalculatePositiveConsumedGroundwater.");
                }
                break;

            case WaterMeasurementCalculationTypeEnum.CalculateUnadjustedExtractedGroundwater:
                //CalculateExtractedGroundwater assumes one dependency that has a calculation of ETMinusPrecipMinusTotalSurfaceWater and one that is of category manual adjustment.
                var consumedGroundwaterWithCreditDependencyUnadjusted = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.ETMinusPrecipMinusTotalSurfaceWater.WaterMeasurementCalculationTypeID);

                var consumedGroundwaterWithCreditMeasurementTypeUnadjusted = consumedGroundwaterWithCreditDependencyUnadjusted?.DependsOnWaterMeasurementType;

                if (consumedGroundwaterWithCreditMeasurementTypeUnadjusted != null)
                {
                    await CalculateUnadjustedExtractedGroundwater(dbContext, geographyID, waterMeasurementType, consumedGroundwaterWithCreditMeasurementTypeUnadjusted, effectiveDate);
                }
                else
                {
                    throw new ArgumentException("Missing dependency for CalculateUnadjustedExtractedGroundwater.");
                }
                break;
            case WaterMeasurementCalculationTypeEnum.CalculateExtractedGroundwater:
                //CalculateExtractedGroundwater assumes one dependency that has a calculation of ETMinusPrecipMinusTotalSurfaceWater and one that is of category manual adjustment.
                var consumedGroundwaterWithCreditDependency = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.ETMinusPrecipMinusTotalSurfaceWater.WaterMeasurementCalculationTypeID);

                var consumedGroundwaterWithCreditMeasurementType = consumedGroundwaterWithCreditDependency?.DependsOnWaterMeasurementType;

                var extractedGroundwaterAdjustmentDependency = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryType.ToEnum == WaterMeasurementCategoryTypeEnum.ManualAdjustment);

                var extractedGroundwaterAdjustmentType = extractedGroundwaterAdjustmentDependency?.DependsOnWaterMeasurementType;

                if (consumedGroundwaterWithCreditMeasurementType != null && extractedGroundwaterAdjustmentType != null)
                {
                    await CalculateExtractedGroundwater(dbContext, geographyID, waterMeasurementType, consumedGroundwaterWithCreditMeasurementType, extractedGroundwaterAdjustmentType, effectiveDate);
                }
                else
                {
                    throw new ArgumentException("Missing dependency for CalculateExtractedGroundwater.");
                }
                break;

            case WaterMeasurementCalculationTypeEnum.CalculateExtractedAgainstSupply:
                //CalculateExtractedAgainstSupply assumes one dependency that is ExtractedGroundwater.

                var extractedGroundwaterDependency = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.CalculateExtractedGroundwater.WaterMeasurementCalculationTypeID);

                var extractedGroundwaterMeasurementType = extractedGroundwaterDependency?.DependsOnWaterMeasurementType;
                if (extractedGroundwaterMeasurementType != null)
                {
                    await CalculateExtractedAgainstSupply(dbContext, geographyID, waterMeasurementType, extractedGroundwaterMeasurementType, effectiveDate);
                }
                else
                {
                    throw new ArgumentException("Missing dependency for CalculateExtractedAgainstSupply.");
                }
                break;
            case WaterMeasurementCalculationTypeEnum.CalculateOpenETConsumptiveUse:
                //CalculateOpenETConsumptiveUse assumes one dependency that is of category ET and one that is of category Precip.
                var openETEvapoDependency = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryType.ToEnum == WaterMeasurementCategoryTypeEnum.ET);

                var openETEvapoMeasurementType = openETEvapoDependency?.DependsOnWaterMeasurementType;

                var openETPrecipDependency = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryType.ToEnum == WaterMeasurementCategoryTypeEnum.Precip);

                var openETPrecipMeasurementType = openETPrecipDependency?.DependsOnWaterMeasurementType;


                var consumedSurfaceWaterDependency = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCalculationType?.ToEnum == WaterMeasurementCalculationTypeEnum.CalculateSurfaceWaterConsumption);

                var consumedSurfaceWaterMeasurementType = consumedSurfaceWaterDependency?.DependsOnWaterMeasurementType;

                if (openETEvapoMeasurementType != null && openETPrecipMeasurementType != null)
                {
                    await CalculateOpenETConsumptiveUse(dbContext, geographyID, waterMeasurementType, openETEvapoMeasurementType, openETPrecipMeasurementType, consumedSurfaceWaterMeasurementType, effectiveDate);
                }
                else
                {
                    throw new ArgumentException("Missing dependency for CalculateOpenETConsumptiveUse.");
                }
                break;
            default:
                throw new ArgumentException($"Calculation method not found for {waterMeasurementType.WaterMeasurementTypeName}");
        }
    }

    /// <summary>
    /// Calculates and stores the Effective Precipitation for a given geography based on recorded precipitation values and the associated precipitation efficiency factors by allocation zone.
    /// </summary>
    /// <param name="dbContext">The database context to interact with the Qanat database.</param>
    /// <param name="geographyID">The unique ID for the geography in which the calculation is performed.</param>
    /// <param name="effectivePrecipMeasurementType">The WaterMeasurementType representing the calculated Effective Precipitation.</param>
    /// <param name="precipWaterMeasurementType">The WaterMeasurementType representing the recorded precipitation values, currently from OpenET or Land IQ.</param>
    /// <param name="reportedDate">The date of the recorded precipitation measurements.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains no object, as the method saves the calculated Effective Precipitation measurements directly to the database.
    /// </returns>
    /// <exception cref="Exception">
    /// Thrown when the geography lacks the required allocation zone group configuration or when any allocation zone is missing a precipitation multiplier.
    /// </exception>
    private static async Task CalculateEffectivePrecip(QanatDbContext dbContext, int geographyID, WaterMeasurementType effectivePrecipMeasurementType, WaterMeasurementType precipWaterMeasurementType, DateTime reportedDate)
    {
        var geographyAllocationPlanConfiguration = await dbContext.GeographyAllocationPlanConfigurations
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID);

        if (geographyAllocationPlanConfiguration == null)
        {
            // todo: use AlertMessageDto to throw error instead of exception
            throw new Exception("Could not calculate Effective Precip because this geography does not have the required Allocation Zone Group configured.");
        }

        var zoneIDByParcelIDs = await dbContext.ParcelZones.AsNoTracking()
            .Include(x => x.Zone)
            .Where(x => x.Zone.ZoneGroupID == geographyAllocationPlanConfiguration.ZoneGroupID)
            .ToDictionaryAsync(x => x.ParcelID, x => x.ZoneID);

        var zoneIDsByUsageEntityNames = dbContext.UsageEntities.AsNoTracking()
            .Include(x => x.Parcel)
            .Where(x => x.GeographyID == geographyID)
            .AsEnumerable()
            .Where(x => zoneIDByParcelIDs.ContainsKey(x.ParcelID))
            .ToLookup(x => x.UsageEntityName, x => zoneIDByParcelIDs[x.ParcelID]);

        var precipWaterMeasurements = await dbContext.WaterMeasurements.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == precipWaterMeasurementType.WaterMeasurementTypeID && x.ReportedDate.Date == reportedDate.Date)
            .ToListAsync();

        var precipMultiplierByZoneID = await dbContext.Zones.AsNoTracking()
            .Where(x => x.ZoneGroupID == geographyAllocationPlanConfiguration.ZoneGroupID)
            .ToDictionaryAsync(x => x.ZoneID, x => x.PrecipMultiplier);

        var effectivePrecipWaterMeasurements = new List<WaterMeasurement>();
        foreach (var precipWaterMeasurement in precipWaterMeasurements)
        {
            if (!zoneIDsByUsageEntityNames[precipWaterMeasurement.UsageEntityName].Any())
            {
                continue;
            }

            var zoneID = zoneIDsByUsageEntityNames[precipWaterMeasurement.UsageEntityName].First();
            if (!precipMultiplierByZoneID.ContainsKey(zoneID) || !precipMultiplierByZoneID[zoneID].HasValue)
            {
                throw new Exception("Could not calculate Effective Precip because at least one Allocation Zone does not have a precipitation multiplier configured.");
            }

            var effectivePrecipMultiplier = precipMultiplierByZoneID[zoneID].Value;
            effectivePrecipWaterMeasurements.Add(new WaterMeasurement()
            {
                WaterMeasurementTypeID = effectivePrecipMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UnitTypeID = precipWaterMeasurement.UnitTypeID,
                UsageEntityName = precipWaterMeasurement.UsageEntityName,
                ReportedDate = reportedDate,
                ReportedValue = precipWaterMeasurement.ReportedValue * effectivePrecipMultiplier,
                ReportedValueInAcreFeet = precipWaterMeasurement.ReportedValueInAcreFeet * effectivePrecipMultiplier,
                UsageEntityArea = precipWaterMeasurement.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = false,
                Comment = $"{precipWaterMeasurementType.WaterMeasurementTypeName} Value: {precipWaterMeasurement.ReportedValueInAcreFeet} ac-ft, Effective Precipitation Factor: {effectivePrecipMultiplier}"
            });
        }

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, effectivePrecipWaterMeasurements, effectivePrecipMeasurementType.WaterMeasurementTypeID);
    }

    /// <summary>
    /// Calculates and stores the consumed surface water for a given geography by applying the Surface Water Efficiency 
    /// Factor to the recorded surface water measurements.
    /// </summary>
    /// <param name="dbContext">The database context to interact with the Qanat database.</param>
    /// <param name="geographyID">The unique identifier for the geography in which the calculation is performed.</param>
    /// <param name="consumedSurfaceWaterMeasurementType">The WaterMeasurementType representing the calculated consumed surface water.</param>
    /// <param name="surfaceWaterMeasurementType">The WaterMeasurementType representing the surface water deliveries.</param>
    /// <param name="reportedDate">The date of the recorded surface water measurements.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains no object, as the method saves the calculated consumed surface water measurements directly to the database.
    /// </returns>
    /// <exception cref="Exception">
    /// Thrown when the Surface Water Efficiency Factor is not set in the CalculationJSON of the consumedSurfaceWaterMeasurementType.
    /// </exception>
    private static async Task CalculateSurfaceWaterConsumption(QanatDbContext dbContext, int geographyID, WaterMeasurementType consumedSurfaceWaterMeasurementType, WaterMeasurementType surfaceWaterMeasurementType, DateTime reportedDate)
    {
        //Surface water consumption requires a Surface Water Efficiency Factor to be set in the calculation JSON.
        var parsed = consumedSurfaceWaterMeasurementType.CalculationJSON.TryParseJObject(out var calculationJSON);
        if (!parsed || !calculationJSON.ContainsKey("SurfaceWaterEfficiencyFactor") || calculationJSON["SurfaceWaterEfficiencyFactor"] == null)
        {
            throw new Exception("Could not calculate Consumed Surface Water because the Surface Water Efficiency Factor is missing from the CalculationJSON.");
        }

        var surfaceWaterEfficiencyFactor = calculationJSON["SurfaceWaterEfficiencyFactor"]!.Value<decimal>();
        var surfaceWaterMeasurements = await dbContext.WaterMeasurements.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == surfaceWaterMeasurementType.WaterMeasurementTypeID && x.ReportedDate.Date == reportedDate.Date)
            .ToListAsync();

        var consumedSurfaceWaterMeasurements = new List<WaterMeasurement>();
        foreach (var surfaceWaterDelivery in surfaceWaterMeasurements)
        {
            var newSurfaceWaterConsumption = new WaterMeasurement()
            {
                WaterMeasurementTypeID = consumedSurfaceWaterMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UnitTypeID = surfaceWaterDelivery.UnitTypeID,
                UsageEntityName = surfaceWaterDelivery.UsageEntityName,
                ReportedDate = reportedDate,
                ReportedValue = surfaceWaterDelivery.ReportedValue * surfaceWaterEfficiencyFactor,
                ReportedValueInAcreFeet = surfaceWaterDelivery.ReportedValueInAcreFeet * surfaceWaterEfficiencyFactor,
                UsageEntityArea = surfaceWaterDelivery.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = false,
                Comment = $"{consumedSurfaceWaterMeasurementType.WaterMeasurementTypeName} Value: {surfaceWaterDelivery.ReportedValueInAcreFeet} ac-ft, Surface Water Efficiency Factor: {surfaceWaterEfficiencyFactor}"
            };

            consumedSurfaceWaterMeasurements.Add(newSurfaceWaterConsumption);
        }

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, consumedSurfaceWaterMeasurements, consumedSurfaceWaterMeasurementType.WaterMeasurementTypeID);
    }

    /// <summary>
    /// Calculates and stores the consumed groundwater for a given geography by subtracting precipitation summed surface water from evapotranspiration measurements.
    /// </summary>
    /// <param name="dbContext">The database context to interact with the Qanat database.</param>
    /// <param name="geographyID">The unique identifier for the geography in which the calculation is performed.</param>
    /// <param name="reportedDate">The date of the recorded water measurements.</param>
    /// <param name="consumedGroundWaterMeasurementType">The WaterMeasurementType representing the calculated consumed groundwater.</param>
    /// <param name="effectivePrecipWaterMeasurementTypeName">The name of the WaterMeasurementType representing effective precipitation values.</param>
    /// <param name="surfaceWaterTypeNames">A list of WaterMeasurementType names representing surface water values to be summed.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains no object, as the method saves the calculated consumed groundwater measurements directly to the database.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the unit type is missing or unrecognized during groundwater consumption calculation.
    /// </exception>
    private static async Task ETMinusPrecipMinusTotalSurfaceWater(QanatDbContext dbContext, int geographyID, DateTime reportedDate, WaterMeasurementType consumedGroundWaterMeasurementType, string effectivePrecipWaterMeasurementTypeName, List<string> surfaceWaterTypeNames)
    {
        var allWaterMeasurementsForGeographyAndDate = await dbContext.WaterMeasurements.AsNoTracking()
            .Include(x => x.WaterMeasurementType)
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == reportedDate.Date)
            .ToListAsync();

        var evapotranspirationWaterMeasurements = allWaterMeasurementsForGeographyAndDate
            .Where(x => x.WaterMeasurementType.WaterMeasurementCategoryType?.ToEnum == WaterMeasurementCategoryTypeEnum.ET)
            .ToList();

        var precipitationWaterMeasurements = allWaterMeasurementsForGeographyAndDate
            .Where(x => x.WaterMeasurementType.WaterMeasurementTypeName == effectivePrecipWaterMeasurementTypeName)
            .DistinctBy(x => new { x.UsageEntityName, x.UsageEntityArea, x.ReportedValueInAcreFeet })
            .ToDictionary(x => new { x.UsageEntityName, x.UsageEntityArea });

        var surfaceWaterMeasurements = allWaterMeasurementsForGeographyAndDate
            .Where(x => surfaceWaterTypeNames.Contains(x.WaterMeasurementType.WaterMeasurementTypeName))
            .ToLookup(x => x.UsageEntityName);

        var newWaterMeasurements = new List<WaterMeasurement>();
        foreach (var evapoWaterMeasurement in evapotranspirationWaterMeasurements)
        {
            if (!precipitationWaterMeasurements.ContainsKey(new { evapoWaterMeasurement.UsageEntityName, evapoWaterMeasurement.UsageEntityArea }))
            {
                continue;
            }

            var precipWaterMeasurement = precipitationWaterMeasurements[new { evapoWaterMeasurement.UsageEntityName, evapoWaterMeasurement.UsageEntityArea }];
            var surfaceWaterMeasurementSumInAcreFeet = surfaceWaterMeasurements[evapoWaterMeasurement.UsageEntityName].Any()
                ? surfaceWaterMeasurements[evapoWaterMeasurement.UsageEntityName].Sum(x => x.ReportedValueInAcreFeet)
                : 0;

            var reportedValueInAcreFeet = evapoWaterMeasurement.ReportedValueInAcreFeet - precipWaterMeasurement.ReportedValueInAcreFeet - surfaceWaterMeasurementSumInAcreFeet;

            newWaterMeasurements.Add(new WaterMeasurement()
            {
                WaterMeasurementTypeID = consumedGroundWaterMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UnitTypeID = evapoWaterMeasurement.UnitTypeID,
                UsageEntityName = evapoWaterMeasurement.UsageEntityName,
                ReportedDate = reportedDate,
                ReportedValueInAcreFeet = reportedValueInAcreFeet,
                ReportedValue = evapoWaterMeasurement.UnitTypeID switch
                {
                    (int)UnitTypeEnum.Inches => UnitConversionHelper.ConvertAcreFeetToInches(reportedValueInAcreFeet.Value, evapoWaterMeasurement.UsageEntityArea.Value),
                    (int)UnitTypeEnum.Millimeters => UnitConversionHelper.ConvertAcreFeetToMillimeters(reportedValueInAcreFeet.Value, evapoWaterMeasurement.UsageEntityArea.Value),
                    _ => throw new ArgumentOutOfRangeException("Unit Type", "Groundwater Consumption cannot be calculated because unit type is missing.")
                },
                UsageEntityArea = evapoWaterMeasurement.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = false,
                Comment = $"{evapoWaterMeasurement.WaterMeasurementType.WaterMeasurementTypeName} Value: {evapoWaterMeasurement.ReportedValueInAcreFeet} ac-ft, {effectivePrecipWaterMeasurementTypeName} Value: {precipWaterMeasurement.ReportedValueInAcreFeet} ac-ft, Total Surface Water Value: {surfaceWaterMeasurementSumInAcreFeet} ac-ft"
            });
        }

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, newWaterMeasurements, consumedGroundWaterMeasurementType.WaterMeasurementTypeID);
    }

    /// <summary>
    /// Calculates and stores positive consumed groundwater values for a given geography.
    /// </summary>
    /// <param name="dbContext">The database context to interact with the Qanat database.</param>
    /// <param name="geographyID">The unique identifier for the geography in which the calculation is performed.</param>
    /// <param name="positiveConsumedGroundwaterMeasurementType">The WaterMeasurementType representing the positive consumed groundwater values.</param>
    /// <param name="consumedGroundwaterMeasurementType">The WaterMeasurementType representing the overall consumed groundwater values.</param>
    /// <param name="precipitationCreditMeasurementType">The WaterMeasurementType representing the precipitation credit used to offset negative values.</param>
    /// <param name="reportedDate">The date of the recorded groundwater measurements.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains no object, as the method saves the calculated positive consumed groundwater and precipitation credit measurements directly to the database.
    /// </returns>
    private static async Task CalculatePositiveConsumedGroundwater(QanatDbContext dbContext, int geographyID, WaterMeasurementType positiveConsumedGroundwaterMeasurementType, WaterMeasurementType consumedGroundwaterMeasurementType, DateTime reportedDate)
    {
        var consumedGroundwaterMeasurements = await dbContext.WaterMeasurements.AsNoTracking()
            .Where(x => x.WaterMeasurementTypeID == consumedGroundwaterMeasurementType.WaterMeasurementTypeID && x.ReportedDate.Date == reportedDate)
            .ToListAsync();

        var newPositiveConsumedGroundwaterMeasurements = new List<WaterMeasurement>();
        foreach (var waterMeasurement in consumedGroundwaterMeasurements)
        {
            // positive consumed groundwater
            newPositiveConsumedGroundwaterMeasurements.Add(new WaterMeasurement()
            {
                WaterMeasurementTypeID = positiveConsumedGroundwaterMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UnitTypeID = waterMeasurement.UnitTypeID,
                UsageEntityName = waterMeasurement.UsageEntityName,
                ReportedDate = reportedDate,
                ReportedValueInAcreFeet = waterMeasurement.ReportedValueInAcreFeet > 0 ? waterMeasurement.ReportedValueInAcreFeet : 0,
                ReportedValue = waterMeasurement.ReportedValue > 0 ? waterMeasurement.ReportedValue : 0,
                UsageEntityArea = waterMeasurement.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = false,
                Comment = $"{consumedGroundwaterMeasurementType} Value: {waterMeasurement.ReportedValueInAcreFeet} ac-ft"
            });
        }

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, newPositiveConsumedGroundwaterMeasurements, positiveConsumedGroundwaterMeasurementType.WaterMeasurementTypeID);
    }

    /// <summary>
    /// Calculates and stores the precipitation credit records.
    /// </summary>
    /// <param name="dbContext">The database context to interact with the Qanat database.</param>
    /// <param name="geographyID">The unique identifier for the geography in which the calculation is performed.</param>
    /// <param name="positiveConsumedGroundwaterMeasurementType">The WaterMeasurementType representing the positive consumed groundwater values.</param>
    /// <param name="consumedGroundwaterMeasurementType">The WaterMeasurementType representing the overall consumed groundwater values.</param>
    /// <param name="precipitationCreditMeasurementType">The WaterMeasurementType representing the precipitation credit used to offset negative values.</param>
    /// <param name="reportedDate">The date of the recorded groundwater measurements.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains no object, as the method saves the calculated positive consumed groundwater and precipitation credit measurements directly to the database.
    /// </returns>
    private static async Task CalculatePrecipitationCreditOffset(QanatDbContext dbContext, int geographyID, WaterMeasurementType precipitationCreditMeasurementType, WaterMeasurementType consumedGroundwaterMeasurementType, DateTime reportedDate)
    {
        var consumedGroundwaterMeasurements = await dbContext.WaterMeasurements.AsNoTracking()
            .Where(x => x.WaterMeasurementTypeID == consumedGroundwaterMeasurementType.WaterMeasurementTypeID && x.ReportedDate.Date == reportedDate)
            .ToListAsync();

        var newPrecipitationCreditMeasurements = new List<WaterMeasurement>();
        foreach (var waterMeasurement in consumedGroundwaterMeasurements)
        {
            // precipitation credit offset
            newPrecipitationCreditMeasurements.Add(new WaterMeasurement()
            {
                WaterMeasurementTypeID = precipitationCreditMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UnitTypeID = waterMeasurement.UnitTypeID,
                UsageEntityName = waterMeasurement.UsageEntityName,
                ReportedDate = reportedDate,
                ReportedValueInAcreFeet = waterMeasurement.ReportedValueInAcreFeet < 0 ? waterMeasurement.ReportedValueInAcreFeet : 0,
                ReportedValue = waterMeasurement.ReportedValue < 0 ? waterMeasurement.ReportedValue : 0,
                UsageEntityArea = waterMeasurement.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = false,
                Comment = $"{consumedGroundwaterMeasurementType} Value: {waterMeasurement.ReportedValueInAcreFeet} ac-ft"
            });
        }

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, newPrecipitationCreditMeasurements, precipitationCreditMeasurementType.WaterMeasurementTypeID);
    }

    /// <summary>
    /// Calculates and stores extracted groundwater values for a given geography based on consumed groundwater multiplied by a groundwater efficiency factor WITHOUT the adjustment.
    /// </summary>
    /// <param name="dbContext">The database context to interact with the Qanat database.</param>
    /// <param name="geographyID">The unique identifier for the geography in which the calculation is performed.</param>
    /// <param name="unadjustedGroundwaterMeasurementType">The WaterMeasurementType representing the calculated extracted groundwater.</param>
    /// <param name="consumedGroundwaterMeasurementType">The WaterMeasurementType representing the consumed groundwater values.</param>
    /// <param name="extractedGroundwaterAdjustmentMeasurementType">The WaterMeasurementType representing any adjustments to the extracted groundwater values.</param>
    /// <param name="reportedDate">The date of the recorded groundwater measurements.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains no object, 
    /// as the method saves the calculated extracted groundwater measurements directly to the database.
    /// </returns>
    /// <exception cref="Exception">
    /// Thrown when the Groundwater Efficiency Factor is missing from the CalculationJSON of the extractedGroundwaterMeasurementType.
    /// </exception>
    private static async Task CalculateUnadjustedExtractedGroundwater(QanatDbContext dbContext, int geographyID, WaterMeasurementType unadjustedGroundwaterMeasurementType, WaterMeasurementType consumedGroundwaterMeasurementType, DateTime reportedDate)
    {
        //Extracted groundwater consumption requires a Groundwater Water Efficiency Factor to be set in the calculation JSON.
        var parsed = unadjustedGroundwaterMeasurementType.CalculationJSON.TryParseJObject(out var calculationJSON);
        if (!parsed || !calculationJSON.ContainsKey("GroundwaterEfficiencyFactor") || calculationJSON["GroundwaterEfficiencyFactor"] == null)
        {
            throw new Exception("Could not calculate Extracted Groundwater because the Groundwater Efficiency Factor is missing from the CalculationJSON.");
        }

        var groundwaterEfficiencyFactor = calculationJSON["GroundwaterEfficiencyFactor"]!.Value<decimal>();
        var consumedGroundWaterMeasurements = await dbContext.WaterMeasurements.Include(x => x.WaterMeasurementType)
            .AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementType.WaterMeasurementTypeID == consumedGroundwaterMeasurementType.WaterMeasurementTypeID && x.ReportedDate.Date == reportedDate.Date)
            .ToListAsync();

        var newWaterMeasurements = new List<WaterMeasurement>();
        foreach (var consumedGroundWaterMeasurement in consumedGroundWaterMeasurements)
        {
            var newUnadjustedGroundwaterMeasurement = new WaterMeasurement()
            {
                WaterMeasurementTypeID = unadjustedGroundwaterMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UnitTypeID = consumedGroundWaterMeasurement.UnitTypeID,
                UsageEntityName = consumedGroundWaterMeasurement.UsageEntityName,
                ReportedDate = reportedDate,
                ReportedValue = consumedGroundWaterMeasurement.ReportedValue / groundwaterEfficiencyFactor,
                ReportedValueInAcreFeet = consumedGroundWaterMeasurement.ReportedValueInAcreFeet / groundwaterEfficiencyFactor,
                UsageEntityArea = consumedGroundWaterMeasurement.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = false,
                Comment = $"{consumedGroundwaterMeasurementType.WaterMeasurementTypeName} Value: {consumedGroundWaterMeasurement.ReportedValue} {consumedGroundWaterMeasurement.UnitType?.UnitTypeAbbreviation}, Groundwater Efficiency Factor: {groundwaterEfficiencyFactor}"
            };

            newWaterMeasurements.Add(newUnadjustedGroundwaterMeasurement);
        }

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, newWaterMeasurements, unadjustedGroundwaterMeasurementType.WaterMeasurementTypeID);
    }

    /// <summary>
    /// Calculates and stores extracted groundwater values for a given geography based on consumed groundwater multiplied by a groundwater efficiency factor and an optional adjustment value.
    /// </summary>
    /// <param name="dbContext">The database context to interact with the Qanat database.</param>
    /// <param name="geographyID">The unique identifier for the geography in which the calculation is performed.</param>
    /// <param name="extractedGroundwaterMeasurementType">The WaterMeasurementType representing the calculated extracted groundwater.</param>
    /// <param name="consumedGroundwaterMeasurementType">The WaterMeasurementType representing the consumed groundwater values.</param>
    /// <param name="extractedGroundwaterAdjustmentMeasurementType">The WaterMeasurementType representing any adjustments to the extracted groundwater values.</param>
    /// <param name="reportedDate">The date of the recorded groundwater measurements.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains no object, 
    /// as the method saves the calculated extracted groundwater measurements directly to the database.
    /// </returns>
    /// <exception cref="Exception">
    /// Thrown when the Groundwater Efficiency Factor is missing from the CalculationJSON of the extractedGroundwaterMeasurementType.
    /// </exception>
    private static async Task CalculateExtractedGroundwater(QanatDbContext dbContext, int geographyID, WaterMeasurementType extractedGroundwaterMeasurementType, WaterMeasurementType consumedGroundwaterMeasurementType, WaterMeasurementType extractedGroundwaterAdjustmentMeasurementType, DateTime reportedDate)
    {
        //Extracted groundwater consumption requires a Groundwater Water Efficiency Factor to be set in the calculation JSON.
        var parsed = extractedGroundwaterMeasurementType.CalculationJSON.TryParseJObject(out var calculationJSON);
        if (!parsed || !calculationJSON.ContainsKey("GroundwaterEfficiencyFactor") || calculationJSON["GroundwaterEfficiencyFactor"] == null)
        {
            throw new Exception("Could not calculate Extracted Groundwater because the Groundwater Efficiency Factor is missing from the CalculationJSON.");
        }

        var groundwaterEfficiencyFactor = calculationJSON["GroundwaterEfficiencyFactor"]!.Value<decimal>();
        var consumedGroundWaterMeasurements = await dbContext.WaterMeasurements.Include(x => x.WaterMeasurementType)
            .AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementType.WaterMeasurementTypeID == consumedGroundwaterMeasurementType.WaterMeasurementTypeID && x.ReportedDate.Date == reportedDate.Date)
            .ToListAsync();

        var extractedGroundwaterAdjustments = await dbContext.WaterMeasurements.Include(x => x.WaterMeasurementType)
            .AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementType.WaterMeasurementTypeID == extractedGroundwaterAdjustmentMeasurementType.WaterMeasurementTypeID && x.ReportedDate.Date == reportedDate.Date)
            .ToListAsync();

        var newWaterMeasurements = new List<WaterMeasurement>();
        foreach (var consumedGroundWaterMeasurement in consumedGroundWaterMeasurements)
        {
            var extractedGroundwaterAdjustment = extractedGroundwaterAdjustments.FirstOrDefault(x => x.GeographyID == consumedGroundWaterMeasurement.GeographyID && x.ReportedDate.Date == reportedDate.Date && x.UsageEntityName == consumedGroundWaterMeasurement.UsageEntityName); //MK 8/14/2024 -- It should also probably check the UsageEntityArea and be a SingleOrDefault, but for my test case the area differed between the two measurements by a small fraction.

            var adjustmentValue = extractedGroundwaterAdjustment?.ReportedValue ?? 0;
            var adjustmentValueInAcreFeet = extractedGroundwaterAdjustment?.ReportedValueInAcreFeet ?? 0;

            var newExtractedGroundwaterMeasurement = new WaterMeasurement()
            {
                WaterMeasurementTypeID = extractedGroundwaterMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UnitTypeID = consumedGroundWaterMeasurement.UnitTypeID,
                UsageEntityName = consumedGroundWaterMeasurement.UsageEntityName,
                ReportedDate = reportedDate,
                ReportedValue = (consumedGroundWaterMeasurement.ReportedValue / groundwaterEfficiencyFactor) + adjustmentValue,
                ReportedValueInAcreFeet = (consumedGroundWaterMeasurement.ReportedValueInAcreFeet / groundwaterEfficiencyFactor) + adjustmentValueInAcreFeet,
                UsageEntityArea = consumedGroundWaterMeasurement.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = false,
                Comment = $"{consumedGroundwaterMeasurementType.WaterMeasurementTypeName} Value: {consumedGroundWaterMeasurement.ReportedValue} {consumedGroundWaterMeasurement.UnitType?.UnitTypeAbbreviation}, Groundwater Efficiency Factor: {groundwaterEfficiencyFactor}, Adjustment={adjustmentValue}"
            };

            newWaterMeasurements.Add(newExtractedGroundwaterMeasurement);
        }

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, newWaterMeasurements, extractedGroundwaterMeasurementType.WaterMeasurementTypeID);
    }

    private static async Task CalculateExtractedAgainstSupply(QanatDbContext dbContext, int geographyID, WaterMeasurementType extractedAgainstSupplyMeasurementType, WaterMeasurementType extractedGroundwaterMeasurementType, DateTime reportedDate)
    {
        var extractedAgainstSupplyMeasurementTypeByDate = new Dictionary<DateTime, List<WaterMeasurement>>();

        var reportedYear = reportedDate.Year;
        var reportingPeriodDto = await ReportingPeriods.GetByGeographyIDAndYearAsync(dbContext, geographyID, reportedYear);
        var reportingPeriodStart = reportingPeriodDto.StartDate;
        var reportingPeriodEnd = reportingPeriodDto.EndDate;

        var extractedGroundwaterMeasurements = await dbContext.WaterMeasurements.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == extractedGroundwaterMeasurementType.WaterMeasurementTypeID && x.ReportedDate >= reportingPeriodStart && x.ReportedDate <= reportingPeriodEnd)
            .ToListAsync();

        var waterAccounts = await dbContext.fWaterAccountParcelByGeographyAndYear(geographyID, reportedYear).AsNoTracking()
            .ToListAsync();

        foreach (var waterAccount in waterAccounts.GroupBy(x => x.WaterAccountID))
        {
            var activeParcelsNumbersInEffect = waterAccount.Select(x => x.ParcelNumber).ToList();
            var waterAccountGroundwaterMeasurements = extractedGroundwaterMeasurements.Where(x => activeParcelsNumbersInEffect.Contains(x.UsageEntityName)).ToList();
            var cumulativeValueInAcreFeet = waterAccountGroundwaterMeasurements.Sum(x => x.ReportedValueInAcreFeet.GetValueOrDefault(0));

            //MK 12/20/2024 -- If a negative cumulative value, set the EAS to 0 for all months in the period. Updated business logic as of QAN-924.
            if (cumulativeValueInAcreFeet <= 0)
            {
                foreach (var extractedGroundwaterMeasurement in waterAccountGroundwaterMeasurements)
                {
                    var extractedAgainstSupplyMeasurement = new WaterMeasurement()
                    {
                        WaterMeasurementTypeID = extractedAgainstSupplyMeasurementType.WaterMeasurementTypeID,
                        GeographyID = geographyID,
                        UnitTypeID = extractedGroundwaterMeasurement.UnitTypeID,
                        UsageEntityName = extractedGroundwaterMeasurement.UsageEntityName,
                        ReportedDate = extractedGroundwaterMeasurement.ReportedDate,
                        ReportedValue = 0,
                        ReportedValueInAcreFeet = 0,
                        UsageEntityArea = extractedGroundwaterMeasurement.UsageEntityArea,
                        LastUpdateDate = DateTime.UtcNow,
                        FromManualUpload = false,
                        Comment = $"Set to 0 because cumulative value for the reporting period for the WATER ACCOUNT was less than or equal to 0: WaterAccount ({waterAccount.Key}) = {cumulativeValueInAcreFeet} acreft."
                    };

                    if (!extractedAgainstSupplyMeasurementTypeByDate.ContainsKey(extractedGroundwaterMeasurement.ReportedDate))
                    {
                        extractedAgainstSupplyMeasurementTypeByDate.Add(extractedGroundwaterMeasurement.ReportedDate, [extractedAgainstSupplyMeasurement]);
                    }
                    else
                    {
                        extractedAgainstSupplyMeasurementTypeByDate[extractedGroundwaterMeasurement.ReportedDate].Add(extractedAgainstSupplyMeasurement);
                    }
                }
            }
            else
            {
                // If cumulative is positive, EAS = EG for all months in the period
                foreach (var extractedGroundwaterMeasurement in waterAccountGroundwaterMeasurements)
                {
                    var extractedAgainstSupplyMeasurement = new WaterMeasurement()
                    {
                        WaterMeasurementTypeID = extractedAgainstSupplyMeasurementType.WaterMeasurementTypeID,
                        GeographyID = geographyID,
                        UnitTypeID = extractedGroundwaterMeasurement.UnitTypeID,
                        UsageEntityName = extractedGroundwaterMeasurement.UsageEntityName,
                        ReportedDate = extractedGroundwaterMeasurement.ReportedDate,
                        ReportedValue = extractedGroundwaterMeasurement.ReportedValue,
                        ReportedValueInAcreFeet = extractedGroundwaterMeasurement.ReportedValueInAcreFeet,
                        UsageEntityArea = extractedGroundwaterMeasurement.UsageEntityArea,
                        LastUpdateDate = DateTime.UtcNow,
                        FromManualUpload = false,
                        Comment = $"Set to {extractedGroundwaterMeasurementType.WaterMeasurementTypeName} because cumulative value for the reporting period for the WATER ACCOUNT was greater than 0: WaterAccount ({waterAccount.Key}) = {cumulativeValueInAcreFeet} acreft."
                    };

                    if (!extractedAgainstSupplyMeasurementTypeByDate.ContainsKey(extractedGroundwaterMeasurement.ReportedDate))
                    {
                        extractedAgainstSupplyMeasurementTypeByDate.Add(extractedGroundwaterMeasurement.ReportedDate, [extractedAgainstSupplyMeasurement]);
                    }
                    else
                    {
                        extractedAgainstSupplyMeasurementTypeByDate[extractedGroundwaterMeasurement.ReportedDate].Add(extractedAgainstSupplyMeasurement);
                    }
                }
            }
        }

        foreach (var reportedDateKey in extractedAgainstSupplyMeasurementTypeByDate.Keys)
        {
            var waterMeasurements = extractedAgainstSupplyMeasurementTypeByDate[reportedDateKey];
            await SaveWaterMeasurements(dbContext, geographyID, reportedDateKey, waterMeasurements, extractedAgainstSupplyMeasurementType.WaterMeasurementTypeID);
        }
    }

    /// <summary>
    /// Calculates and stores the Consumptive Use for a given geography based on OpenET Evapotranspiration and OpenET Precipitation measurements. Consumptive Use is determined by subtracting precipitation from evapotranspiration for each usage entity.
    /// </summary>
    /// <param name="dbContext">The database context to interact with the Qanat database.</param>
    /// <param name="geographyID">The unique identifier for the geography in which the calculation is performed.</param>
    /// <param name="openETConsumptiveUseWaterMeasurementType">The WaterMeasurementType representing the calculated Consumptive Use.</param>
    /// <param name="evapoWaterMeasurementType">The WaterMeasurementType representing the evapotranspiration values.</param>
    /// <param name="precipWaterMeasurementType">The WaterMeasurementType representing the precipitation values.</param>
    /// <param name="consumedSurfaceWaterMeasurementType">The WaterMeasurementType representing the consumed surface water values.</param>
    /// <param name="reportedDate">The date of the recorded water measurements.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains no object, as the method saves the calculated Consumptive Use measurements directly to the database.
    /// </returns>
    private static async Task CalculateOpenETConsumptiveUse(QanatDbContext dbContext, int geographyID, WaterMeasurementType openETConsumptiveUseWaterMeasurementType, WaterMeasurementType evapoWaterMeasurementType, WaterMeasurementType precipWaterMeasurementType, WaterMeasurementType consumedSurfaceWaterMeasurementType, DateTime reportedDate)
    {
        var allWaterMeasurementsForGeographyAndDate = dbContext.WaterMeasurements.Include(x => x.WaterMeasurementType)
            .AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == reportedDate.Date && x.FromManualUpload == false)
            .ToList();

        var evapotranspirationWaterMeasurements = allWaterMeasurementsForGeographyAndDate
            .Where(x => x.WaterMeasurementType.WaterMeasurementTypeID == evapoWaterMeasurementType.WaterMeasurementTypeID)
            .ToList();

        var precipitationWaterMeasurements = allWaterMeasurementsForGeographyAndDate
            .Where(x => x.WaterMeasurementType.WaterMeasurementTypeID == precipWaterMeasurementType.WaterMeasurementTypeID)
            .ToList();

        var consumedSurfaceWaterMeasurements = allWaterMeasurementsForGeographyAndDate
            .Where(x => x.WaterMeasurementType.WaterMeasurementTypeID == consumedSurfaceWaterMeasurementType?.WaterMeasurementTypeID)
            .ToList();

        if (!evapotranspirationWaterMeasurements.Any() || !precipitationWaterMeasurements.Any())
        {
            return;
        }

        var consumptiveUseRecords = new List<WaterMeasurement>();

        // calculating consumptive use for parcels that have both evapo and precip water measurement values for the given effective date
        // handling multipolygons (i.e. multiple records with the same parcel number) by matching records on ParcelNumber and ParcelArea (column received from OpenET)
        // NOTE: will break if a parcel contains multiple polygons with exactly the same area
        foreach (var evapotranspirationWaterMeasurement in evapotranspirationWaterMeasurements)
        {
            var precipitationWaterMeasurement = precipitationWaterMeasurements
                .SingleOrDefault(x => x.UsageEntityName == evapotranspirationWaterMeasurement.UsageEntityName);

            if (precipitationWaterMeasurement == null)
            {
                continue;
            }

            var consumedSurfaceWaterMeasurement = consumedSurfaceWaterMeasurements
                .SingleOrDefault(x => x.UsageEntityName == evapotranspirationWaterMeasurement.UsageEntityName);

            var reportedValue = evapotranspirationWaterMeasurement.ReportedValue - precipitationWaterMeasurement.ReportedValue - (consumedSurfaceWaterMeasurement?.ReportedValue ?? 0);
            var reportedValueInAcreFeet = (evapotranspirationWaterMeasurement.ReportedValueInAcreFeet ?? 0) - (precipitationWaterMeasurement.ReportedValueInAcreFeet ?? 0) - (consumedSurfaceWaterMeasurement?.ReportedValueInAcreFeet ?? 0);

            var consumptiveUseRecord = new WaterMeasurement()
            {
                WaterMeasurementTypeID = openETConsumptiveUseWaterMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UnitTypeID = evapotranspirationWaterMeasurement.UnitTypeID,
                UsageEntityName = evapotranspirationWaterMeasurement.UsageEntityName,
                ReportedDate = reportedDate,
                UsageEntityArea = evapotranspirationWaterMeasurement.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = false,
                Comment = $"{evapoWaterMeasurementType.WaterMeasurementTypeName} Value: {evapotranspirationWaterMeasurement.ReportedValueInAcreFeet} ac-ft, {precipWaterMeasurementType.WaterMeasurementTypeName} Value: {precipitationWaterMeasurement.ReportedValueInAcreFeet} ac-ft",

                // months with heavy rain can lead to negative Consumptive Use calculations, so ensuring Reported Values cannot be < 0
                // also months with lots of surface water delivery can lead to negative Consumed Groundwater 
                ReportedValue = reportedValue > 0 ? reportedValue : 0,
                ReportedValueInAcreFeet = reportedValueInAcreFeet > 0 ? reportedValueInAcreFeet : 0,
            };

            consumptiveUseRecords.Add(consumptiveUseRecord);
        }

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, consumptiveUseRecords, openETConsumptiveUseWaterMeasurementType.WaterMeasurementTypeID);
    }

    private static async Task SaveWaterMeasurements(QanatDbContext dbContext, int geographyID, DateTime reportedDate, List<WaterMeasurement> newWaterMeasurements, int waterMeasurementTypeID)
    {
        await dbContext.WaterMeasurements.Where(x =>
                                                    x.GeographyID == geographyID && x.ReportedDate == reportedDate &&
                                                    x.WaterMeasurementTypeID == waterMeasurementTypeID && x.FromManualUpload == false)
            .ExecuteDeleteAsync();

        await dbContext.WaterMeasurements.AddRangeAsync(newWaterMeasurements);
        await dbContext.SaveChangesAsync();
    }
}