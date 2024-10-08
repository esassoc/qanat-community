using Microsoft.EntityFrameworkCore;
using Qanat.Common.Util;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public class WaterMeasurementCalculationsLegacyFlow
{
    private const decimal MIUGSASurfaceWaterEfficiencyFactor = 0.8m;
    private const decimal MIUGSAGroundwaterEfficiencyFactor = 0.8m;

    private static readonly List<string> MIUGSASurfaceWaterMeasurementTypeNames = new() { "MID Surface Water Consumption", "Other Surface Water Consumption" };

    private static readonly Dictionary<string, string> MIUGSASurfaceWaterConsumptionMeasurementTypeDictionary = new()
    {
        { "MID Surface Water Delivery", "MID Surface Water Consumption" },
        { "Other Surface Water Delivery", "Other Surface Water Consumption" }
    };

    private const string EffectivePrecipWaterMeasurementTypeName = "Effective Precip";
    private const string ConsumedGroundwaterWithCreditWaterMeasurementTypeName = "Consumed Groundwater with Credit";
    private const string PrecipCreditWaterMeasurementTypeName = "Precipitation Credit";
    private const string ExtractedGroundwaterWaterMeasurementTypeName = "Extracted Groundwater";

    public const string OpenETEvapoWaterMeasurementTypeName = "OpenET Evapotranspiration";
    public const string OpenETPrecipWaterMeasurementTypeName = "OpenET Precipitation";
    public const string OpenETConsumptiveUseWaterMeasurementTypeName = "OpenET Consumptive Use";


    public static async Task RunAllForGeography(QanatDbContext dbContext, int geographyID, DateTime effectiveDate)
    {
        var waterMeasurementTypes = dbContext.WaterMeasurementTypes.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToList();


        switch (geographyID)
        {
            // MIUGSA
            case (1):
                var miugsaPrecipMeasurementType = waterMeasurementTypes
                    .Single(x => x.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.Precip.WaterMeasurementCategoryTypeID);

                await CalculateEffectivePrecip(dbContext, geographyID, miugsaPrecipMeasurementType, effectiveDate);

                var surfaceWaterMeasurementTypes = waterMeasurementTypes
                    .Where(x => x.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.SurfaceWater.WaterMeasurementCategoryTypeID)
                    .ToList();

                foreach (var surfaceWaterMeasurementType in surfaceWaterMeasurementTypes)
                {
                    var surfaceWaterConsumptionMeasurementTypeName = MIUGSASurfaceWaterConsumptionMeasurementTypeDictionary[surfaceWaterMeasurementType.WaterMeasurementTypeName];
                    await CalculateSurfaceWaterConsumption(dbContext, geographyID, surfaceWaterMeasurementType.WaterMeasurementTypeID, surfaceWaterConsumptionMeasurementTypeName, effectiveDate);
                }

                await CalculateConsumedGroundwaterAndRelatedWaterMeasurementsByGeographyID(dbContext, geographyID, effectiveDate);

                break;

            // Demo, MSGSA
            case (5):
            case (6):
                await CalculateOpenETConsumptiveUse(dbContext, geographyID, effectiveDate);

                break;

            // ETSGSA
            case (7):
                var etsgsaPrecipMeasurementType = waterMeasurementTypes
                    .Single(x => x.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.Precip.WaterMeasurementCategoryTypeID);

                await CalculateEffectivePrecip(dbContext, geographyID, etsgsaPrecipMeasurementType, effectiveDate);
                await CalculateConsumedGroundwaterAndRelatedWaterMeasurementsByGeographyID(dbContext, geographyID, effectiveDate);

                break;

            default:
                // leaving this blank intentionally, not all geographies have calculations to run
                break;
        }
    }

    public static async Task RunByMeasurementTypeForGeography(QanatDbContext dbContext, int geographyID, int waterMeasurementTypeID, DateTime effectiveDate)
    {
        var waterMeasurementType = dbContext.WaterMeasurementTypes.AsNoTracking()
            .SingleOrDefault(x => x.WaterMeasurementTypeID == waterMeasurementTypeID && x.GeographyID == geographyID);

        switch (geographyID, waterMeasurementType?.WaterMeasurementCategoryTypeID)
        {
            // MIUGSA
            case (1, (int)WaterMeasurementCategoryTypeEnum.Precip):
                await CalculateEffectivePrecip(dbContext, geographyID, waterMeasurementType, effectiveDate);
                await CalculateConsumedGroundwaterAndRelatedWaterMeasurementsByGeographyID(dbContext, geographyID, effectiveDate);

                break;

            case (1, (int)WaterMeasurementCategoryTypeEnum.ET):
                await CalculateConsumedGroundwaterAndRelatedWaterMeasurementsByGeographyID(dbContext, geographyID, effectiveDate);

                break;

            case (1, (int)WaterMeasurementCategoryTypeEnum.SurfaceWater):
                var surfaceWaterConsumptionMeasurementTypeName = MIUGSASurfaceWaterConsumptionMeasurementTypeDictionary.ContainsKey(waterMeasurementType.WaterMeasurementTypeName)
                    ? MIUGSASurfaceWaterConsumptionMeasurementTypeDictionary[waterMeasurementType.WaterMeasurementTypeName]
                    : throw new ArgumentOutOfRangeException("WaterMeasurementType", "Surface Water Consumption cannot be calculated because the required Water Measurement Type is not specified.");

                await CalculateSurfaceWaterConsumption(dbContext, geographyID, waterMeasurementTypeID, surfaceWaterConsumptionMeasurementTypeName, effectiveDate);
                await CalculateConsumedGroundwaterAndRelatedWaterMeasurementsByGeographyID(dbContext, geographyID, effectiveDate);

                break;

            // ETSGSA
            case (7, (int)WaterMeasurementCategoryTypeEnum.Precip):
                await CalculateEffectivePrecip(dbContext, geographyID, waterMeasurementType, effectiveDate);
                await CalculateConsumedGroundwaterAndRelatedWaterMeasurementsByGeographyID(dbContext, geographyID, effectiveDate);

                break;

            case (7, (int)WaterMeasurementCategoryTypeEnum.ET):
            case (7, (int)WaterMeasurementCategoryTypeEnum.SurfaceWater):
                await CalculateConsumedGroundwaterAndRelatedWaterMeasurementsByGeographyID(dbContext, geographyID, effectiveDate);

                break;

            // Demo & MSGSA
            case (5, (int)WaterMeasurementCategoryTypeEnum.ET):
            case (5, (int)WaterMeasurementCategoryTypeEnum.Precip):
            case (6, (int)WaterMeasurementCategoryTypeEnum.ET):
            case (6, (int)WaterMeasurementCategoryTypeEnum.Precip):
                await CalculateOpenETConsumptiveUse(dbContext, geographyID, effectiveDate);

                break;

            default:
                // leaving this blank intentionally, not all geographies have calculations to run
                break;
        }
    }

    private static async Task CalculateConsumedGroundwaterAndRelatedWaterMeasurementsByGeographyID(QanatDbContext dbContext, int geographyID, DateTime effectiveDate)
    {
        var surfaceWaterMeasurementTypeNames = GetSurfaceWaterMeasurementTypeNamesByGeographyID(dbContext, geographyID);

        switch (geographyID)
        {
            // MIUGSA
            case 1:
                await ETMinusPrecipMinusTotalSurfaceWater(dbContext, geographyID, effectiveDate, ConsumedGroundwaterWithCreditWaterMeasurementTypeName, EffectivePrecipWaterMeasurementTypeName, surfaceWaterMeasurementTypeNames);
                await CalculatePositiveConsumedGroundwaterWithPrecipitationCreditOffset(dbContext, geographyID, ConsumedGroundwaterWithCreditWaterMeasurementTypeName, "Consumed Groundwater", effectiveDate);
                await CalculateExtractedGroundwater(dbContext, geographyID, ConsumedGroundwaterWithCreditWaterMeasurementTypeName, effectiveDate);

                break;

            // ETSGSA
            case 7:
                await ETMinusPrecipMinusTotalSurfaceWater(dbContext, geographyID, effectiveDate, "Consumed Groundwater", EffectivePrecipWaterMeasurementTypeName, surfaceWaterMeasurementTypeNames);

                break;
        }
    }

    private static List<string> GetSurfaceWaterMeasurementTypeNamesByGeographyID(QanatDbContext dbContext, int geographyID)
    {
        return geographyID switch
        {
            1 => MIUGSASurfaceWaterMeasurementTypeNames,
            _ => dbContext.WaterMeasurementTypes.AsNoTracking()
                .Where(x => x.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.SurfaceWater.WaterMeasurementCategoryTypeID)
                .Select(x => x.WaterMeasurementTypeName)
                .ToList()
        };
    }

    /// <summary>
    /// Calculates Effective Precip as precip * precipitation effiency factor by allocation zone
    /// </summary>
    private static async Task CalculateEffectivePrecip(QanatDbContext dbContext, int geographyID, WaterMeasurementType precipWaterMeasurementType, DateTime reportedDate)
    {
        var effectivePrecipWaterMeasurementType = dbContext.WaterMeasurementTypes.AsNoTracking()
            .SingleOrDefault(x => x.GeographyID == geographyID && x.WaterMeasurementTypeName == EffectivePrecipWaterMeasurementTypeName
                                                               && !x.IsUserEditable);

        if (effectivePrecipWaterMeasurementType == null)
        {
            // todo: use AlertMessageDto to throw error instead of exception
            throw new Exception($"Attempting to calculate {EffectivePrecipWaterMeasurementTypeName} for a geography that does not have the required Water Measurement Type configured.");
        }

        var geographyAllocationPlanConfiguration = dbContext.GeographyAllocationPlanConfigurations.AsNoTracking()
            .SingleOrDefault(x => x.GeographyID == geographyID);

        if (geographyAllocationPlanConfiguration == null)
        {
            // todo: use AlertMessageDto to throw error instead of exception
            throw new Exception("Could not calculate Effective Precip because this geography does not have the required Allocation Zone Group configured.");
        }

        var zoneIDByParcelIDs = dbContext.ParcelZones.AsNoTracking()
            .Include(x => x.Zone)
            .Where(x => x.Zone.ZoneGroupID == geographyAllocationPlanConfiguration.ZoneGroupID)
            .ToDictionary(x => x.ParcelID, x => x.ZoneID);

        var zoneIDsByUsageEntityNames = dbContext.UsageEntities.AsNoTracking()
            .Include(x => x.Parcel)
            .Where(x => x.GeographyID == geographyID)
            .AsEnumerable()
            .Where(x => zoneIDByParcelIDs.ContainsKey(x.ParcelID))
            .ToLookup(x => x.UsageEntityName, x => zoneIDByParcelIDs[x.ParcelID]);

        var precipWaterMeasurements = dbContext.WaterMeasurements.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == precipWaterMeasurementType.WaterMeasurementTypeID &&
                        x.ReportedDate.Date == reportedDate.Date)
            .ToList();

        var precipMultiplierByZoneID = dbContext.Zones.AsNoTracking()
            .Where(x => x.ZoneGroupID == geographyAllocationPlanConfiguration.ZoneGroupID)
            .ToDictionary(x => x.ZoneID, x => x.PrecipMultiplier);

        //var usageEntitiesWithoutZoneAssignements = precipWaterMeasurements
        //    .Where(x => !zoneIDsByUsageEntityNames[x.UsageEntityName].Any()).ToList();

        //if (usageEntitiesWithoutZoneAssignements.Any())
        //{
        //    // todo: throw warning
        //}

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
                WaterMeasurementTypeID = effectivePrecipWaterMeasurementType.WaterMeasurementTypeID,
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

        await SaveToWaterMeasurement(dbContext, geographyID, reportedDate, effectivePrecipWaterMeasurements, effectivePrecipWaterMeasurementType.WaterMeasurementTypeID);
    }

    /// <summary>
    /// Calculates Consumptive use as OpenETEvapotranspiration - OpenETPrecipitation
    /// </summary>
    private static async Task CalculateSurfaceWaterConsumption(QanatDbContext dbContext,
                                                               int geographyID,
                                                               int surfaceWaterMeasurementTypeID,
                                                               string consumedSurfaceWaterMeasurementTypeName,
                                                               DateTime reportedDate)
    {
        var consumedSurfaceWaterMeasurementType = dbContext.WaterMeasurementTypes.AsNoTracking()
            .SingleOrDefault(x => x.GeographyID == geographyID && x.WaterMeasurementTypeName == consumedSurfaceWaterMeasurementTypeName);

        if (consumedSurfaceWaterMeasurementType == null)
        {
            throw new Exception($"Attempting to calculate {consumedSurfaceWaterMeasurementTypeName} for a geography that does not have the required Water Measurement Type configured.");
        }

        var newWaterMeasurements = dbContext.WaterMeasurements.AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == surfaceWaterMeasurementTypeID && x.ReportedDate.Date == reportedDate.Date)
            .Select(x => new WaterMeasurement()
            {
                WaterMeasurementTypeID = consumedSurfaceWaterMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UnitTypeID = x.UnitTypeID,
                UsageEntityName = x.UsageEntityName,
                ReportedDate = reportedDate,
                ReportedValue = x.ReportedValue * MIUGSASurfaceWaterEfficiencyFactor,
                ReportedValueInAcreFeet = x.ReportedValueInAcreFeet * MIUGSASurfaceWaterEfficiencyFactor,
                UsageEntityArea = x.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = false,
                Comment = $"{consumedSurfaceWaterMeasurementTypeName} Value: {x.ReportedValueInAcreFeet} ac-ft, Surface Water Efficiency Factor: {MIUGSASurfaceWaterEfficiencyFactor}"
            })
            .ToList();

        await SaveToWaterMeasurement(dbContext, geographyID, reportedDate, newWaterMeasurements, consumedSurfaceWaterMeasurementType.WaterMeasurementTypeID);
    }

    /// <summary>
    /// Calculates Consumed Groundwater as evapotranspiration - precipitation - summed surface water
    /// </summary>
    private static async Task ETMinusPrecipMinusTotalSurfaceWater(QanatDbContext dbContext, int geographyID, DateTime reportedDate, string waterMeasurementTypeNameToSaveAs, string precipWaterMeasurementTypeName, List<string> surfaceWaterTypeNames)
    {
        var waterMeasurementTypeToSaveAs = dbContext.WaterMeasurementTypes
            .AsNoTracking()
            .SingleOrDefault(x => x.GeographyID == geographyID && x.WaterMeasurementTypeName == waterMeasurementTypeNameToSaveAs && !x.IsUserEditable);

        if (waterMeasurementTypeToSaveAs == null)
        {
            throw new Exception($"Attempting to calculate {waterMeasurementTypeNameToSaveAs} for a geography that does not have the required Water Measurement Type configured.");
        }

        var waterMeasurements = dbContext.WaterMeasurements.Include(x => x.WaterMeasurementType)
            .AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == reportedDate.Date)
            .ToList();

        var evapotranspirationWaterMeasurements = waterMeasurements
            .Where(x => x.WaterMeasurementType.WaterMeasurementCategoryType?.ToEnum == WaterMeasurementCategoryTypeEnum.ET)
            .ToList();

        var precipitationWaterMeasurements = waterMeasurements
            .Where(x => x.WaterMeasurementType.WaterMeasurementTypeName == precipWaterMeasurementTypeName)
            .DistinctBy(x => new { x.UsageEntityName, x.UsageEntityArea, x.ReportedValueInAcreFeet }) // todo: verify that these duplicated records are actually coming from OpenET
            .ToDictionary(x => new { x.UsageEntityName, x.UsageEntityArea });

        var surfaceWaterMeasurements = waterMeasurements
            .Where(x => surfaceWaterTypeNames.Contains(x.WaterMeasurementType.WaterMeasurementTypeName))
            .ToLookup(x => x.UsageEntityName);

        var newWaterMeasurements = new List<WaterMeasurement>();
        foreach (var evapoWaterMeasurement in evapotranspirationWaterMeasurements)
        {
            if (!precipitationWaterMeasurements.ContainsKey(new { evapoWaterMeasurement.UsageEntityName, evapoWaterMeasurement.UsageEntityArea })) continue;

            var precipWaterMeasurement = precipitationWaterMeasurements[new { evapoWaterMeasurement.UsageEntityName, evapoWaterMeasurement.UsageEntityArea }];
            var surfaceWaterMeasurementSumInAcreFeet = surfaceWaterMeasurements[evapoWaterMeasurement.UsageEntityName].Any()
                ? surfaceWaterMeasurements[evapoWaterMeasurement.UsageEntityName].Sum(x => x.ReportedValueInAcreFeet)
                : 0;

            var reportedValueInAcreFeet = evapoWaterMeasurement.ReportedValueInAcreFeet - precipWaterMeasurement.ReportedValueInAcreFeet - surfaceWaterMeasurementSumInAcreFeet;

            newWaterMeasurements.Add(new WaterMeasurement()
            {
                WaterMeasurementTypeID = waterMeasurementTypeToSaveAs.WaterMeasurementTypeID,
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
                Comment = $"{evapoWaterMeasurement.WaterMeasurementType.WaterMeasurementTypeName} Value: {evapoWaterMeasurement.ReportedValueInAcreFeet} ac-ft, {precipWaterMeasurementTypeName} Value: {precipWaterMeasurement.ReportedValueInAcreFeet} ac-ft, Total Surface Water Value: {surfaceWaterMeasurementSumInAcreFeet} ac-ft"
            });
        }

        await SaveToWaterMeasurement(dbContext, geographyID, reportedDate, newWaterMeasurements, waterMeasurementTypeToSaveAs.WaterMeasurementTypeID);
    }

    /// <summary>
    /// Calculates positive consumed groundwater values with a precipitation credit record to offset negative values
    /// </summary>
    private static async Task CalculatePositiveConsumedGroundwaterWithPrecipitationCreditOffset(QanatDbContext dbContext,
                                                                                                int geographyID,
                                                                                                string consumedGroundwaterMeasurementType,
                                                                                                string positiveConsumedGroundwaterMeasurementTypeName,
                                                                                                DateTime reportedDate)
    {
        var waterMeasurementTypes = dbContext.WaterMeasurementTypes.AsNoTracking()
            .Where(x => x.GeographyID == geographyID)
            .ToList();

        var positiveConsumedGroundwaterMeasurementType = waterMeasurementTypes
            .SingleOrDefault(x => x.GeographyID == geographyID && x.WaterMeasurementTypeName == positiveConsumedGroundwaterMeasurementTypeName && !x.IsUserEditable);

        if (positiveConsumedGroundwaterMeasurementType == null)
        {
            throw new Exception($"Attempting to calculate {positiveConsumedGroundwaterMeasurementTypeName} for a geography that does not have the required Water Measurement Type configured.");
        }

        var precipitationCreditGroundwaterMeasurementType = waterMeasurementTypes
            .SingleOrDefault(x => x.GeographyID == geographyID && x.WaterMeasurementTypeName == PrecipCreditWaterMeasurementTypeName && !x.IsUserEditable);

        if (precipitationCreditGroundwaterMeasurementType == null)
        {
            throw new Exception($"Attempting to calculate {PrecipCreditWaterMeasurementTypeName} for a geography that does not have the required Water Measurement Type configured.");
        }

        var rawConsumedGroundwaterMeasurementType = waterMeasurementTypes.SingleOrDefault(x => x.WaterMeasurementTypeName == consumedGroundwaterMeasurementType);
        var rawConsumedGroundwaterMeasurements = dbContext.WaterMeasurements.AsNoTracking()
            .Where(x => x.WaterMeasurementTypeID == rawConsumedGroundwaterMeasurementType.WaterMeasurementTypeID && x.ReportedDate.Date == reportedDate)
            .ToList();

        var newPositiveConsumedGroundwaterMeasurements = new List<WaterMeasurement>();
        var newPrecipitationCreditMeasurements = new List<WaterMeasurement>();
        foreach (var waterMeasurement in rawConsumedGroundwaterMeasurements)
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

            // precipitation credit offset
            newPrecipitationCreditMeasurements.Add(new WaterMeasurement()
            {
                WaterMeasurementTypeID = precipitationCreditGroundwaterMeasurementType.WaterMeasurementTypeID,
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

        await SaveToWaterMeasurement(dbContext, geographyID, reportedDate, newPositiveConsumedGroundwaterMeasurements, positiveConsumedGroundwaterMeasurementType.WaterMeasurementTypeID);
        await SaveToWaterMeasurement(dbContext, geographyID, reportedDate, newPrecipitationCreditMeasurements, precipitationCreditGroundwaterMeasurementType.WaterMeasurementTypeID);
    }

    /// <summary>
    /// Calculates Extracted Groundwater as consumed groundwater / groundwater efficiency factor + adjustment
    /// </summary>
    private static async Task CalculateExtractedGroundwater(QanatDbContext dbContext, int geographyID, string consumedGroundwaterMeasurementTypeName, DateTime reportedDate)
    {
        var extractedGroundwaterMeasurementType = dbContext.WaterMeasurementTypes.AsNoTracking()
            .SingleOrDefault(x => x.GeographyID == geographyID && x.WaterMeasurementTypeName == ExtractedGroundwaterWaterMeasurementTypeName);

        if (extractedGroundwaterMeasurementType == null)
        {
            throw new Exception($"Attempting to calculate {ExtractedGroundwaterWaterMeasurementTypeName} for a geography that does not have the required Water Measurement Type configured.");
        }

        var consumedGroundWaterMeasurements = dbContext.WaterMeasurements.Include(x => x.WaterMeasurementType)
            .AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementType.WaterMeasurementTypeName == consumedGroundwaterMeasurementTypeName && x.ReportedDate.Date == reportedDate.Date)
            .ToList();

        var extractedGroundwaterAdjustments = dbContext.WaterMeasurements.Include(x => x.WaterMeasurementType)
            .AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.WaterMeasurementType.WaterMeasurementTypeName == "Extracted Groundwater Adjustment" && x.ReportedDate.Date == reportedDate.Date) //TODO: remove hardcoded string
            .ToList();

        var newWaterMeasurements = new List<WaterMeasurement>();
        foreach (var consumedGroundWaterMeasurement in consumedGroundWaterMeasurements)
        {
            var extractedGroundwaterAdjustment = extractedGroundwaterAdjustments.SingleOrDefault(x => x.GeographyID == consumedGroundWaterMeasurement.GeographyID && x.ReportedDate.Date == reportedDate.Date && x.UsageEntityName == consumedGroundWaterMeasurement.UsageEntityName && x.UsageEntityArea == consumedGroundWaterMeasurement.UsageEntityArea);

            var adjustmentValue = extractedGroundwaterAdjustment?.ReportedValue ?? 0;
            var adjustmentValueInAcreFeet = extractedGroundwaterAdjustment?.ReportedValueInAcreFeet ?? 0;

            var newExtractedGroundwaterMeasurement = new WaterMeasurement()
            {
                WaterMeasurementTypeID = extractedGroundwaterMeasurementType.WaterMeasurementTypeID,
                GeographyID = geographyID,
                UnitTypeID = consumedGroundWaterMeasurement.UnitTypeID,
                UsageEntityName = consumedGroundWaterMeasurement.UsageEntityName,
                ReportedDate = reportedDate,
                ReportedValue = (consumedGroundWaterMeasurement.ReportedValue / MIUGSAGroundwaterEfficiencyFactor) + adjustmentValue,
                ReportedValueInAcreFeet = (consumedGroundWaterMeasurement.ReportedValueInAcreFeet / MIUGSAGroundwaterEfficiencyFactor) + adjustmentValueInAcreFeet,
                UsageEntityArea = consumedGroundWaterMeasurement.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                FromManualUpload = false,
                Comment = $"{consumedGroundwaterMeasurementTypeName} Value: {consumedGroundWaterMeasurement.ReportedValue} {consumedGroundWaterMeasurement.UnitType?.UnitTypeAbbreviation}, Groundwater Efficiency Factor: {MIUGSAGroundwaterEfficiencyFactor}, Adjustment={adjustmentValue}"
            };

            newWaterMeasurements.Add(newExtractedGroundwaterMeasurement);
        }

        await SaveToWaterMeasurement(dbContext, geographyID, reportedDate, newWaterMeasurements, extractedGroundwaterMeasurementType.WaterMeasurementTypeID);
    }

    /// <summary>
    /// Calculates Consumptive Use as OpenETEvapotranspiration - OpenETPrecipitation
    /// </summary>
    private static async Task CalculateOpenETConsumptiveUse(QanatDbContext dbContext, int geographyID, DateTime reportedDate)
    {
        var openETConsumptiveUseWaterMeasurementType = dbContext.WaterMeasurementTypes.AsNoTracking()
            .SingleOrDefault(x => x.GeographyID == geographyID && x.WaterMeasurementTypeName == OpenETConsumptiveUseWaterMeasurementTypeName && x.IsActive);

        if (openETConsumptiveUseWaterMeasurementType == null)
        {
            throw new Exception($"Attempting to calculate {OpenETConsumptiveUseWaterMeasurementTypeName} for a geography that does not have the required Water Measurement Type configured or activated.");
        }

        var waterMeasurements = dbContext.WaterMeasurements.Include(x => x.WaterMeasurementType)
            .AsNoTracking()
            .Where(x => x.GeographyID == geographyID && x.ReportedDate == reportedDate && x.FromManualUpload == false)
            .ToList();

        var evapotranspirationWaterMeasurements = waterMeasurements
            .Where(x => x.WaterMeasurementType.WaterMeasurementTypeName == OpenETEvapoWaterMeasurementTypeName)
            .ToList();

        var precipitationWaterMeasurements = waterMeasurements
            .Where(x => x.WaterMeasurementType.WaterMeasurementTypeName == OpenETPrecipWaterMeasurementTypeName)
            .ToList();

        if (!evapotranspirationWaterMeasurements.Any() || !precipitationWaterMeasurements.Any()) return;

        var consumptiveUseRecords = new List<WaterMeasurement>();

        // calculating consumptive use for parcels that have both evapo and precip water measurement values for the given effective date
        // handling multipolygons (i.e. multiple records with the same parcel number) by matching records on ParcelNumber and ParcelArea (column received from OpenET)
        // NOTE: will break if a parcel contains multiple polygons with exactly the same area
        foreach (var evapotranspirationWaterMeasurement in evapotranspirationWaterMeasurements)
        {
            var precipitationWaterMeasurement = precipitationWaterMeasurements
                .SingleOrDefault(x => x.UsageEntityName == evapotranspirationWaterMeasurement.UsageEntityName && x.UsageEntityArea == evapotranspirationWaterMeasurement.UsageEntityArea);

            if (precipitationWaterMeasurement == null) continue;

            var reportedValue = evapotranspirationWaterMeasurement.ReportedValue - precipitationWaterMeasurement.ReportedValue;
            var reportedValueInAcreFeet = (evapotranspirationWaterMeasurement.ReportedValueInAcreFeet ?? 0) - (precipitationWaterMeasurement.ReportedValueInAcreFeet ?? 0);

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
                Comment = $"{OpenETEvapoWaterMeasurementTypeName} Value: {evapotranspirationWaterMeasurement.ReportedValueInAcreFeet} ac-ft, {OpenETPrecipWaterMeasurementTypeName} Value: {precipitationWaterMeasurement.ReportedValueInAcreFeet} ac-ft",

                // months with heavy rain can lead to negative Consumptive Use calculations, so ensuring Reported Values cannot be < 0
                ReportedValue = reportedValue > 0 ? reportedValue : 0,
                ReportedValueInAcreFeet = reportedValueInAcreFeet > 0 ? reportedValueInAcreFeet : 0,
            };

            consumptiveUseRecords.Add(consumptiveUseRecord);
        }

        await SaveToWaterMeasurement(dbContext, geographyID, reportedDate, consumptiveUseRecords, openETConsumptiveUseWaterMeasurementType.WaterMeasurementTypeID);
    }

    private static async Task SaveToWaterMeasurement(QanatDbContext dbContext, int geographyID, DateTime reportedDate, List<WaterMeasurement> newWaterMeasurements, int waterMeasurementTypeID)
    {
        await dbContext.WaterMeasurements.Where(x =>
                                                    x.GeographyID == geographyID && x.ReportedDate == reportedDate &&
                                                    x.WaterMeasurementTypeID == waterMeasurementTypeID && x.FromManualUpload == false)
            .ExecuteDeleteAsync();

        await dbContext.WaterMeasurements.AddRangeAsync(newWaterMeasurements);
        await dbContext.SaveChangesAsync();
    }
}