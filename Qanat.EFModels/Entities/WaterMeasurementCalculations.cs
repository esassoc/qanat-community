using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Qanat.EFModels.Entities;

public static class WaterMeasurementCalculations
{
    public static async Task RunAllMeasurementTypesForGeographyAsync(QanatDbContext dbContext, int geographyID, DateTime effectiveDate, List<int> usageLocationIDs = null)
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
            await RunCalculationAsync(dbContext, geographyID, effectiveDate, waterMeasurementType, usageLocationIDs);
        }
    }

    public static async Task RunMeasurementTypeForGeographyAsync(QanatDbContext dbContext, int geographyID, int waterMeasurementTypeID, DateTime effectiveDate, List<int> usageLocationIDs = null)
    {
        var waterMeasurementType = await dbContext.WaterMeasurementTypes
            .Include(x => x.WaterMeasurementTypeDependencyWaterMeasurementTypes).ThenInclude(x => x.DependsOnWaterMeasurementType) //MK 8/15/2024 -- Need dependencies to run calculations.
            .Include(x => x.WaterMeasurementTypeDependencyDependsOnWaterMeasurementTypes).ThenInclude(x => x.WaterMeasurementType) //MK 8/15/2024 -- Need the inverse relationship to crawl the dependency tree, wish the generated names were a bit more friendly.
            .AsNoTracking()
            .SingleAsync(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == waterMeasurementTypeID && x.IsActive);

        //MK 8/15/2024 -- If the water measurement type has a calculation, run the calculation and then recalculate the dependants. Otherwise, skip it and just recalculate the dependants.
        if (waterMeasurementType.WaterMeasurementCalculationTypeID.HasValue)
        {
            await RunCalculationAsync(dbContext, geographyID, effectiveDate, waterMeasurementType);
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
            await RunCalculationAsync(dbContext, geographyID, effectiveDate, dependantType, usageLocationIDs);
        }
    }

    private static async Task RunCalculationAsync(QanatDbContext dbContext, int geographyID, DateTime effectiveDate, WaterMeasurementType waterMeasurementType, List<int> usageLocationIDs = null)
    {
        if (waterMeasurementType.WaterMeasurementCalculationType == null)
        {
            throw new ArgumentException($"Calculation method not found for {waterMeasurementType.WaterMeasurementTypeName}");
        }

        switch (waterMeasurementType.WaterMeasurementCalculationType!.ToEnum)
        {
            case WaterMeasurementCalculationTypeEnum.CalculateEffectivePrecipByZone:
                //CalculateEffectivePrecipByZone assumes one dependency that is of Precip category.
                var precipDependency = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.Precip.WaterMeasurementCategoryTypeID);

                var precipMeasurementType = precipDependency?.DependsOnWaterMeasurementType;
                if (precipMeasurementType != null)
                {
                    await CalculateEffectivePrecipByZone(dbContext, geographyID, waterMeasurementType, precipMeasurementType, effectiveDate, usageLocationIDs);
                }
                else
                {
                    throw new ArgumentException("Missing dependency for CalculateEffectivePrecipByZone.");
                }
                break;

            case WaterMeasurementCalculationTypeEnum.CalculateEffectivePrecipByScalarValue:
                //CalculateEffectivePrecipByScalarValue assumes one dependency that is of Precip category.
                precipDependency = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.Precip.WaterMeasurementCategoryTypeID);

                precipMeasurementType = precipDependency?.DependsOnWaterMeasurementType;
                if (precipMeasurementType != null)
                {
                    await CalculateEffectivePrecipByScalarValue(dbContext, geographyID, waterMeasurementType, precipMeasurementType, effectiveDate, usageLocationIDs);
                }
                else
                {
                    throw new ArgumentException("Missing dependency for CalculateEffectivePrecipByScalarValue.");
                }
                break;

            case WaterMeasurementCalculationTypeEnum.CoverCropAdjustment:
                //CalculateCoverCropAdjustment assumes one dependency that is of Precip category.
                precipDependency = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.Precip.WaterMeasurementCategoryTypeID);

                precipMeasurementType = precipDependency?.DependsOnWaterMeasurementType;
                if (precipMeasurementType != null)
                {
                    await CalculateCoverCropAdjustment(dbContext, geographyID, waterMeasurementType, precipMeasurementType, effectiveDate, usageLocationIDs);
                }
                else
                {
                    throw new ArgumentException("Missing dependency for CalculateEffectivePrecipByScalarValue.");
                }
                break;

            case WaterMeasurementCalculationTypeEnum.CalculateSurfaceWaterConsumption:
                //CalculateSurfaceWaterConsumption assumes one dependency that is of SurfaceWater category.
                var surfaceWaterDependency = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.SurfaceWater.WaterMeasurementCategoryTypeID);

                var surfaceWaterMeasurementType = surfaceWaterDependency?.DependsOnWaterMeasurementType;
                if (surfaceWaterMeasurementType != null)
                {
                    await CalculateSurfaceWaterConsumption(dbContext, geographyID, waterMeasurementType, surfaceWaterMeasurementType, effectiveDate, usageLocationIDs);
                }
                else
                {
                    throw new ArgumentException("Missing dependency for CalculateSurfaceWaterConsumption.");
                }
                break;

            case WaterMeasurementCalculationTypeEnum.ETMinusPrecipMinusTotalSurfaceWater:
                //ETMinusPrecipMinusTotalSurfaceWater assumes one dependency that has a CalculateEffectivePrecipByZone or CalculateEffectivePrecipByScalarValue and 1 to N that are of category SurfaceWater or have a CalculationName of CalculateSurfaceWaterConsumption.
                var precipitationDependency = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.CalculateEffectivePrecipByZone.WaterMeasurementCalculationTypeID
                                      || x.DependsOnWaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.CalculateEffectivePrecipByScalarValue.WaterMeasurementCalculationTypeID);

                var precipitationMeasurementType = precipitationDependency?.DependsOnWaterMeasurementType;

                var coverCroppedAdjustmentMeasurementType = await dbContext.WaterMeasurementTypes.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.CoverCropAdjustment.WaterMeasurementCalculationTypeID);

                var surfaceWaterMeasurementTypes = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .Where(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.SurfaceWater.WaterMeasurementCategoryTypeID || x.DependsOnWaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.CalculateSurfaceWaterConsumption.WaterMeasurementCalculationTypeID)
                    .Select(x => x.DependsOnWaterMeasurementType)
                    .ToList();

                var surfaceWaterMeasurementTypeNames = surfaceWaterMeasurementTypes.Select(x => x.WaterMeasurementTypeName).ToList();
                if (precipitationMeasurementType != null && surfaceWaterMeasurementTypeNames.Any())
                {
                    await ETMinusPrecipMinusTotalSurfaceWater(dbContext, geographyID, effectiveDate, waterMeasurementType, precipitationMeasurementType, coverCroppedAdjustmentMeasurementType, surfaceWaterMeasurementTypeNames, usageLocationIDs);
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
                    await CalculatePrecipitationCreditOffset(dbContext, geographyID, waterMeasurementType, etMinusPrecipMinusTotalSurfaceWaterMeasurementTypeForCredit, effectiveDate, usageLocationIDs);
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
                    await CalculatePositiveConsumedGroundwater(dbContext, geographyID, waterMeasurementType, etMinusPrecipMinusTotalSurfaceWaterMeasurementType, effectiveDate, usageLocationIDs);
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
                    await CalculateUnadjustedExtractedGroundwater(dbContext, geographyID, waterMeasurementType, consumedGroundwaterWithCreditMeasurementTypeUnadjusted, effectiveDate, usageLocationIDs);
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
                    await CalculateExtractedGroundwater(dbContext, geographyID, waterMeasurementType, consumedGroundwaterWithCreditMeasurementType, extractedGroundwaterAdjustmentType, effectiveDate, usageLocationIDs);
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
                    await CalculateExtractedAgainstSupply(dbContext, geographyID, waterMeasurementType, extractedGroundwaterMeasurementType, effectiveDate, usageLocationIDs);
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
                    await CalculateOpenETConsumptiveUse(dbContext, geographyID, waterMeasurementType, openETEvapoMeasurementType, openETPrecipMeasurementType, consumedSurfaceWaterMeasurementType, effectiveDate, usageLocationIDs);
                }
                else
                {
                    throw new ArgumentException("Missing dependency for CalculateOpenETConsumptiveUse.");
                }
                break;
            case WaterMeasurementCalculationTypeEnum.CalculateConsumedGroundwater:

                //CalculateOpenETConsumptiveUse assumes one dependency that is of category ET and one that is of category Precip Credit.
                openETEvapoDependency = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryType.ToEnum == WaterMeasurementCategoryTypeEnum.ET);

                openETEvapoMeasurementType = openETEvapoDependency?.DependsOnWaterMeasurementType;

                var precipCredit = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .FirstOrDefault(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryType.ToEnum == WaterMeasurementCategoryTypeEnum.PrecipitationCredit);

                var precipCreditMeasurementType = precipCredit?.DependsOnWaterMeasurementType;

                var surfaceWaterDependencies = waterMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
                    .Where(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryType?.ToEnum == WaterMeasurementCategoryTypeEnum.SurfaceWater);

                surfaceWaterMeasurementTypes = surfaceWaterDependencies.Select(x => x.DependsOnWaterMeasurementType).ToList();

                if (openETEvapoMeasurementType != null && precipCreditMeasurementType != null)
                {
                    await CalculateConsumedGroundwater(dbContext, geographyID, waterMeasurementType, openETEvapoMeasurementType, precipCreditMeasurementType, surfaceWaterMeasurementTypes, effectiveDate, usageLocationIDs);
                }
                else
                {
                    throw new ArgumentException("Missing dependency for CalculateConsumedGroundwater.");
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
    private static async Task CalculateEffectivePrecipByZone(QanatDbContext dbContext, int geographyID, WaterMeasurementType effectivePrecipMeasurementType, WaterMeasurementType precipWaterMeasurementType, DateTime reportedDate, List<int> usageLocationIDs = null)
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

        var zoneIDsByUsageLocationNames = dbContext.UsageLocations.AsNoTracking()
            .Include(x => x.Parcel)
            .Where(x => x.GeographyID == geographyID)
            .AsEnumerable()
            .Where(x => zoneIDByParcelIDs.ContainsKey(x.ParcelID))
            .ToLookup(x => x.Name, x => zoneIDByParcelIDs[x.ParcelID]);

        var precipWaterMeasurements = await WaterMeasurements.ListByGeographyIDAndReportedDateAndOptionalWaterMeasurementTypeIDAndOptionalUsageLocationIDs(dbContext, geographyID, reportedDate, precipWaterMeasurementType.WaterMeasurementTypeID, usageLocationIDs);

        var precipMultiplierByZoneID = await dbContext.Zones.AsNoTracking()
            .Where(x => x.ZoneGroupID == geographyAllocationPlanConfiguration.ZoneGroupID)
            .ToDictionaryAsync(x => x.ZoneID, x => x.PrecipMultiplier);

        var effectivePrecipWaterMeasurements = new List<WaterMeasurement>();
        foreach (var precipWaterMeasurement in precipWaterMeasurements)
        {
            if (!zoneIDsByUsageLocationNames[precipWaterMeasurement.UsageLocation.Name].Any())
            {
                continue;
            }

            var zoneID = zoneIDsByUsageLocationNames[precipWaterMeasurement.UsageLocation.Name].First();
            if (!precipMultiplierByZoneID.ContainsKey(zoneID) || !precipMultiplierByZoneID[zoneID].HasValue)
            {
                throw new Exception("Could not calculate Effective Precip because at least one Allocation Zone does not have a precipitation multiplier configured.");
            }

            var effectivePrecipMultiplier = precipMultiplierByZoneID[zoneID].Value;
            var volume = precipWaterMeasurement.ReportedValueInAcreFeet * effectivePrecipMultiplier;
            var depth = volume / (decimal)precipWaterMeasurement.UsageLocation.Area;
            effectivePrecipWaterMeasurements.Add(new WaterMeasurement()
            {
                WaterMeasurementTypeID = effectivePrecipMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UsageLocationID = precipWaterMeasurement.UsageLocationID,
                ReportedDate = reportedDate,
                ReportedValueInAcreFeet = volume,
                ReportedValueInFeet = depth,
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = false,
                Comment = $"{precipWaterMeasurementType.WaterMeasurementTypeName} Value: {precipWaterMeasurement.ReportedValueInAcreFeet} ac-ft, Effective Precipitation Factor: {effectivePrecipMultiplier}"
            });
        }

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, effectivePrecipWaterMeasurements, effectivePrecipMeasurementType.WaterMeasurementTypeID, usageLocationIDs);
    }

    public class EffectivePrecipitationCalculationDto
    {
        public decimal EffectivePrecipitationMultiplier { get; set; }
        public decimal CoverCropEffectivePrecipitationMultiplier { get; set; }
    }

    private static async Task CalculateEffectivePrecipByScalarValue(QanatDbContext dbContext, int geographyID, WaterMeasurementType effectivePrecipMeasurementType, WaterMeasurementType precipWaterMeasurementType, DateTime reportedDate, List<int> usageLocationIDs = null)
    {
        //Extracted groundwater consumption requires a Groundwater Water Efficiency Factor to be set in the calculation JSON.
        var calculationJSON = JsonSerializer.Deserialize<EffectivePrecipitationCalculationDto>(effectivePrecipMeasurementType.CalculationJSON);
        var precipitationMultiplier =  calculationJSON.EffectivePrecipitationMultiplier;

        var openETPrecipitationMeasurements = await WaterMeasurements.ListByGeographyIDAndReportedDateAndOptionalWaterMeasurementTypeIDAndOptionalUsageLocationIDs(dbContext, geographyID, reportedDate, precipWaterMeasurementType.WaterMeasurementTypeID, usageLocationIDs);

        var newWaterMeasurements = new List<WaterMeasurement>();
        foreach (var precipitationMeasurement in openETPrecipitationMeasurements)
        {
            var volume = precipitationMeasurement.ReportedValueInAcreFeet * precipitationMultiplier;
            var depth = volume / (decimal)precipitationMeasurement.UsageLocation.Area;
            var newUnadjustedGroundwaterMeasurement = new WaterMeasurement()
            {
                WaterMeasurementTypeID = effectivePrecipMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UsageLocationID = precipitationMeasurement.UsageLocationID,
                ReportedDate = reportedDate,
                ReportedValueInAcreFeet = volume,
                ReportedValueInFeet = depth,
                FromManualUpload = false,
                Comment = $"{effectivePrecipMeasurementType.WaterMeasurementTypeName} Value: {volume} ac-ft, Effective Precipitation Multiplier: {precipitationMultiplier}",
                LastUpdateDate = DateTime.UtcNow
            };

            newWaterMeasurements.Add(newUnadjustedGroundwaterMeasurement);
        }

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, newWaterMeasurements, effectivePrecipMeasurementType.WaterMeasurementTypeID, usageLocationIDs);
    }

    private static async Task CalculateCoverCropAdjustment(QanatDbContext dbContext, int geographyID, WaterMeasurementType coverCropAdjustmentMeasurementType, WaterMeasurementType precipWaterMeasurementType, DateTime reportedDate, List<int> usageLocationIDs = null)
    {
        var calculationJSON = JsonSerializer.Deserialize<EffectivePrecipitationCalculationDto>(coverCropAdjustmentMeasurementType.CalculationJSON);

        var openETPrecipitationMeasurements = await WaterMeasurements.ListByGeographyIDAndReportedDateAndOptionalWaterMeasurementTypeIDAndOptionalUsageLocationIDs(dbContext, geographyID, reportedDate, precipWaterMeasurementType.WaterMeasurementTypeID, usageLocationIDs);

        var newWaterMeasurements = new List<WaterMeasurement>();
        var waterMeasurementIDsToDelete = new List<int>();

        foreach (var precipitationMeasurement in openETPrecipitationMeasurements)
        {
            var isCoverCropped = precipitationMeasurement.UsageLocation.UsageLocationType.WaterMeasurementType != null;
            if (isCoverCropped)
            {
                var precipitationMultiplier = calculationJSON.EffectivePrecipitationMultiplier - calculationJSON.CoverCropEffectivePrecipitationMultiplier;

                var volume = precipitationMeasurement.ReportedValueInAcreFeet * precipitationMultiplier;
                var depth = volume / (decimal)precipitationMeasurement.UsageLocation.Area;
                var coverCropAdjustment = new WaterMeasurement()
                {
                    WaterMeasurementTypeID = coverCropAdjustmentMeasurementType.WaterMeasurementTypeID,
                    GeographyID = geographyID,
                    UsageLocationID = precipitationMeasurement.UsageLocationID,
                    ReportedDate = reportedDate,
                    ReportedValueInAcreFeet = volume,
                    ReportedValueInFeet = depth,
                    FromManualUpload = false,
                    Comment = $"{coverCropAdjustmentMeasurementType.WaterMeasurementTypeName} Value: {volume} ac-ft, Cover Crop Adjustment Multiplier: {precipitationMultiplier}",
                    LastUpdateDate = DateTime.UtcNow
                };

                newWaterMeasurements.Add(coverCropAdjustment);
            }
            else
            {
                var waterMeasurementToDelete = await dbContext.WaterMeasurements.AsNoTracking()
                    .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == coverCropAdjustmentMeasurementType.WaterMeasurementTypeID && x.UsageLocationID == precipitationMeasurement.UsageLocationID && x.ReportedDate == reportedDate);

                if (waterMeasurementToDelete != null)
                {
                    waterMeasurementIDsToDelete.Add(waterMeasurementToDelete.WaterMeasurementID);
                }
            }
        }

        await dbContext.WaterMeasurements.Where(x => waterMeasurementIDsToDelete.Contains(x.WaterMeasurementID)).ExecuteDeleteAsync();

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, newWaterMeasurements, coverCropAdjustmentMeasurementType.WaterMeasurementTypeID, usageLocationIDs);
    }

    public class SurfaceWaterConsumptionCalculationDto
    {
        public decimal SurfaceWaterEfficiencyFactor { get; set; }
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
    private static async Task CalculateSurfaceWaterConsumption(QanatDbContext dbContext, int geographyID, WaterMeasurementType consumedSurfaceWaterMeasurementType, WaterMeasurementType surfaceWaterMeasurementType, DateTime reportedDate, List<int> usageLocationIDs = null)
    {
        //Surface water consumption requires a Surface Water Efficiency Factor to be set in the calculation JSON.
        var calculationJSON = System.Text.Json.JsonSerializer.Deserialize<SurfaceWaterConsumptionCalculationDto>(consumedSurfaceWaterMeasurementType.CalculationJSON);

        var surfaceWaterEfficiencyFactor = calculationJSON.SurfaceWaterEfficiencyFactor;
        var surfaceWaterMeasurements = await WaterMeasurements.ListByGeographyIDAndReportedDateAndOptionalWaterMeasurementTypeIDAndOptionalUsageLocationIDs(dbContext, geographyID, reportedDate, surfaceWaterMeasurementType.WaterMeasurementTypeID, usageLocationIDs);

        var consumedSurfaceWaterMeasurements = new List<WaterMeasurement>();
        foreach (var surfaceWaterDelivery in surfaceWaterMeasurements)
        {
            var volume = surfaceWaterDelivery.ReportedValueInAcreFeet * surfaceWaterEfficiencyFactor;
            var depth = volume / (decimal)surfaceWaterDelivery.UsageLocation.Area;
            var newSurfaceWaterConsumption = new WaterMeasurement()
            {
                WaterMeasurementTypeID = consumedSurfaceWaterMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UsageLocationID = surfaceWaterDelivery.UsageLocationID,
                ReportedDate = reportedDate,
                ReportedValueInAcreFeet = volume,
                ReportedValueInFeet = depth,
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = false,
                Comment = $"{consumedSurfaceWaterMeasurementType.WaterMeasurementTypeName} Value: {surfaceWaterDelivery.ReportedValueInAcreFeet} ac-ft, Surface Water Efficiency Factor: {surfaceWaterEfficiencyFactor}"
            };

            consumedSurfaceWaterMeasurements.Add(newSurfaceWaterConsumption);
        }

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, consumedSurfaceWaterMeasurements, consumedSurfaceWaterMeasurementType.WaterMeasurementTypeID, usageLocationIDs);
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
    private static async Task ETMinusPrecipMinusTotalSurfaceWater(QanatDbContext dbContext, int geographyID, DateTime reportedDate, WaterMeasurementType consumedGroundWaterMeasurementType, WaterMeasurementType effectivePrecipWaterMeasurementType, WaterMeasurementType coverCropMeasurementType, List<string> surfaceWaterTypeNames, List<int> usageLocationIDs = null)
    {
        var allWaterMeasurementsForGeographyAndDate = await WaterMeasurements.ListByGeographyIDAndReportedDateAndOptionalWaterMeasurementTypeIDAndOptionalUsageLocationIDs(dbContext, geographyID, reportedDate, null, usageLocationIDs);

        var evapotranspirationWaterMeasurements = allWaterMeasurementsForGeographyAndDate
            .Where(x => x.WaterMeasurementType.WaterMeasurementCategoryType?.ToEnum == WaterMeasurementCategoryTypeEnum.ET)
            .ToList();

        var precipitationWaterMeasurements = allWaterMeasurementsForGeographyAndDate
            .Where(x => x.WaterMeasurementType.WaterMeasurementTypeID == effectivePrecipWaterMeasurementType.WaterMeasurementTypeID)
            .ToList();

        var coverCropWaterMeasurements = allWaterMeasurementsForGeographyAndDate
            .Where(x => x.WaterMeasurementTypeID == coverCropMeasurementType?.WaterMeasurementTypeID)
            .ToList();

        var surfaceWaterMeasurements = allWaterMeasurementsForGeographyAndDate
            .Where(x => surfaceWaterTypeNames.Contains(x.WaterMeasurementType.WaterMeasurementTypeName))
            .ToList();

        var newWaterMeasurements = new List<WaterMeasurement>();
        foreach (var evapoWaterMeasurement in evapotranspirationWaterMeasurements)
        {
            var precipWaterMeasurement = precipitationWaterMeasurements.FirstOrDefault(x => x.UsageLocationID == evapoWaterMeasurement.UsageLocationID);
            var coverCropAdjustmentMeasurement = coverCropWaterMeasurements.FirstOrDefault(x => x.UsageLocationID == evapoWaterMeasurement.UsageLocationID);
            var surfaceWaterMeasurementsForUsageLocation = surfaceWaterMeasurements.Where(x => x.UsageLocationID == evapoWaterMeasurement.UsageLocationID).ToList();

            var surfaceWaterMeasurementSumInAcreFeet = surfaceWaterMeasurementsForUsageLocation.Any()
                ? surfaceWaterMeasurementsForUsageLocation.Sum(x => x.ReportedValueInAcreFeet)
                : 0;

            var volume = evapoWaterMeasurement.ReportedValueInAcreFeet - (precipWaterMeasurement?.ReportedValueInAcreFeet ?? 0 ) - (coverCropAdjustmentMeasurement?.ReportedValueInAcreFeet ?? 0) - surfaceWaterMeasurementSumInAcreFeet;
            var depth = volume / (decimal)evapoWaterMeasurement.UsageLocation.Area;

            var coverCropComment = coverCropMeasurementType != null && coverCropAdjustmentMeasurement != null
                ? $" {coverCropMeasurementType.WaterMeasurementTypeName} Value: {coverCropAdjustmentMeasurement.ReportedValueInAcreFeet} ac-ft,"
                : string.Empty;

            newWaterMeasurements.Add(new WaterMeasurement()
            {
                WaterMeasurementTypeID = consumedGroundWaterMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UsageLocationID = evapoWaterMeasurement.UsageLocationID,
                ReportedDate = reportedDate,
                ReportedValueInAcreFeet = volume,
                ReportedValueInFeet = depth,
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = false,
                Comment = $"{evapoWaterMeasurement.WaterMeasurementType.WaterMeasurementTypeName} Value: {evapoWaterMeasurement.ReportedValueInAcreFeet} ac-ft, {effectivePrecipWaterMeasurementType.WaterMeasurementTypeName} Value: {precipWaterMeasurement?.ReportedValueInAcreFeet} ac-ft,{coverCropComment} Total Surface Water Value: {surfaceWaterMeasurementSumInAcreFeet} ac-ft"
            });
        }

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, newWaterMeasurements, consumedGroundWaterMeasurementType.WaterMeasurementTypeID, usageLocationIDs);
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
    private static async Task CalculatePositiveConsumedGroundwater(QanatDbContext dbContext, int geographyID, WaterMeasurementType positiveConsumedGroundwaterMeasurementType, WaterMeasurementType consumedGroundwaterMeasurementType, DateTime reportedDate, List<int> usageLocationIDs = null)
    {
        var consumedGroundwaterMeasurements = await WaterMeasurements.ListByGeographyIDAndReportedDateAndOptionalWaterMeasurementTypeIDAndOptionalUsageLocationIDs(dbContext, geographyID, reportedDate, consumedGroundwaterMeasurementType.WaterMeasurementTypeID, usageLocationIDs);

        var newPositiveConsumedGroundwaterMeasurements = new List<WaterMeasurement>();
        foreach (var waterMeasurement in consumedGroundwaterMeasurements)
        {
            var volume = waterMeasurement.ReportedValueInAcreFeet > 0
                ? waterMeasurement.ReportedValueInAcreFeet
                : 0;
            var depth = volume / (decimal)waterMeasurement.UsageLocation.Area;

            // positive consumed groundwater
            newPositiveConsumedGroundwaterMeasurements.Add(new WaterMeasurement()
            {
                WaterMeasurementTypeID = positiveConsumedGroundwaterMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UsageLocationID = waterMeasurement.UsageLocationID,
                ReportedDate = reportedDate,
                ReportedValueInAcreFeet = volume,
                ReportedValueInFeet = depth,
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = false,
                Comment = $"{consumedGroundwaterMeasurementType} Value: {waterMeasurement.ReportedValueInAcreFeet} ac-ft"
            });
        }

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, newPositiveConsumedGroundwaterMeasurements, positiveConsumedGroundwaterMeasurementType.WaterMeasurementTypeID, usageLocationIDs);
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
    private static async Task CalculatePrecipitationCreditOffset(QanatDbContext dbContext, int geographyID, WaterMeasurementType precipitationCreditMeasurementType, WaterMeasurementType consumedGroundwaterMeasurementType, DateTime reportedDate, List<int> usageLocationIDs = null)
    {
        var consumedGroundwaterMeasurements = await dbContext.WaterMeasurements.AsNoTracking()
            .Include(x => x.UsageLocation)
            .Where(x => x.WaterMeasurementTypeID == consumedGroundwaterMeasurementType.WaterMeasurementTypeID && x.ReportedDate.Date == reportedDate && (usageLocationIDs == null || usageLocationIDs.Contains(x.UsageLocationID)))
            .ToListAsync();

        var newPrecipitationCreditMeasurements = new List<WaterMeasurement>();
        foreach (var waterMeasurement in consumedGroundwaterMeasurements)
        {
            var volume = waterMeasurement.ReportedValueInAcreFeet < 0
                ? waterMeasurement.ReportedValueInAcreFeet
                : 0;

            var depth = volume / (decimal)waterMeasurement.UsageLocation.Area;

            // precipitation credit offset
            newPrecipitationCreditMeasurements.Add(new WaterMeasurement()
            {
                WaterMeasurementTypeID = precipitationCreditMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UsageLocationID = waterMeasurement.UsageLocationID,
                ReportedDate = reportedDate,
                ReportedValueInAcreFeet = volume,
                ReportedValueInFeet = depth,
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = false,
                Comment = $"{consumedGroundwaterMeasurementType} Value: {waterMeasurement.ReportedValueInAcreFeet} ac-ft"
            });
        }

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, newPrecipitationCreditMeasurements, precipitationCreditMeasurementType.WaterMeasurementTypeID, usageLocationIDs);
    }


    public class GroundwaterCalculationDto
    {
        public decimal GroundwaterEfficiencyFactor { get; set; }
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
    private static async Task CalculateUnadjustedExtractedGroundwater(QanatDbContext dbContext, int geographyID, WaterMeasurementType unadjustedGroundwaterMeasurementType, WaterMeasurementType consumedGroundwaterMeasurementType, DateTime reportedDate, List<int> usageLocationIDs = null)
    {
        //Extracted groundwater consumption requires a Groundwater Water Efficiency Factor to be set in the calculation JSON.
        var calculationJSON = System.Text.Json.JsonSerializer.Deserialize<GroundwaterCalculationDto>(unadjustedGroundwaterMeasurementType.CalculationJSON);
        var groundwaterEfficiencyFactor = calculationJSON.GroundwaterEfficiencyFactor;
        var consumedGroundWaterMeasurements = await dbContext.WaterMeasurements.AsNoTracking()
            .Include(x => x.WaterMeasurementType)
            .Include(x => x.UsageLocation)
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementType.WaterMeasurementTypeID == consumedGroundwaterMeasurementType.WaterMeasurementTypeID && x.ReportedDate.Date == reportedDate.Date && (usageLocationIDs == null || usageLocationIDs.Contains(x.UsageLocationID)))
            .ToListAsync();

        var newWaterMeasurements = new List<WaterMeasurement>();
        foreach (var consumedGroundWaterMeasurement in consumedGroundWaterMeasurements)
        {
            var volume = consumedGroundWaterMeasurement.ReportedValueInAcreFeet / groundwaterEfficiencyFactor;
            var depth = volume / (decimal)consumedGroundWaterMeasurement.UsageLocation.Area;
            var newUnadjustedGroundwaterMeasurement = new WaterMeasurement()
            {
                WaterMeasurementTypeID = unadjustedGroundwaterMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UsageLocationID = consumedGroundWaterMeasurement.UsageLocationID,
                ReportedDate = reportedDate,
                ReportedValueInAcreFeet = volume,
                ReportedValueInFeet = depth,
                FromManualUpload = false,
                Comment = $"{consumedGroundwaterMeasurementType.WaterMeasurementTypeName} Value: {consumedGroundWaterMeasurement.ReportedValueInAcreFeet} ac-ft, Groundwater Efficiency Factor: {groundwaterEfficiencyFactor}",
                LastUpdateDate = DateTime.UtcNow
            };

            newWaterMeasurements.Add(newUnadjustedGroundwaterMeasurement);
        }

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, newWaterMeasurements, unadjustedGroundwaterMeasurementType.WaterMeasurementTypeID, usageLocationIDs);
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
    private static async Task CalculateExtractedGroundwater(QanatDbContext dbContext, int geographyID, WaterMeasurementType extractedGroundwaterMeasurementType, WaterMeasurementType consumedGroundwaterMeasurementType, WaterMeasurementType extractedGroundwaterAdjustmentMeasurementType, DateTime reportedDate, List<int> usageLocationIDs = null)
    {
        //Extracted groundwater consumption requires a Groundwater Water Efficiency Factor to be set in the calculation JSON.
        var calculationJSON = System.Text.Json.JsonSerializer.Deserialize<GroundwaterCalculationDto>(extractedGroundwaterMeasurementType.CalculationJSON);
        var groundwaterEfficiencyFactor = calculationJSON.GroundwaterEfficiencyFactor;
        var consumedGroundWaterMeasurements = await WaterMeasurements.ListByGeographyIDAndReportedDateAndOptionalWaterMeasurementTypeIDAndOptionalUsageLocationIDs(dbContext, geographyID, reportedDate, consumedGroundwaterMeasurementType.WaterMeasurementTypeID, usageLocationIDs);
        var extractedGroundwaterAdjustments = await WaterMeasurements.ListByGeographyIDAndReportedDateAndOptionalWaterMeasurementTypeIDAndOptionalUsageLocationIDs(dbContext, geographyID, reportedDate, extractedGroundwaterAdjustmentMeasurementType.WaterMeasurementTypeID, usageLocationIDs);

        var newWaterMeasurements = new List<WaterMeasurement>();
        foreach (var consumedGroundWaterMeasurement in consumedGroundWaterMeasurements)
        {
            var extractedGroundwaterAdjustment = extractedGroundwaterAdjustments.FirstOrDefault(x => x.GeographyID == consumedGroundWaterMeasurement.GeographyID && x.ReportedDate.Date == reportedDate.Date && x.UsageLocationID == consumedGroundWaterMeasurement.UsageLocationID);

            var adjustmentValueInAcreFeet = extractedGroundwaterAdjustment?.ReportedValueInAcreFeet ?? 0;
            var volume = (consumedGroundWaterMeasurement.ReportedValueInAcreFeet / groundwaterEfficiencyFactor) + adjustmentValueInAcreFeet;
            var depth = volume / (decimal)consumedGroundWaterMeasurement.UsageLocation.Area;

            var newExtractedGroundwaterMeasurement = new WaterMeasurement()
            {
                WaterMeasurementTypeID = extractedGroundwaterMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UsageLocationID = consumedGroundWaterMeasurement.UsageLocationID,
                ReportedDate = reportedDate,
                ReportedValueInAcreFeet = volume,
                ReportedValueInFeet = depth,
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = false,
                Comment = $"{consumedGroundwaterMeasurementType.WaterMeasurementTypeName} Value: {consumedGroundWaterMeasurement.ReportedValueInAcreFeet} acre-ft, Groundwater Efficiency Factor: {groundwaterEfficiencyFactor}, Adjustment={adjustmentValueInAcreFeet}"
            };

            newWaterMeasurements.Add(newExtractedGroundwaterMeasurement);
        }

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, newWaterMeasurements, extractedGroundwaterMeasurementType.WaterMeasurementTypeID, usageLocationIDs);
    }

    private static async Task CalculateExtractedAgainstSupply(QanatDbContext dbContext, int geographyID, WaterMeasurementType extractedAgainstSupplyMeasurementType, WaterMeasurementType extractedGroundwaterMeasurementType, DateTime reportedDate, List<int> usageLocationIDs = null)
    {
        var extractedAgainstSupplyMeasurementTypeByDate = new Dictionary<DateTime, List<WaterMeasurement>>();

        var reportedYear = reportedDate.Year;
        var reportingPeriodDto = await ReportingPeriods.GetByGeographyIDAndYearAsync(dbContext, geographyID, reportedYear);
        var reportingPeriodStart = reportingPeriodDto.StartDate;
        var reportingPeriodEnd = reportingPeriodDto.EndDate;

        var extractedGroundwaterMeasurements = await dbContext.WaterMeasurements.AsNoTracking()
            .Include(x => x.UsageLocation)
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == extractedGroundwaterMeasurementType.WaterMeasurementTypeID && x.ReportedDate >= reportingPeriodStart && x.ReportedDate <= reportingPeriodEnd && (usageLocationIDs == null || usageLocationIDs.Contains(x.UsageLocationID)))
            .ToListAsync();

        var waterAccounts = await dbContext.fWaterAccountParcelByGeographyAndYear(geographyID, reportedYear).AsNoTracking()
            .ToListAsync();

        foreach (var waterAccount in waterAccounts.GroupBy(x => x.WaterAccountID))
        {
            var activeParcelsNumbersInEffect = waterAccount.Select(x => x.ParcelNumber).ToList();
            var waterAccountGroundwaterMeasurements = extractedGroundwaterMeasurements.Where(x => activeParcelsNumbersInEffect.Contains(x.UsageLocation.Name)).ToList(); //MK 2/20/2025: Not sure this will work well 
            var cumulativeValueInAcreFeet = waterAccountGroundwaterMeasurements.Sum(x => x.ReportedValueInAcreFeet);

            //MK 12/20/2024 -- If a negative cumulative value, set the EAS to 0 for all months in the period. Updated business logic as of QAN-924.
            if (cumulativeValueInAcreFeet <= 0)
            {
                foreach (var extractedGroundwaterMeasurement in waterAccountGroundwaterMeasurements)
                {
                    var extractedAgainstSupplyMeasurement = new WaterMeasurement()
                    {
                        WaterMeasurementTypeID = extractedAgainstSupplyMeasurementType.WaterMeasurementTypeID,
                        GeographyID = geographyID,
                        UsageLocationID = extractedGroundwaterMeasurement.UsageLocationID,
                        UnitTypeID = extractedGroundwaterMeasurement.UnitTypeID,
                        ReportedDate = extractedGroundwaterMeasurement.ReportedDate,
                        ReportedValueInAcreFeet = 0,
                        ReportedValueInFeet = 0,
                        LastUpdateDate = DateTime.UtcNow,
                        FromManualUpload = false,
                        Comment = $"Set to 0 because cumulative value for the reporting period for the WATER ACCOUNT was less than or equal to 0: WaterAccount ({waterAccount.Key}) = {cumulativeValueInAcreFeet} ac-ft."
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
                    var volume = extractedGroundwaterMeasurement.ReportedValueInAcreFeet;
                    var depth = volume / (decimal)extractedGroundwaterMeasurement.UsageLocation.Area;
                    var extractedAgainstSupplyMeasurement = new WaterMeasurement()
                    {
                        WaterMeasurementTypeID = extractedAgainstSupplyMeasurementType.WaterMeasurementTypeID,
                        GeographyID = geographyID,
                        UsageLocationID = extractedGroundwaterMeasurement.UsageLocationID,
                        UnitTypeID = extractedGroundwaterMeasurement.UnitTypeID,
                        ReportedDate = extractedGroundwaterMeasurement.ReportedDate,
                        ReportedValueInAcreFeet = volume,
                        ReportedValueInFeet = depth,
                        LastUpdateDate = DateTime.UtcNow,
                        FromManualUpload = false,
                        Comment = $"Set to {extractedGroundwaterMeasurementType.WaterMeasurementTypeName} because cumulative value for the reporting period for the WATER ACCOUNT was greater than 0: WaterAccount ({waterAccount.Key}) = {cumulativeValueInAcreFeet} ac-ft."
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
            await SaveWaterMeasurements(dbContext, geographyID, reportedDateKey, waterMeasurements, extractedAgainstSupplyMeasurementType.WaterMeasurementTypeID, usageLocationIDs);
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
    private static async Task CalculateOpenETConsumptiveUse(QanatDbContext dbContext, int geographyID, WaterMeasurementType openETConsumptiveUseWaterMeasurementType, WaterMeasurementType evapoWaterMeasurementType, WaterMeasurementType precipWaterMeasurementType, WaterMeasurementType consumedSurfaceWaterMeasurementType, DateTime reportedDate, List<int> usageLocationIDs = null)
    {
        var allWaterMeasurementsForGeographyAndDate = await WaterMeasurements.ListByGeographyIDAndReportedDateAndOptionalWaterMeasurementTypeIDAndOptionalUsageLocationIDs(dbContext, geographyID, reportedDate, null, usageLocationIDs);
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
                .SingleOrDefault(x => x.UsageLocationID == evapotranspirationWaterMeasurement.UsageLocationID);

            if (precipitationWaterMeasurement == null)
            {
                continue;
            }

            var consumedSurfaceWaterMeasurement = consumedSurfaceWaterMeasurements
                .SingleOrDefault(x => x.UsageLocationID == evapotranspirationWaterMeasurement.UsageLocationID);

            var reportedValueInAcreFeet = (evapotranspirationWaterMeasurement.ReportedValueInAcreFeet) - (precipitationWaterMeasurement.ReportedValueInAcreFeet) - (consumedSurfaceWaterMeasurement?.ReportedValueInAcreFeet ?? 0);
            var volume = reportedValueInAcreFeet > 0
                ? reportedValueInAcreFeet
                : 0;

            var depth = volume / (decimal)evapotranspirationWaterMeasurement.UsageLocation.Area;

            var consumptiveUseRecord = new WaterMeasurement()
            {
                WaterMeasurementTypeID = openETConsumptiveUseWaterMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UsageLocationID = evapotranspirationWaterMeasurement.UsageLocationID,
                ReportedDate = reportedDate,
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = false,
                Comment = $"{evapoWaterMeasurementType.WaterMeasurementTypeName} Value: {evapotranspirationWaterMeasurement.ReportedValueInAcreFeet} ac-ft, {precipWaterMeasurementType.WaterMeasurementTypeName} Value: {precipitationWaterMeasurement.ReportedValueInAcreFeet} ac-ft",

                // months with heavy rain can lead to negative Consumptive Use calculations, so ensuring Reported Values cannot be < 0
                // also months with lots of surface water delivery can lead to negative Consumed Groundwater 
                ReportedValueInAcreFeet = volume,
                ReportedValueInFeet = depth
            };

            consumptiveUseRecords.Add(consumptiveUseRecord);
        }

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, consumptiveUseRecords, openETConsumptiveUseWaterMeasurementType.WaterMeasurementTypeID, usageLocationIDs);
    }

    /// <summary>
    /// Calculates and stores Consumed Groundwater for a given geography.
    /// </summary>
    /// <param name="dbContext">The database context to interact with the Qanat database.</param>
    /// <param name="geographyID">The unique identifier for the geography in which the calculation is performed.</param>
    /// <param name="consumedGroundwaterMeasurementType">The WaterMeasurementType representing the calculated Consumed Groundwater.</param>
    /// <param name="evapoWaterMeasurementType">The WaterMeasurementType representing the evapotranspiration values.</param>
    /// <param name="precipCreditWaterMeasurementType">The WaterMeasurementType representing the precipitation credit values.</param>
    /// <param name="surfaceWaterMeasurementTypes">The WaterMeasurementTypes representing the surface water values.</param>
    /// <param name="reportedDate">The date of the recorded water measurements.</param>
    /// <returns></returns>
    private static async Task CalculateConsumedGroundwater(QanatDbContext dbContext, int geographyID, WaterMeasurementType consumedGroundwaterMeasurementType, WaterMeasurementType evapoWaterMeasurementType, WaterMeasurementType precipCreditWaterMeasurementType, List<WaterMeasurementType> surfaceWaterMeasurementTypes, DateTime reportedDate, List<int> usageLocationIDs = null)
    {
        var allWaterMeasurementsForGeographyAndDate = await WaterMeasurements.ListByGeographyIDAndReportedDateAndOptionalWaterMeasurementTypeIDAndOptionalUsageLocationIDs(dbContext, geographyID, reportedDate, null, usageLocationIDs);

        var evapotranspirationWaterMeasurements = allWaterMeasurementsForGeographyAndDate
            .Where(x => x.WaterMeasurementType.WaterMeasurementTypeID == evapoWaterMeasurementType.WaterMeasurementTypeID)
            .ToList();

        var precipitationCreditWaterMeasurements = allWaterMeasurementsForGeographyAndDate
            .Where(x => x.WaterMeasurementType.WaterMeasurementTypeID == precipCreditWaterMeasurementType.WaterMeasurementTypeID)
            .ToList();

        var surfaceWaterMeasurementTypeIDs = surfaceWaterMeasurementTypes.Select(x => x.WaterMeasurementTypeID).ToList();
        var allSurfaceWaterMeasurements = allWaterMeasurementsForGeographyAndDate
            .Where(x => surfaceWaterMeasurementTypeIDs.Contains(x.WaterMeasurementTypeID!.Value))
            .ToList();

        if (!evapotranspirationWaterMeasurements.Any())
        {
            return;
        }

        var consumedGroundwaterRecords = new List<WaterMeasurement>();

        foreach (var evapotranspirationWaterMeasurement in evapotranspirationWaterMeasurements)
        {
            var precipitationCreditsWaterMeasurement = precipitationCreditWaterMeasurements
                .SingleOrDefault(x => x.UsageLocationID == evapotranspirationWaterMeasurement.UsageLocationID);

            var surfaceWaterMeasurements = allSurfaceWaterMeasurements
                .Where(x => x.UsageLocationID == evapotranspirationWaterMeasurement.UsageLocationID)
                .ToList();

            var surfaceWaterMeasurementReportedValueInAcreFeet = surfaceWaterMeasurements.Sum(x => x.ReportedValueInAcreFeet);

            var surfaceWaterMeasurementComment = "";
            var index = 0;
            foreach (var surfaceWaterMeasurementType in surfaceWaterMeasurementTypes)
            {
                var reportedValueInAcreFeetForSurfaceWater = surfaceWaterMeasurements.FirstOrDefault(x => x.WaterMeasurementTypeID == surfaceWaterMeasurementType.WaterMeasurementTypeID)?.ReportedValueInAcreFeet ?? 0;
                surfaceWaterMeasurementComment += $"{surfaceWaterMeasurementType.WaterMeasurementTypeName} Value: {reportedValueInAcreFeetForSurfaceWater} ac-ft, ";
                if (index == surfaceWaterMeasurementTypes.Count - 1)
                {
                    surfaceWaterMeasurementComment = surfaceWaterMeasurementComment.TrimEnd(',', ' ');
                }

                index++;
            }

            var volume = (evapotranspirationWaterMeasurement.ReportedValueInAcreFeet) - (precipitationCreditsWaterMeasurement?.ReportedValueInAcreFeet ?? 0) - surfaceWaterMeasurementReportedValueInAcreFeet;
            var depth = volume / (decimal)evapotranspirationWaterMeasurement.UsageLocation.Area;

            var consumptiveUseRecord = new WaterMeasurement()
            {
                GeographyID = geographyID,
                WaterMeasurementTypeID = consumedGroundwaterMeasurementType.WaterMeasurementTypeID,
                UsageLocationID = evapotranspirationWaterMeasurement.UsageLocationID,
                ReportedDate = reportedDate,
                ReportedValueInAcreFeet = volume,
                ReportedValueInFeet = depth,
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = false,
                Comment = $"{evapoWaterMeasurementType.WaterMeasurementTypeName} Value: {evapotranspirationWaterMeasurement.ReportedValueInAcreFeet} ac-ft, {precipCreditWaterMeasurementType.WaterMeasurementTypeName} Value: {precipitationCreditsWaterMeasurement?.ReportedValueInAcreFeet ?? 0} ac-ft, {surfaceWaterMeasurementComment}",

            };

            consumedGroundwaterRecords.Add(consumptiveUseRecord);
        }

        await SaveWaterMeasurements(dbContext, geographyID, reportedDate, consumedGroundwaterRecords, consumedGroundwaterMeasurementType.WaterMeasurementTypeID, usageLocationIDs);
    }

    private static async Task SaveWaterMeasurements(QanatDbContext dbContext, int geographyID, DateTime reportedDate, List<WaterMeasurement> newWaterMeasurements, int waterMeasurementTypeID, List<int> usageLocationIDs = null)
    {
        await dbContext.WaterMeasurements.Where(x => x.GeographyID == geographyID && x.ReportedDate == reportedDate && x.WaterMeasurementTypeID == waterMeasurementTypeID && x.FromManualUpload == false && (usageLocationIDs == null || usageLocationIDs.Contains(x.UsageLocationID)))
            .ExecuteDeleteAsync();

        await dbContext.WaterMeasurements.AddRangeAsync(newWaterMeasurements);
        await dbContext.SaveChangesAsync();
    }
}