using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Qanat.Common.Util;
using Qanat.EFModels.Entities;

namespace Qanat.Tests.IntegrationTests.WaterMeasurementCalculationsTests;

[TestClass]
public class WaterMeasurementCalculationTests
{
    private static QanatDbContext _dbContext;

    //MK 8/23/2024 -- I think we need to be rounding all values (including acres) to 4 decimal places on WaterMeasurement save. I am going to apply a tolerance for now, but we should add in a story to remove the rounding and tolerance check from these tests.
    private readonly decimal _acceptableTolerance = .0001m;

    [TestInitialize]
    public void TestInitialize()
    {
        var dbCS = AssemblySteps.Configuration["sqlConnectionString"];
        _dbContext = new QanatDbContext(dbCS);
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        await _dbContext.DisposeAsync();
    }

    #region MIUGSA Specific Tests

    [DataTestMethod]
    [DataRow("20230731", "067-070-033", 1000.0000d)]
    [Description("MIUGSA - After updating the ET Precip it should reflect in all the dependant calculations.")]
    public async Task MIUGSA_CanUpdateETPrecipAndHaveDependantCalculationsModified(string dateAsString, string usageEntityName, double updatedAmountInAcreFeetAsDouble)
    {
        #region Hard Coded Assumptions

        var geographyID = 1; //1 == MIGUISA
        var openETPrecipMeasurementTypeID = 9; //9==ETPrecip
        var effectivePrecipMeasurementTypeID = 35; //35==EffectivePrecip
        var precipitationCreditMeasurementTypeID = 38; //38==PrecipitationCredit
        var consumedGroundwaterMeasurementTypeID = 39; //39==ConsumedGroundwater
        var consumedGroundwaterWithCreditMeasurementTypeID = 40; //40==ConsumedGroundwaterWithCredit
        var unadjustedGroundwaterMeasurementTypeID = 42; //42==UnadjustedExtractedGroundwater
        var extractedGroundwaterAdjustmentMeasurementTypeID = 41; //41==ExtractedGroundwaterAdjustment
        var extractedGroundwaterMeasurementTypeID = 15; //15==ExtractedGroundwater
        var extractedAgainstSupplyMeasurementTypeID = 43; //43==ExtractedAgainstSupply

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var usageEntity = await _dbContext.UsageEntities.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageEntityName == usageEntityName);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var updatedAmountInInches = Math.Round(updatedAmountInAcreFeetAsDecimal / (decimal)usageEntity.UsageEntityArea, 4, MidpointRounding.ToEven);

        #endregion

        #region Previous Values

        var etPrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == openETPrecipMeasurementTypeID && x.UsageEntityName == usageEntityName);
        var previousETPrecipValueInAcreFeet = etPrecipMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousETPrecipValueInInches = etPrecipMeasurement?.ReportedValue ?? 0;

        var previousEffectivePrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID && x.UsageEntityName == usageEntityName);
        var previousEffectivePrecipValueInAcreFeet = previousEffectivePrecipMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousEffectivePrecipValueInInches = previousEffectivePrecipMeasurement?.ReportedValue ?? 0;

        var previousPrecipitationCreditMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == precipitationCreditMeasurementTypeID && x.UsageEntityName == usageEntityName);
        var previousPrecipitationCreditValueInAcreFeet = previousPrecipitationCreditMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousPrecipitationCreditValueInInches = previousPrecipitationCreditMeasurement?.ReportedValue ?? 0;

        var previousConsumedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterMeasurementTypeID && x.UsageEntityName == usageEntityName);
        var previousConsumedGroundwaterValueInAcreFeet = previousConsumedGroundwaterMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousConsumedGroundwaterValueInInches = previousConsumedGroundwaterMeasurement?.ReportedValue ?? 0;

        var previousConsumedGroundwaterWithCreditMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterWithCreditMeasurementTypeID && x.UsageEntityName == usageEntityName);
        var previousConsumedGroundwaterWithCreditValueInAcreFeet = previousConsumedGroundwaterWithCreditMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousConsumedGroundwaterWithCreditValueInInches = previousConsumedGroundwaterWithCreditMeasurement?.ReportedValue ?? 0;

        var previousUnadjustedExtractedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == unadjustedGroundwaterMeasurementTypeID && x.UsageEntityName == usageEntityName);
        var previousUnadjustedExtractedGroundwaterValueInAcreFeet = previousUnadjustedExtractedGroundwaterMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousUnadjustedExtractedGroundwaterValueInInches = previousUnadjustedExtractedGroundwaterMeasurement?.ReportedValue ?? 0;

        var previousExtractedGroundwaterAdjustmentMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedGroundwaterAdjustmentMeasurementTypeID && x.UsageEntityName == usageEntityName);
        var previousExtractedGroundwaterAdjustmentValueInAcreFeet = previousExtractedGroundwaterAdjustmentMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousExtractedGroundwaterAdjustmentValueInInches = previousExtractedGroundwaterAdjustmentMeasurement?.ReportedValue ?? 0;

        var previousExtractedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedGroundwaterMeasurementTypeID && x.UsageEntityName == usageEntityName);
        var previousExtractedGroundwaterValueInAcreFeet = previousExtractedGroundwaterMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousExtractedGroundwaterValueInInches = previousExtractedGroundwaterMeasurement?.ReportedValue ?? 0;

        #endregion

        #region Upsert OpenET Precip and Run Calculations

        if (etPrecipMeasurement == null)
        {
            etPrecipMeasurement = new WaterMeasurement()
            {
                UsageEntityName = usageEntityName,
                UsageEntityArea = (decimal?)usageEntity.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                ReportedDate = dateToCalculate,
                ReportedValue = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                WaterMeasurementTypeID = openETPrecipMeasurementTypeID,
                UnitTypeID = 1,
                GeographyID = geographyID
            };

            _dbContext.WaterMeasurements.Add(etPrecipMeasurement);
        }
        else
        {
            etPrecipMeasurement.ReportedValue = updatedAmountInInches;
            etPrecipMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeography(_dbContext, geographyID, openETPrecipMeasurementTypeID, dateToCalculate);

        #endregion

        #region Check Precip

        var updatedETPrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == openETPrecipMeasurementTypeID && x.UsageEntityName == usageEntityName);
        Assert.IsNotNull(updatedETPrecipMeasurement);
        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedETPrecipMeasurement.ReportedValueInAcreFeet);
        Assert.AreEqual(updatedAmountInInches, updatedETPrecipMeasurement.ReportedValue);

        #endregion

        #region Check Effective Precip

        var geographyAllocationPlanConfiguration = await _dbContext.GeographyAllocationPlanConfigurations
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID);

        if (geographyAllocationPlanConfiguration == null)
        {
            // todo: use AlertMessageDto to throw error instead of exception
            throw new Exception("Could not calculate Effective Precip because this geography does not have the required Allocation Zone Group configured.");
        }

        var zoneIDByParcelIDs = await _dbContext.ParcelZones.AsNoTracking()
            .Include(x => x.Zone)
            .Where(x => x.Zone.ZoneGroupID == geographyAllocationPlanConfiguration.ZoneGroupID)
            .ToDictionaryAsync(x => x.ParcelID, x => x.ZoneID);

        var zoneIDsByUsageEntityNames = _dbContext.UsageEntities.AsNoTracking()
            .Include(x => x.Parcel)
            .Where(x => x.GeographyID == geographyID)
            .AsEnumerable()
            .Where(x => zoneIDByParcelIDs.ContainsKey(x.ParcelID))
            .ToLookup(x => x.UsageEntityName, x => zoneIDByParcelIDs[x.ParcelID]);

        var precipMultiplierByZoneID = await _dbContext.Zones.AsNoTracking()
            .Where(x => x.ZoneGroupID == geographyAllocationPlanConfiguration.ZoneGroupID)
            .ToDictionaryAsync(x => x.ZoneID, x => x.PrecipMultiplier);

        var zoneID = zoneIDsByUsageEntityNames[usageEntityName].First();
        if (!precipMultiplierByZoneID.ContainsKey(zoneID) || !precipMultiplierByZoneID[zoneID].HasValue)
        {
            throw new Exception("Could not calculate Effective Precip because at least one Allocation Zone does not have a precipitation multiplier configured.");
        }

        var effectivePrecipMultiplier = precipMultiplierByZoneID[zoneID].Value;

        var expectedEffectivePrecipValueInAcreFeet = Math.Round(updatedAmountInAcreFeetAsDecimal * effectivePrecipMultiplier, 4, MidpointRounding.ToEven);
        var expectedEffectivePrecipValueInInches = Math.Round(updatedAmountInInches * effectivePrecipMultiplier, 4, MidpointRounding.ToEven);

        var updatedEffectivePrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID && x.UsageEntityName == usageEntityName);
        Assert.IsNotNull(updatedEffectivePrecipMeasurement);
        Assert.IsTrue(Math.Abs(expectedEffectivePrecipValueInAcreFeet - updatedEffectivePrecipMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0)) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedEffectivePrecipValueInAcreFeet}. Got: {updatedEffectivePrecipMeasurement.ReportedValueInAcreFeet}");
        Assert.IsTrue(Math.Abs(expectedEffectivePrecipValueInInches - updatedEffectivePrecipMeasurement.ReportedValue) <= _acceptableTolerance, $"Reported Value in Inches do not match within tolerance. Expected: {expectedEffectivePrecipValueInInches}. Got: {updatedEffectivePrecipMeasurement.ReportedValue}");

        #endregion

        #region Check ETMinusPrecipMinusTotalSurfaceWater

        var evapotranspirationWaterMeasurement = await _dbContext.WaterMeasurements
            .Where(x => x.WaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.ET.WaterMeasurementCategoryTypeID)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageEntityName == usageEntityName);

        var consumedGroundwaterWithCreditMeasurementType = await _dbContext.WaterMeasurementTypes.AsNoTracking()
            .Include(x => x.WaterMeasurementTypeDependencyWaterMeasurementTypes).ThenInclude(x => x.DependsOnWaterMeasurementType)
            .SingleOrDefaultAsync(x => x.WaterMeasurementTypeID == consumedGroundwaterWithCreditMeasurementTypeID);

        var surfaceWaterMeasurementTypeIDs = consumedGroundwaterWithCreditMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
            .Where(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.SurfaceWater.WaterMeasurementCategoryTypeID || x.DependsOnWaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.CalculateSurfaceWaterConsumption.WaterMeasurementCalculationTypeID)
            .Select(x => x.DependsOnWaterMeasurementType.WaterMeasurementTypeID)
            .ToList();

        var surfaceWaterMeasurements = await _dbContext.WaterMeasurements
            .Include(x => x.WaterMeasurementType)
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageEntityName == usageEntityName && surfaceWaterMeasurementTypeIDs.Contains(x.WaterMeasurementTypeID.GetValueOrDefault(0)))
            .ToListAsync();

        var surfaceWaterTotalInAcreFeet = surfaceWaterMeasurements.Sum(x => x.ReportedValueInAcreFeet);

        var expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet = Math.Round((evapotranspirationWaterMeasurement.ReportedValueInAcreFeet - updatedEffectivePrecipMeasurement.ReportedValueInAcreFeet - surfaceWaterTotalInAcreFeet).GetValueOrDefault(0), 4, MidpointRounding.ToEven);

        var expectedETMinusPrecipMinusTotalSurfaceWaterInInches = Math.Round(UnitConversionHelper.ConvertAcreFeetToInches(expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet, (decimal)previousConsumedGroundwaterMeasurement.UsageEntityArea), 4, MidpointRounding.ToEven);

        var updatedETMinusPrecipMinusTotalSurfaceWater = await _dbContext.WaterMeasurements
            .Include(x => x.WaterMeasurementType)
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageEntityName == usageEntityName && x.WaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.ETMinusPrecipMinusTotalSurfaceWater.WaterMeasurementCalculationTypeID);

        var updatedETMinusPrecipMinusTotalSurfaceWaterInInches = Math.Round(updatedETMinusPrecipMinusTotalSurfaceWater.ReportedValue, 4, MidpointRounding.ToEven);

        var updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet = Math.Round(updatedETMinusPrecipMinusTotalSurfaceWater.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedETMinusPrecipMinusTotalSurfaceWaterInInches - updatedETMinusPrecipMinusTotalSurfaceWaterInInches) <= _acceptableTolerance, $"Reported Value in Inches do not match within tolerance. Expected: {expectedETMinusPrecipMinusTotalSurfaceWaterInInches}. Got: {updatedETMinusPrecipMinusTotalSurfaceWaterInInches}");

        Assert.IsTrue(Math.Abs(expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet - updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet}. Got: {updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet}");

        #endregion

        #region Check Positive Consumed Groundwater and Precipitation Credit

        var updatedPrecipitationCreditMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == precipitationCreditMeasurementTypeID && x.UsageEntityName == usageEntityName);

        var updatedPositiveConsumedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterMeasurementTypeID && x.UsageEntityName == usageEntityName);

        if (updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet < 0)
        {
            Assert.IsNotNull(updatedPrecipitationCreditMeasurement);
            Assert.AreEqual(updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet, updatedPrecipitationCreditMeasurement.ReportedValueInAcreFeet);
            Assert.AreEqual(updatedETMinusPrecipMinusTotalSurfaceWaterInInches, updatedPrecipitationCreditMeasurement.ReportedValue);

            Assert.IsNotNull(updatedPositiveConsumedGroundwaterMeasurement);
            Assert.AreEqual(0, updatedPositiveConsumedGroundwaterMeasurement.ReportedValueInAcreFeet);
        }
        else if (updatedPrecipitationCreditMeasurement != null)
        {
            Assert.AreEqual(0, updatedPrecipitationCreditMeasurement.ReportedValueInAcreFeet);
            Assert.AreEqual(0, updatedPrecipitationCreditMeasurement.ReportedValue);

            Assert.IsNotNull(updatedPositiveConsumedGroundwaterMeasurement);
            Assert.AreEqual(updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet, updatedPositiveConsumedGroundwaterMeasurement.ReportedValueInAcreFeet);
            Assert.AreEqual(updatedETMinusPrecipMinusTotalSurfaceWaterInInches, updatedPositiveConsumedGroundwaterMeasurement.ReportedValue);
        }

        #endregion

        #region Check Extracted Groundwater Adjustment

        var extractedGroundwaterAdjustment = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedGroundwaterAdjustmentMeasurementTypeID && x.UsageEntityName == usageEntityName);


        #endregion

        #region Check Extracted Groundwater

        var extractedGroundwaterMeasurementType = await _dbContext.WaterMeasurementTypes.SingleOrDefaultAsync(x => x.WaterMeasurementTypeID == extractedGroundwaterMeasurementTypeID);
        var parsed = extractedGroundwaterMeasurementType.CalculationJSON.TryParseJObject(out var calculationJSON);
        Assert.IsTrue(parsed);

        var groundwaterEfficiencyFactor = calculationJSON["GroundwaterEfficiencyFactor"]!.Value<decimal>();
        var groundwaterAdjustment = await _dbContext.WaterMeasurements
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.ManualAdjustment.WaterMeasurementCategoryTypeID && x.UsageEntityName == usageEntityName);

        var updatedExtractedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedGroundwaterMeasurementTypeID && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedExtractedGroundwaterMeasurement);

        var expectedExtractedGroundwaterValueInAcreFeet = Math.Round(updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet / groundwaterEfficiencyFactor + (groundwaterAdjustment?.ReportedValueInAcreFeet.GetValueOrDefault(0) ?? 0), 4, MidpointRounding.ToEven);
        var expectedExtractedGroundwaterValueInInches = Math.Round(updatedETMinusPrecipMinusTotalSurfaceWaterInInches / groundwaterEfficiencyFactor + (groundwaterAdjustment?.ReportedValue ?? 0), 4, MidpointRounding.ToEven);

        var updatedExtractedGroundwaterValueInAcreFeet = Math.Round(updatedExtractedGroundwaterMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);
        var updatedExtractedGroundwaterValueInInches = Math.Round(updatedExtractedGroundwaterMeasurement.ReportedValue, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedExtractedGroundwaterValueInAcreFeet - updatedExtractedGroundwaterValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedExtractedGroundwaterValueInAcreFeet}. Got: {updatedExtractedGroundwaterValueInAcreFeet}");
        Assert.IsTrue(Math.Abs(expectedExtractedGroundwaterValueInInches - updatedExtractedGroundwaterValueInInches) <= _acceptableTolerance, $"Reported Value in Inches do not match within tolerance. Expected: {expectedExtractedGroundwaterValueInInches}. Got: {updatedExtractedGroundwaterValueInInches}");

        #endregion

        #region Check Unadjusted Extracted Groundwater

        var updatedUnadjustedExtractedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == unadjustedGroundwaterMeasurementTypeID && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedUnadjustedExtractedGroundwaterMeasurement);

        if (extractedGroundwaterAdjustment != null)
        {
            var expectedUnadjustedExtractedGroundwaterValueInAcreFeet = Math.Round(updatedExtractedGroundwaterValueInAcreFeet - extractedGroundwaterAdjustment.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);
            var expectedUnadjustedExtractedGroundwaterValueInInches = Math.Round(updatedExtractedGroundwaterValueInInches - extractedGroundwaterAdjustment.ReportedValue, 4, MidpointRounding.ToEven);

            var updatedUnadjustedExtractedGroundwaterValueInAcreFeet = Math.Round(updatedUnadjustedExtractedGroundwaterMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);
            var updatedUnadjustedExtractedGroundwaterValueInInches = Math.Round(updatedUnadjustedExtractedGroundwaterMeasurement.ReportedValue, 4, MidpointRounding.ToEven);

            Assert.IsTrue(Math.Abs(expectedUnadjustedExtractedGroundwaterValueInAcreFeet - updatedUnadjustedExtractedGroundwaterValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedUnadjustedExtractedGroundwaterValueInAcreFeet}. Got: {updatedUnadjustedExtractedGroundwaterValueInAcreFeet}");
            Assert.IsTrue(Math.Abs(expectedUnadjustedExtractedGroundwaterValueInInches - updatedUnadjustedExtractedGroundwaterValueInInches) <= _acceptableTolerance, $"Reported Value in Inches do not match within tolerance. Expected: {expectedUnadjustedExtractedGroundwaterValueInInches}. Got: {updatedUnadjustedExtractedGroundwaterValueInInches}");
        }

        #endregion

        #region Check Extracted Against Supply

        var updatedExtractedAgainstSupplyMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedAgainstSupplyMeasurementTypeID && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedExtractedAgainstSupplyMeasurement);

        //MK 8/23/2024 -- The value should be 0 if the cumulative extra water for the Water Account is less than 0. Otherwise it should be the same as the extracted groundwater. This is kind of a lazy check but it should be helpful for now.
        Assert.IsTrue(updatedExtractedAgainstSupplyMeasurement.ReportedValueInAcreFeet == 0 || Math.Round(updatedExtractedAgainstSupplyMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven) == Math.Round(updatedExtractedGroundwaterMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven));

        #endregion
    }


    [DataTestMethod]
    [DataRow("20230731", "067-070-033", 1000.0000d)]
    [Description("MIUGSA - After updating the MID Surface Water Delivery it should reflect in all the dependant calculations.")]
    public async Task MIUGSA_CanUpdateMIDSurfaceWaterDeliveryAndHaveDependantCalculationsModified(string dateAsString, string usageEntityName, double updatedAmountInAcreFeetAsDouble)
    {
        #region Hardcoded Assumptions

        var geographyID = 1; //1 == MIGUISA
        var midSurfaceWaterDeliveryMeasurementTypeID = 23; //23 MID Surface Water Delivery
        var midSurfaceWaterConsumption = 36; //36 MID Surface Water Consumption
        var effectivePrecipMeasurementTypeID = 35; //35==EffectivePrecip
        var consumedGroundwaterWithCreditMeasurementTypeID = 40; //40==ConsumedGroundwaterWithCredit

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var usageEntity = await _dbContext.UsageEntities.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageEntityName == usageEntityName);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var updatedAmountInInches = Math.Round(updatedAmountInAcreFeetAsDecimal / (decimal)usageEntity.UsageEntityArea, 4, MidpointRounding.ToEven);

        #endregion

        #region Previous Values

        var midSurfaceWaterDeliveryMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == midSurfaceWaterDeliveryMeasurementTypeID && x.UsageEntityName == usageEntityName);
        var previousMidSurfaceWaterDeliveryValueInAcreFeet = midSurfaceWaterDeliveryMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousMidSurfaceWaterDeliveryValueInInches = midSurfaceWaterDeliveryMeasurement?.ReportedValue ?? 0;

        var previousMidSurfaceWaterConsumptionMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == midSurfaceWaterConsumption && x.UsageEntityName == usageEntityName);
        var previousMidSurfaceWaterConsumptionValueInAcreFeet = previousMidSurfaceWaterConsumptionMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousMidSurfaceWaterConsumptionValueInInches = previousMidSurfaceWaterConsumptionMeasurement?.ReportedValue ?? 0;

        var previousEffectivePrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID && x.UsageEntityName == usageEntityName);
        var previousEffectivePrecipValueInAcreFeet = previousEffectivePrecipMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousEffectivePrecipValueInInches = previousEffectivePrecipMeasurement?.ReportedValue ?? 0;

        var previousConsumedGroundwaterWithCreditMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterWithCreditMeasurementTypeID && x.UsageEntityName == usageEntityName);
        var previousConsumedGroundwaterWithCreditValueInAcreFeet = previousConsumedGroundwaterWithCreditMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousConsumedGroundwaterWithCreditValueInInches = previousConsumedGroundwaterWithCreditMeasurement?.ReportedValue ?? 0;

        #endregion

        #region Upsert MID Surface Water Delivery and Run Calculations

        if (midSurfaceWaterDeliveryMeasurement == null)
        {
            midSurfaceWaterDeliveryMeasurement = new WaterMeasurement()
            {
                UsageEntityName = usageEntityName,
                UsageEntityArea = (decimal?)usageEntity.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                ReportedDate = dateToCalculate,
                ReportedValue = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                WaterMeasurementTypeID = midSurfaceWaterDeliveryMeasurementTypeID,
                UnitTypeID = 1,
                GeographyID = geographyID
            };

            _dbContext.WaterMeasurements.Add(midSurfaceWaterDeliveryMeasurement);
        }
        else
        {
            midSurfaceWaterDeliveryMeasurement.ReportedValue = updatedAmountInInches;
            midSurfaceWaterDeliveryMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeography(_dbContext, geographyID, midSurfaceWaterDeliveryMeasurementTypeID, dateToCalculate);

        #endregion

        #region Check MID Surface Water Delivery

        var updatedMidSurfaceWaterDeliveryMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == midSurfaceWaterDeliveryMeasurementTypeID && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedMidSurfaceWaterDeliveryMeasurement);
        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedMidSurfaceWaterDeliveryMeasurement.ReportedValueInAcreFeet);
        Assert.AreEqual(updatedAmountInInches, updatedMidSurfaceWaterDeliveryMeasurement.ReportedValue);

        #endregion

        #region Check MID Surface Water Consumption

        var updatedMidSurfaceWaterConsumptionMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == midSurfaceWaterConsumption && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedMidSurfaceWaterConsumptionMeasurement);

        var consumedSurfaceWaterMeasurementType = await _dbContext.WaterMeasurementTypes.SingleOrDefaultAsync(x => x.WaterMeasurementTypeID == midSurfaceWaterConsumption);
        var parsed = consumedSurfaceWaterMeasurementType.CalculationJSON.TryParseJObject(out var calculationJSON);
        if (!parsed || !calculationJSON.ContainsKey("SurfaceWaterEfficiencyFactor") || calculationJSON["SurfaceWaterEfficiencyFactor"] == null)
        {
            throw new Exception("Could not calculate Consumed Surface Water because the Surface Water Efficiency Factor is missing from the CalculationJSON.");
        }

        var surfaceWaterEfficiencyFactor = calculationJSON["SurfaceWaterEfficiencyFactor"]!.Value<decimal>();

        var expectedMidSurfaceWaterConsumptionValueInAcreFeet = Math.Round(updatedAmountInAcreFeetAsDecimal * surfaceWaterEfficiencyFactor, 4, MidpointRounding.ToEven);
        var expectedMidSurfaceWaterConsumptionValueInInches = Math.Round(updatedAmountInInches * surfaceWaterEfficiencyFactor, 4, MidpointRounding.ToEven);

        var updatedMidSurfaceWaterConsumptionValueInAcreFeet = Math.Round(updatedMidSurfaceWaterConsumptionMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);
        var updatedMidSurfaceWaterConsumptionValueInInches = Math.Round(updatedMidSurfaceWaterConsumptionMeasurement.ReportedValue, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedMidSurfaceWaterConsumptionValueInAcreFeet - updatedMidSurfaceWaterConsumptionValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedMidSurfaceWaterConsumptionValueInAcreFeet}. Got: {updatedMidSurfaceWaterConsumptionValueInAcreFeet}");
        Assert.IsTrue(Math.Abs(expectedMidSurfaceWaterConsumptionValueInInches - updatedMidSurfaceWaterConsumptionValueInInches) <= _acceptableTolerance, $"Reported Value in Inches do not match within tolerance. Expected: {expectedMidSurfaceWaterConsumptionValueInInches}. Got: {updatedMidSurfaceWaterConsumptionValueInInches}");

        #endregion

        #region Check ETMinusPrecipMinusTotalSurfaceWater

        var evapotranspirationWaterMeasurement = await _dbContext.WaterMeasurements
            .Where(x => x.WaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.ET.WaterMeasurementCategoryTypeID)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageEntityName == usageEntityName);

        var consumedGroundwaterWithCreditMeasurementType = await _dbContext.WaterMeasurementTypes.AsNoTracking()
            .Include(x => x.WaterMeasurementTypeDependencyWaterMeasurementTypes).ThenInclude(x => x.DependsOnWaterMeasurementType)
            .SingleOrDefaultAsync(x => x.WaterMeasurementTypeID == consumedGroundwaterWithCreditMeasurementTypeID);

        var surfaceWaterMeasurementTypeIDs = consumedGroundwaterWithCreditMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
            .Where(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.SurfaceWater.WaterMeasurementCategoryTypeID || x.DependsOnWaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.CalculateSurfaceWaterConsumption.WaterMeasurementCalculationTypeID)
            .Select(x => x.DependsOnWaterMeasurementType.WaterMeasurementTypeID)
            .ToList();

        var surfaceWaterMeasurements = await _dbContext.WaterMeasurements
            .Include(x => x.WaterMeasurementType)
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageEntityName == usageEntityName && surfaceWaterMeasurementTypeIDs.Contains(x.WaterMeasurementTypeID.GetValueOrDefault(0)))
            .ToListAsync();

        var surfaceWaterTotalInAcreFeet = surfaceWaterMeasurements.Sum(x => x.ReportedValueInAcreFeet);

        var expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet = Math.Round((evapotranspirationWaterMeasurement.ReportedValueInAcreFeet - previousEffectivePrecipMeasurement.ReportedValueInAcreFeet - surfaceWaterTotalInAcreFeet).GetValueOrDefault(0), 4, MidpointRounding.ToEven);

        var expectedETMinusPrecipMinusTotalSurfaceWaterInInches = Math.Round(UnitConversionHelper.ConvertAcreFeetToInches(expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet, (decimal)previousConsumedGroundwaterWithCreditMeasurement.UsageEntityArea), 4, MidpointRounding.ToEven);

        var updatedETMinusPrecipMinusTotalSurfaceWater = await _dbContext.WaterMeasurements
            .Include(x => x.WaterMeasurementType)
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageEntityName == usageEntityName && x.WaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.ETMinusPrecipMinusTotalSurfaceWater.WaterMeasurementCalculationTypeID);

        var updatedETMinusPrecipMinusTotalSurfaceWaterInInches = Math.Round(updatedETMinusPrecipMinusTotalSurfaceWater.ReportedValue, 4, MidpointRounding.ToEven);

        var updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet = Math.Round(updatedETMinusPrecipMinusTotalSurfaceWater.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedETMinusPrecipMinusTotalSurfaceWaterInInches - updatedETMinusPrecipMinusTotalSurfaceWaterInInches) <= _acceptableTolerance, $"Reported Value in Inches do not match within tolerance. Expected: {expectedETMinusPrecipMinusTotalSurfaceWaterInInches}. Got: {updatedETMinusPrecipMinusTotalSurfaceWaterInInches}");

        Assert.IsTrue(Math.Abs(expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet - updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet}. Got: {updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet}");

        #endregion
    }

    [DataTestMethod]
    [DataRow("20230731", "067-070-033", 1000.0000d)]
    [Description("MIUGSA - After updating the Other Surface Water Delivery it should reflect in all the dependant calculations.")]
    public async Task MIUGSA_CanUpdateOtherSurfaceWaterDeliveryAndHaveDependantCalculationsModified(string dateAsString, string usageEntityName, double updatedAmountInAcreFeetAsDouble)
    {
        #region Hardcoded Assumptions

        var geographyID = 1; //1 == MIGUISA
        var otherSurfaceWaterDeliveryMeasurementTypeID = 24; //24 Other Surface Water Delivery
        var otherSurfaceWaterConsumption = 37; //37 Other Surface Water Consumption
        var effectivePrecipMeasurementTypeID = 35; //35==EffectivePrecip
        var consumedGroundwaterWithCreditMeasurementTypeID = 40; //40==ConsumedGroundwaterWithCredit

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var usageEntity = await _dbContext.UsageEntities.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageEntityName == usageEntityName);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var updatedAmountInInches = Math.Round(updatedAmountInAcreFeetAsDecimal / (decimal)usageEntity.UsageEntityArea, 4, MidpointRounding.ToEven);

        #endregion

        #region Previous Values

        var otherSurfaceWaterDeliveryMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == otherSurfaceWaterDeliveryMeasurementTypeID && x.UsageEntityName == usageEntityName);
        var previousOtherSurfaceWaterDeliveryValueInAcreFeet = otherSurfaceWaterDeliveryMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousOtherSurfaceWaterDeliveryValueInInches = otherSurfaceWaterDeliveryMeasurement?.ReportedValue ?? 0;

        var previousOtherSurfaceWaterConsumptionMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == otherSurfaceWaterConsumption && x.UsageEntityName == usageEntityName);
        var previousOtherSurfaceWaterConsumptionValueInAcreFeet = previousOtherSurfaceWaterConsumptionMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousOtherSurfaceWaterConsumptionValueInInches = previousOtherSurfaceWaterConsumptionMeasurement?.ReportedValue ?? 0;


        var previousEffectivePrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID && x.UsageEntityName == usageEntityName);
        var previousEffectivePrecipValueInAcreFeet = previousEffectivePrecipMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousEffectivePrecipValueInInches = previousEffectivePrecipMeasurement?.ReportedValue ?? 0;

        var previousConsumedGroundwaterWithCreditMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterWithCreditMeasurementTypeID && x.UsageEntityName == usageEntityName);
        var previousConsumedGroundwaterWithCreditValueInAcreFeet = previousConsumedGroundwaterWithCreditMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousConsumedGroundwaterWithCreditValueInInches = previousConsumedGroundwaterWithCreditMeasurement?.ReportedValue ?? 0;

        #endregion

        #region Upsert Other Surface Water Delivery and Run Calculations

        if (otherSurfaceWaterDeliveryMeasurement == null)
        {
            otherSurfaceWaterDeliveryMeasurement = new WaterMeasurement()
            {
                UsageEntityName = usageEntityName,
                UsageEntityArea = (decimal?)usageEntity.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                ReportedDate = dateToCalculate,
                ReportedValue = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                WaterMeasurementTypeID = otherSurfaceWaterDeliveryMeasurementTypeID,
                UnitTypeID = 1,
                GeographyID = geographyID
            };

            _dbContext.WaterMeasurements.Add(otherSurfaceWaterDeliveryMeasurement);
        }
        else
        {
            otherSurfaceWaterDeliveryMeasurement.ReportedValue = updatedAmountInInches;
            otherSurfaceWaterDeliveryMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeography(_dbContext, geographyID, otherSurfaceWaterDeliveryMeasurementTypeID, dateToCalculate);

        #endregion

        #region Check Other Surface Water Delivery

        var updatedOtherSurfaceWaterDeliveryMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == otherSurfaceWaterDeliveryMeasurementTypeID && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedOtherSurfaceWaterDeliveryMeasurement);

        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedOtherSurfaceWaterDeliveryMeasurement.ReportedValueInAcreFeet);
        Assert.AreEqual(updatedAmountInInches, updatedOtherSurfaceWaterDeliveryMeasurement.ReportedValue);

        #endregion

        #region Check Other Surface Water Consumption

        var updatedOtherSurfaceWaterConsumptionMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == otherSurfaceWaterConsumption && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedOtherSurfaceWaterConsumptionMeasurement);

        var consumedSurfaceWaterMeasurementType = await _dbContext.WaterMeasurementTypes.SingleOrDefaultAsync(x => x.WaterMeasurementTypeID == otherSurfaceWaterConsumption);
        var parsed = consumedSurfaceWaterMeasurementType.CalculationJSON.TryParseJObject(out var calculationJSON);
        Assert.IsTrue(parsed);

        var surfaceWaterEfficiencyFactor = calculationJSON["SurfaceWaterEfficiencyFactor"]!.Value<decimal>();

        var expectedOtherSurfaceWaterConsumptionValueInAcreFeet = Math.Round(updatedAmountInAcreFeetAsDecimal * surfaceWaterEfficiencyFactor, 4, MidpointRounding.ToEven);
        var expectedOtherSurfaceWaterConsumptionValueInInches = Math.Round(updatedAmountInInches * surfaceWaterEfficiencyFactor, 4, MidpointRounding.ToEven);

        var updatedOtherSurfaceWaterConsumptionValueInAcreFeet = Math.Round(updatedOtherSurfaceWaterConsumptionMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);
        var updatedOtherSurfaceWaterConsumptionValueInInches = Math.Round(updatedOtherSurfaceWaterConsumptionMeasurement.ReportedValue, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedOtherSurfaceWaterConsumptionValueInAcreFeet - updatedOtherSurfaceWaterConsumptionValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedOtherSurfaceWaterConsumptionValueInAcreFeet}. Got: {updatedOtherSurfaceWaterConsumptionValueInAcreFeet}");
        Assert.IsTrue(Math.Abs(expectedOtherSurfaceWaterConsumptionValueInInches - updatedOtherSurfaceWaterConsumptionValueInInches) <= _acceptableTolerance, $"Reported Value in Inches do not match within tolerance. Expected: {expectedOtherSurfaceWaterConsumptionValueInInches}. Got: {updatedOtherSurfaceWaterConsumptionValueInInches}");

        #endregion

        #region Check ETMinusPrecipMinusTotalSurfaceWater

        var evapotranspirationWaterMeasurement = await _dbContext.WaterMeasurements
            .Where(x => x.WaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.ET.WaterMeasurementCategoryTypeID)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageEntityName == usageEntityName);

        var consumedGroundwaterWithCreditMeasurementType = await _dbContext.WaterMeasurementTypes.AsNoTracking()
            .Include(x => x.WaterMeasurementTypeDependencyWaterMeasurementTypes).ThenInclude(x => x.DependsOnWaterMeasurementType)
            .SingleOrDefaultAsync(x => x.WaterMeasurementTypeID == consumedGroundwaterWithCreditMeasurementTypeID);

        var surfaceWaterMeasurementTypeIDs = consumedGroundwaterWithCreditMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
            .Where(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.SurfaceWater.WaterMeasurementCategoryTypeID || x.DependsOnWaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.CalculateSurfaceWaterConsumption.WaterMeasurementCalculationTypeID)
            .Select(x => x.DependsOnWaterMeasurementType.WaterMeasurementTypeID)
            .ToList();

        var surfaceWaterMeasurements = await _dbContext.WaterMeasurements
            .Include(x => x.WaterMeasurementType)
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageEntityName == usageEntityName && surfaceWaterMeasurementTypeIDs.Contains(x.WaterMeasurementTypeID.GetValueOrDefault(0)))
            .ToListAsync();

        var surfaceWaterTotalInAcreFeet = surfaceWaterMeasurements.Sum(x => x.ReportedValueInAcreFeet);

        var expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet = Math.Round((evapotranspirationWaterMeasurement.ReportedValueInAcreFeet - previousEffectivePrecipMeasurement.ReportedValueInAcreFeet - surfaceWaterTotalInAcreFeet).GetValueOrDefault(0), 4, MidpointRounding.ToEven);

        var expectedETMinusPrecipMinusTotalSurfaceWaterInInches = Math.Round(UnitConversionHelper.ConvertAcreFeetToInches(expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet, (decimal)previousConsumedGroundwaterWithCreditMeasurement.UsageEntityArea), 4, MidpointRounding.ToEven);

        var updatedETMinusPrecipMinusTotalSurfaceWater = await _dbContext.WaterMeasurements
            .Include(x => x.WaterMeasurementType)
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageEntityName == usageEntityName && x.WaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.ETMinusPrecipMinusTotalSurfaceWater.WaterMeasurementCalculationTypeID);

        var updatedETMinusPrecipMinusTotalSurfaceWaterInInches = Math.Round(updatedETMinusPrecipMinusTotalSurfaceWater.ReportedValue, 4, MidpointRounding.ToEven);

        var updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet = Math.Round(updatedETMinusPrecipMinusTotalSurfaceWater.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedETMinusPrecipMinusTotalSurfaceWaterInInches - updatedETMinusPrecipMinusTotalSurfaceWaterInInches) <= _acceptableTolerance, $"Reported Value in Inches do not match within tolerance. Expected: {expectedETMinusPrecipMinusTotalSurfaceWaterInInches}. Got: {updatedETMinusPrecipMinusTotalSurfaceWaterInInches}");

        Assert.IsTrue(Math.Abs(expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet - updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet}. Got: {updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet}");

        #endregion
    }

    [DataTestMethod]
    [DataRow("20230731", "067-070-033", 1000.0d)]
    [Description("MIUGSA - After updating the extracted groundwater adjustment, the extracted groundwater should be updated to reflect that change, but the Unadjusted Extracted Groundwater should remain the same.")]
    public async Task MIUGSA_CanUpdateExtractedGroundwaterAdjustmentAndHaveDependantCalculationsModified(string dateAsString, string usageEntityName, double updatedAmountInAcreFeetAsDouble)
    {
        #region Hard Coded Assumptions

        var geographyID = 1; //1 == MIGUISA
        var extractedGroundwaterAdjustmentMeasurementTypeID = 41; //41==ExtractedGroundwaterAdjustment
        var unadjustedGroundwaterMeasurementTypeID = 42; //42==UnadjustedExtractedGroundwater
        var extractedGroundwaterMeasurementTypeID = 15; //15==ExtractedGroundwater
        var extractedAgainstSupplyMeasurementTypeID = 43; //43==ExtractedAgainstSupply

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var usageEntity = await _dbContext.UsageEntities.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageEntityName == usageEntityName);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var updatedAmountInInches = Math.Round(updatedAmountInAcreFeetAsDecimal / (decimal)usageEntity.UsageEntityArea, 4, MidpointRounding.ToEven);

        #endregion

        #region Previous Values

        var extractedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedGroundwaterMeasurementTypeID && x.UsageEntityName == usageEntityName);

        var previousExtractedGroundwaterValueInAcreFeet = extractedGroundwaterMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousExtractedGroundwaterValueInInches = extractedGroundwaterMeasurement?.ReportedValue ?? 0;

        var extractedGroundwaterAdjustment = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedGroundwaterAdjustmentMeasurementTypeID && x.UsageEntityName == usageEntityName);

        var previousAdjustmentAmountInInches = extractedGroundwaterAdjustment?.ReportedValue ?? 0;
        var previousAdjustmentAmountInAcreFeet = extractedGroundwaterAdjustment?.ReportedValueInAcreFeet ?? 0;

        var unadjustedExtractedGroundwater = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == unadjustedGroundwaterMeasurementTypeID && x.UsageEntityName == usageEntityName);

        var previousUnadjustedExtractedGroundwaterValueInAcreFeet = unadjustedExtractedGroundwater?.ReportedValueInAcreFeet ?? 0;
        var previousUnadjustedExtractedGroundwaterValueInInches = unadjustedExtractedGroundwater?.ReportedValue ?? 0;

        #endregion

        #region Upsert Adjustment with new Value and Run Calculations

        if (extractedGroundwaterAdjustment == null)
        {
            extractedGroundwaterAdjustment = new WaterMeasurement()
            {
                UsageEntityName = usageEntityName,
                UsageEntityArea = (decimal?)usageEntity.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                ReportedDate = dateToCalculate,
                ReportedValue = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                WaterMeasurementTypeID = extractedGroundwaterAdjustmentMeasurementTypeID,
                UnitTypeID = 1,
                GeographyID = geographyID
            };

            _dbContext.WaterMeasurements.Add(extractedGroundwaterAdjustment);
        }
        else
        {
            extractedGroundwaterAdjustment.ReportedValue = updatedAmountInInches;
            extractedGroundwaterAdjustment.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeography(_dbContext, geographyID, extractedGroundwaterAdjustmentMeasurementTypeID, dateToCalculate);

        #endregion

        #region Verify Updated Extracted Groundwater Adjustment

        var updatedExtractedGroundwaterAdjustment = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedGroundwaterAdjustmentMeasurementTypeID && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedExtractedGroundwaterAdjustment);
        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedExtractedGroundwaterAdjustment.ReportedValueInAcreFeet);
        Assert.AreEqual(updatedAmountInInches, updatedExtractedGroundwaterAdjustment.ReportedValue);

        #endregion

        #region Verify Extracted Groundwater

        var updatedExtractedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedGroundwaterMeasurementTypeID && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedExtractedGroundwaterMeasurement);

        var expectedExtractedGroundwaterInAcreFeet = Math.Round(previousExtractedGroundwaterValueInAcreFeet - previousAdjustmentAmountInAcreFeet + updatedAmountInAcreFeetAsDecimal, 4, MidpointRounding.ToEven);
        var expectedExtractedInInches = Math.Round(previousExtractedGroundwaterValueInInches - previousAdjustmentAmountInInches + updatedAmountInInches, 4, MidpointRounding.ToEven);

        var updatedExtractedGroundwaterInAcreFeetRounded = Math.Round(updatedExtractedGroundwaterMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);
        var updatedExtractedGroundwaterInInchesRounded = Math.Round(updatedExtractedGroundwaterMeasurement.ReportedValue, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedExtractedGroundwaterInAcreFeet - updatedExtractedGroundwaterInAcreFeetRounded) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedExtractedGroundwaterInAcreFeet}. Got: {updatedExtractedGroundwaterInAcreFeetRounded}");
        Assert.IsTrue(Math.Abs(expectedExtractedInInches - updatedExtractedGroundwaterInInchesRounded) <= _acceptableTolerance, $"Reported Value in Inches do not match within tolerance. Expected: {expectedExtractedInInches}. Got: {updatedExtractedGroundwaterInInchesRounded}");

        #endregion

        #region Verify Unadjusted Extracted Groundwater

        var unadjustedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == unadjustedGroundwaterMeasurementTypeID && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(unadjustedGroundwaterMeasurement);
        var unadjustedValueInAcreFeet = Math.Round(unadjustedGroundwaterMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);
        var unadjustedValueInInches = Math.Round(unadjustedGroundwaterMeasurement.ReportedValue, 4, MidpointRounding.ToEven);

        var expectedUnadjustedGroundwaterInAcreFeet = Math.Round(updatedExtractedGroundwaterInAcreFeetRounded - updatedAmountInAcreFeetAsDecimal, 4, MidpointRounding.ToEven);
        var expectedUnadjustedInInches = Math.Round(updatedExtractedGroundwaterInInchesRounded - updatedAmountInInches, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedUnadjustedGroundwaterInAcreFeet - unadjustedValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedUnadjustedGroundwaterInAcreFeet}. Got: {unadjustedValueInAcreFeet}");
        Assert.IsTrue(Math.Abs(expectedUnadjustedInInches - unadjustedValueInInches) <= _acceptableTolerance, $"Reported Value in Inches do not match within tolerance. Expected: {expectedUnadjustedInInches}. Got: {unadjustedValueInInches}");

        #endregion

        #region Check Extracted Against Supply

        var updatedExtractedAgainstSupplyMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedAgainstSupplyMeasurementTypeID && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedExtractedAgainstSupplyMeasurement);

        //MK 8/23/2024 -- The value should be 0 if the cumulative extra water for the Water Account is less than 0. Otherwise it should be the same as the extracted groundwater. This is kind of a lazy check but it should be helpful for now.
        Assert.IsTrue(updatedExtractedAgainstSupplyMeasurement.ReportedValueInAcreFeet == 0 || Math.Round(updatedExtractedAgainstSupplyMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven) == Math.Round(updatedExtractedGroundwaterMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven));

        #endregion
    }


    #endregion

    #region Demo Specific Tests

    [DataTestMethod]
    [DataRow("20230731", "555-038-36", 1000.0000d)]
    [Description("Demo - After updating the ET Evapotranspiration it should reflect in all the dependant calculations.")]
    public async Task Demo_CanUpdateETEvapotranspirationAndHaveDependantCalculationsModified(string dateAsString, string usageEntityName, double updatedAmountInAcreFeetAsDouble)
    {
        #region Hard Coded Assumptions

        var geographyID = 5; //5 ==Demo
        var etEvapotranspirationMeasurementTypeID = 5; //5==ETEvapotranspiration
        var etPrecipMeasurementType = 13; //13==ETPrecip
        var consumptiveUseMeasurementType = 19; //19==ConsumptiveUse

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var usageEntity = await _dbContext.UsageEntities.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageEntityName == usageEntityName);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var updatedAmountInInches = Math.Round(updatedAmountInAcreFeetAsDecimal / (decimal)usageEntity.UsageEntityArea, 4, MidpointRounding.ToEven);

        #endregion

        #region Previous Values

        var etEvapotranspirationMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etEvapotranspirationMeasurementTypeID && x.UsageEntityName == usageEntityName);
        var previousETEvapotranspirationValueInAcreFeet = etEvapotranspirationMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousETEvapotranspirationValueInInches = etEvapotranspirationMeasurement?.ReportedValue ?? 0;

        var previousConsumptiveUseMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumptiveUseMeasurementType && x.UsageEntityName == usageEntityName);
        var previousConsumptiveUseValueInAcreFeet = previousConsumptiveUseMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousConsumptiveUseValueInInches = previousConsumptiveUseMeasurement?.ReportedValue ?? 0;

        #endregion

        #region Upsert ET Evapotranspiration and Run Calculations

        if (etEvapotranspirationMeasurement == null)
        {
            etEvapotranspirationMeasurement = new WaterMeasurement()
            {
                UsageEntityName = usageEntityName,
                UsageEntityArea = (decimal?)usageEntity.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                ReportedDate = dateToCalculate,
                ReportedValue = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                WaterMeasurementTypeID = etEvapotranspirationMeasurementTypeID,
                UnitTypeID = 1,
                GeographyID = geographyID
            };

            _dbContext.WaterMeasurements.Add(etEvapotranspirationMeasurement);
        }
        else
        {
            etEvapotranspirationMeasurement.ReportedValue = updatedAmountInInches;
            etEvapotranspirationMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeography(_dbContext, geographyID, etEvapotranspirationMeasurementTypeID, dateToCalculate);

        #endregion

        #region Check ET Evapotranspiration

        var updatedETEvapotranspirationMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etEvapotranspirationMeasurementTypeID && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedETEvapotranspirationMeasurement);

        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedETEvapotranspirationMeasurement.ReportedValueInAcreFeet);
        Assert.AreEqual(updatedAmountInInches, updatedETEvapotranspirationMeasurement.ReportedValue);

        #endregion

        #region Check Consumptive Use

        var etPrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etPrecipMeasurementType && x.UsageEntityName == usageEntityName);


        var updatedConsumptiveUseMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumptiveUseMeasurementType && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedConsumptiveUseMeasurement);

        var expectedConsumptiveUseValueInAcreFeet = Math.Round(updatedAmountInAcreFeetAsDecimal - etPrecipMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);
        var expectedConsumptiveUseValueInInches = Math.Round(updatedAmountInInches - etPrecipMeasurement.ReportedValue, 4, MidpointRounding.ToEven);

        var updatedConsumptiveUseValueInAcreFeet = Math.Round(updatedConsumptiveUseMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);
        var updatedConsumptiveUseValueInInches = Math.Round(updatedConsumptiveUseMeasurement.ReportedValue, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedConsumptiveUseValueInAcreFeet - updatedConsumptiveUseValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedConsumptiveUseValueInAcreFeet}. Got: {updatedConsumptiveUseValueInAcreFeet}");
        Assert.IsTrue(Math.Abs(expectedConsumptiveUseValueInInches - updatedConsumptiveUseValueInInches) <= _acceptableTolerance, $"Reported Value in Inches do not match within tolerance. Expected: {expectedConsumptiveUseValueInInches}. Got: {updatedConsumptiveUseValueInInches}");

        #endregion
    }

    [DataTestMethod]
    [DataRow("20230731", "555-038-36", 1000.0000d)]
    [Description("Demo - After updating the ET Precip it should reflect in all the dependant calculations.")]
    public async Task Demo_CanUpdateETPrecipAndHaveDependantCalculationsModified(string dateAsString, string usageEntityName, double updatedAmountInAcreFeetAsDouble)
    {
        #region Hard Coded Assumptions

        var geographyID = 5; //5 ==Demo
        var etEvapotranspirationMeasurementTypeID = 5; //5==ETEvapotranspiration
        var etPrecipMeasurementType = 13; //13==ETPrecip
        var consumptiveUseMeasurementType = 19; //19==ConsumptiveUse

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var usageEntity = await _dbContext.UsageEntities.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageEntityName == usageEntityName);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var updatedAmountInInches = Math.Round(updatedAmountInAcreFeetAsDecimal / (decimal)usageEntity.UsageEntityArea, 4, MidpointRounding.ToEven);

        #endregion

        #region Previous Values

        var etPrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etPrecipMeasurementType && x.UsageEntityName == usageEntityName);
        var previousETPrecipValueInAcreFeet = etPrecipMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousETPrecipValueInInches = etPrecipMeasurement?.ReportedValue ?? 0;

        var previousConsumptiveUseMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumptiveUseMeasurementType && x.UsageEntityName == usageEntityName);
        var previousConsumptiveUseValueInAcreFeet = previousConsumptiveUseMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousConsumptiveUseValueInInches = previousConsumptiveUseMeasurement?.ReportedValue ?? 0;

        #endregion

        #region Upsert ET Precip and Run Calculations

        if (etPrecipMeasurement == null)
        {
            etPrecipMeasurement = new WaterMeasurement()
            {
                UsageEntityName = usageEntityName,
                UsageEntityArea = (decimal?)usageEntity.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                ReportedDate = dateToCalculate,
                ReportedValue = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                WaterMeasurementTypeID = etPrecipMeasurementType,
                UnitTypeID = 1,
                GeographyID = geographyID
            };

            _dbContext.WaterMeasurements.Add(etPrecipMeasurement);
        }
        else
        {
            etPrecipMeasurement.ReportedValue = updatedAmountInInches;
            etPrecipMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeography(_dbContext, geographyID, etPrecipMeasurementType, dateToCalculate);

        #endregion

        #region Check ET Precip

        var updatedETPrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etPrecipMeasurementType && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedETPrecipMeasurement);

        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedETPrecipMeasurement.ReportedValueInAcreFeet);
        Assert.AreEqual(updatedAmountInInches, updatedETPrecipMeasurement.ReportedValue);

        #endregion

        #region Check Consumptive Use

        var etEvapotranspirationMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etEvapotranspirationMeasurementTypeID && x.UsageEntityName == usageEntityName);

        var updatedConsumptiveUseMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumptiveUseMeasurementType && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedConsumptiveUseMeasurement);

        var expectedConsumptiveUseValueInAcreFeet = Math.Round(etEvapotranspirationMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0) - updatedAmountInAcreFeetAsDecimal, 4, MidpointRounding.ToEven);
        var expectedConsumptiveUseValueInInches = Math.Round(etEvapotranspirationMeasurement.ReportedValue - updatedAmountInInches, 4, MidpointRounding.ToEven);

        var updatedConsumptiveUseValueInAcreFeet = Math.Round(updatedConsumptiveUseMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);
        var updatedConsumptiveUseValueInInches = Math.Round(updatedConsumptiveUseMeasurement.ReportedValue, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedConsumptiveUseValueInAcreFeet - updatedConsumptiveUseValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedConsumptiveUseValueInAcreFeet}. Got: {updatedConsumptiveUseValueInAcreFeet}");
        Assert.IsTrue(Math.Abs(expectedConsumptiveUseValueInInches - updatedConsumptiveUseValueInInches) <= _acceptableTolerance, $"Reported Value in Inches do not match within tolerance. Expected: {expectedConsumptiveUseValueInInches}. Got: {updatedConsumptiveUseValueInInches}");

        #endregion
    }

    #endregion

    #region MSGSA Specific Tests

    [DataTestMethod]
    [DataRow("20230731", "049-150-001", 1000.0000d)]
    [Description("MSGSA - After updating the ET Evapotranspiration it should reflect in all the dependant calculations.")]
    public async Task MSGSA_CanUpdateETEvapotranspirationAndHaveDependantCalculationsModified(string dateAsString, string usageEntityName, double updatedAmountInAcreFeetAsDouble)
    {
        #region Hard Coded Assumptions

        var geographyID = 6; //6 == MSGSA
        var etEvapotranspirationMeasurementTypeID = 6; //5==ETEvapotranspiration
        var etPrecipMeasurementType = 14; //13==ETPrecip
        var consumptiveUseMeasurementType = 20; //19==ConsumptiveUse

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var usageEntity = await _dbContext.UsageEntities.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageEntityName == usageEntityName);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var updatedAmountInInches = Math.Round(updatedAmountInAcreFeetAsDecimal / (decimal)usageEntity.UsageEntityArea, 4, MidpointRounding.ToEven);

        #endregion

        #region Previous Values

        var etEvapotranspirationMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etEvapotranspirationMeasurementTypeID && x.UsageEntityName == usageEntityName);
        var previousETEvapotranspirationValueInAcreFeet = etEvapotranspirationMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousETEvapotranspirationValueInInches = etEvapotranspirationMeasurement?.ReportedValue ?? 0;

        var previousConsumptiveUseMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumptiveUseMeasurementType && x.UsageEntityName == usageEntityName);
        var previousConsumptiveUseValueInAcreFeet = previousConsumptiveUseMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousConsumptiveUseValueInInches = previousConsumptiveUseMeasurement?.ReportedValue ?? 0;

        #endregion

        #region Upsert ET Evapotranspiration and Run Calculations

        if (etEvapotranspirationMeasurement == null)
        {
            etEvapotranspirationMeasurement = new WaterMeasurement()
            {
                UsageEntityName = usageEntityName,
                UsageEntityArea = (decimal?)usageEntity.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                ReportedDate = dateToCalculate,
                ReportedValue = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                WaterMeasurementTypeID = etEvapotranspirationMeasurementTypeID,
                UnitTypeID = 1,
                GeographyID = geographyID
            };

            _dbContext.WaterMeasurements.Add(etEvapotranspirationMeasurement);
        }
        else
        {
            etEvapotranspirationMeasurement.ReportedValue = updatedAmountInInches;
            etEvapotranspirationMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeography(_dbContext, geographyID, etEvapotranspirationMeasurementTypeID, dateToCalculate);

        #endregion

        #region Check ET Evapotranspiration

        var updatedETEvapotranspirationMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etEvapotranspirationMeasurementTypeID && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedETEvapotranspirationMeasurement);

        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedETEvapotranspirationMeasurement.ReportedValueInAcreFeet);
        Assert.AreEqual(updatedAmountInInches, updatedETEvapotranspirationMeasurement.ReportedValue);

        #endregion

        #region Check Consumptive Use

        var etPrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etPrecipMeasurementType && x.UsageEntityName == usageEntityName);


        var updatedConsumptiveUseMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumptiveUseMeasurementType && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedConsumptiveUseMeasurement);

        var expectedConsumptiveUseValueInAcreFeet = Math.Round(updatedAmountInAcreFeetAsDecimal - etPrecipMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);
        var expectedConsumptiveUseValueInInches = Math.Round(updatedAmountInInches - etPrecipMeasurement.ReportedValue, 4, MidpointRounding.ToEven);

        var updatedConsumptiveUseValueInAcreFeet = Math.Round(updatedConsumptiveUseMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);
        var updatedConsumptiveUseValueInInches = Math.Round(updatedConsumptiveUseMeasurement.ReportedValue, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedConsumptiveUseValueInAcreFeet - updatedConsumptiveUseValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedConsumptiveUseValueInAcreFeet}. Got: {updatedConsumptiveUseValueInAcreFeet}");
        Assert.IsTrue(Math.Abs(expectedConsumptiveUseValueInInches - updatedConsumptiveUseValueInInches) <= _acceptableTolerance, $"Reported Value in Inches do not match within tolerance. Expected: {expectedConsumptiveUseValueInInches}. Got: {updatedConsumptiveUseValueInInches}");

        #endregion
    }

    [DataTestMethod]
    [DataRow("20230731", "049-150-001", 1000.0000d)]
    [Description("MSGSA - After updating the ET Precip it should reflect in all the dependant calculations.")]
    public async Task MSGSA_CanUpdateETPrecipAndHaveDependantCalculationsModified(string dateAsString, string usageEntityName, double updatedAmountInAcreFeetAsDouble)
    {
        #region Hard Coded Assumptions

        var geographyID = 6; //6 == MSGSA
        var etEvapotranspirationMeasurementTypeID = 6; //5==ETEvapotranspiration
        var etPrecipMeasurementType = 14; //13==ETPrecip
        var consumptiveUseMeasurementType = 20; //19==ConsumptiveUse

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var usageEntity = await _dbContext.UsageEntities.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageEntityName == usageEntityName);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var updatedAmountInInches = Math.Round(updatedAmountInAcreFeetAsDecimal / (decimal)usageEntity.UsageEntityArea, 4, MidpointRounding.ToEven);

        #endregion

        #region Previous Values

        var etPrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etPrecipMeasurementType && x.UsageEntityName == usageEntityName);
        var previousETPrecipValueInAcreFeet = etPrecipMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousETPrecipValueInInches = etPrecipMeasurement?.ReportedValue ?? 0;

        var previousConsumptiveUseMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumptiveUseMeasurementType && x.UsageEntityName == usageEntityName);
        var previousConsumptiveUseValueInAcreFeet = previousConsumptiveUseMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousConsumptiveUseValueInInches = previousConsumptiveUseMeasurement?.ReportedValue ?? 0;

        #endregion

        #region Upsert ET Precip and Run Calculations

        if (etPrecipMeasurement == null)
        {
            etPrecipMeasurement = new WaterMeasurement()
            {
                UsageEntityName = usageEntityName,
                UsageEntityArea = (decimal?)usageEntity.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                ReportedDate = dateToCalculate,
                ReportedValue = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                WaterMeasurementTypeID = etPrecipMeasurementType,
                UnitTypeID = 1,
                GeographyID = geographyID
            };

            _dbContext.WaterMeasurements.Add(etPrecipMeasurement);
        }
        else
        {
            etPrecipMeasurement.ReportedValue = updatedAmountInInches;
            etPrecipMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeography(_dbContext, geographyID, etPrecipMeasurementType, dateToCalculate);

        #endregion

        #region Check ET Precip

        var updatedETPrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etPrecipMeasurementType && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedETPrecipMeasurement);

        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedETPrecipMeasurement.ReportedValueInAcreFeet);
        Assert.AreEqual(updatedAmountInInches, updatedETPrecipMeasurement.ReportedValue);

        #endregion

        #region Check Consumptive Use

        var etEvapotranspirationMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etEvapotranspirationMeasurementTypeID && x.UsageEntityName == usageEntityName);

        var updatedConsumptiveUseMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumptiveUseMeasurementType && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedConsumptiveUseMeasurement);

        var expectedConsumptiveUseValueInAcreFeet = Math.Round(etEvapotranspirationMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0) - updatedAmountInAcreFeetAsDecimal, 4, MidpointRounding.ToEven);
        var expectedConsumptiveUseValueInInches = Math.Round(etEvapotranspirationMeasurement.ReportedValue - updatedAmountInInches, 4, MidpointRounding.ToEven);

        var updatedConsumptiveUseValueInAcreFeet = Math.Round(updatedConsumptiveUseMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);
        var updatedConsumptiveUseValueInInches = Math.Round(updatedConsumptiveUseMeasurement.ReportedValue, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedConsumptiveUseValueInAcreFeet - updatedConsumptiveUseValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedConsumptiveUseValueInAcreFeet}. Got: {updatedConsumptiveUseValueInAcreFeet}");
        Assert.IsTrue(Math.Abs(expectedConsumptiveUseValueInInches - updatedConsumptiveUseValueInInches) <= _acceptableTolerance, $"Reported Value in Inches do not match within tolerance. Expected: {expectedConsumptiveUseValueInInches}. Got: {updatedConsumptiveUseValueInInches}");

        #endregion
    }

    #endregion

    #region ETSGSA Specific Tests 

    [DataTestMethod]
    [DataRow("20240331", "ETSGSA-9318-1297-2024", 1000.0000d)]
    [Description("ETSGSA - After updating the LandIQ Precip it should reflect in all the dependant calculations.")]
    public async Task ETSGSA_CanUpdateLandIQPrecipAndHaveDependantCalculationsModified(string dateAsString, string usageEntityName, double updatedAmountInAcreFeetAsDouble)
    {
        #region Hard Coded Assumptions

        var geographyID = 7; //7 == ETSGSA
        var landIQPrecipMeasurementTypeID = 22; //22==LandIQPrecip
        var effectivePrecipMeasurementTypeID = 34; //34==EffectivePrecip

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var usageEntity = await _dbContext.UsageEntities.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageEntityName == usageEntityName);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var updatedAmountInInches = Math.Round(updatedAmountInAcreFeetAsDecimal / (decimal)usageEntity.UsageEntityArea, 4, MidpointRounding.ToEven);

        #endregion

        #region Previous Values

        var landIQPrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == landIQPrecipMeasurementTypeID && x.UsageEntityName == usageEntityName);
        var previousLandIQPrecipValueInAcreFeet = landIQPrecipMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousLandIQPrecipValueInInches = landIQPrecipMeasurement?.ReportedValue ?? 0;

        var previousEffectivePrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID && x.UsageEntityName == usageEntityName);
        var previousEffectivePrecipValueInAcreFeet = previousEffectivePrecipMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousEffectivePrecipValueInInches = previousEffectivePrecipMeasurement?.ReportedValue ?? 0;

        #endregion

        #region Upsert LandIQ Precip and Run Calculations

        if (landIQPrecipMeasurement == null)
        {
            landIQPrecipMeasurement = new WaterMeasurement()
            {
                UsageEntityName = usageEntityName,
                UsageEntityArea = (decimal?)usageEntity.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                ReportedDate = dateToCalculate,
                ReportedValue = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                WaterMeasurementTypeID = landIQPrecipMeasurementTypeID,
                UnitTypeID = 1,
                GeographyID = geographyID
            };

            _dbContext.WaterMeasurements.Add(landIQPrecipMeasurement);
        }
        else
        {
            landIQPrecipMeasurement.ReportedValue = updatedAmountInInches;
            landIQPrecipMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeography(_dbContext, geographyID, landIQPrecipMeasurementTypeID, dateToCalculate);

        #endregion

        #region Check Precip

        var updatedLandIQPrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == landIQPrecipMeasurementTypeID && x.UsageEntityName == usageEntityName);
        Assert.IsNotNull(updatedLandIQPrecipMeasurement);
        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedLandIQPrecipMeasurement.ReportedValueInAcreFeet);
        Assert.AreEqual(updatedAmountInInches, updatedLandIQPrecipMeasurement.ReportedValue);

        #endregion

        #region Check Effective Precip

        var geographyAllocationPlanConfiguration = await _dbContext.GeographyAllocationPlanConfigurations
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID);

        if (geographyAllocationPlanConfiguration == null)
        {
            // todo: use AlertMessageDto to throw error instead of exception
            throw new Exception("Could not calculate Effective Precip because this geography does not have the required Allocation Zone Group configured.");
        }

        var zoneIDByParcelIDs = await _dbContext.ParcelZones.AsNoTracking()
            .Include(x => x.Zone)
            .Where(x => x.Zone.ZoneGroupID == geographyAllocationPlanConfiguration.ZoneGroupID)
            .ToDictionaryAsync(x => x.ParcelID, x => x.ZoneID);

        var zoneIDsByUsageEntityNames = _dbContext.UsageEntities.AsNoTracking()
            .Include(x => x.Parcel)
            .Where(x => x.GeographyID == geographyID)
            .AsEnumerable()
            .Where(x => zoneIDByParcelIDs.ContainsKey(x.ParcelID))
            .ToLookup(x => x.UsageEntityName, x => zoneIDByParcelIDs[x.ParcelID]);

        var precipMultiplierByZoneID = await _dbContext.Zones.AsNoTracking()
            .Where(x => x.ZoneGroupID == geographyAllocationPlanConfiguration.ZoneGroupID)
            .ToDictionaryAsync(x => x.ZoneID, x => x.PrecipMultiplier);

        var zoneID = zoneIDsByUsageEntityNames[usageEntityName].First();
        if (!precipMultiplierByZoneID.ContainsKey(zoneID) || !precipMultiplierByZoneID[zoneID].HasValue)
        {
            throw new Exception("Could not calculate Effective Precip because at least one Allocation Zone does not have a precipitation multiplier configured.");
        }

        var effectivePrecipMultiplier = precipMultiplierByZoneID[zoneID].Value;

        var expectedEffectivePrecipValueInAcreFeet = Math.Round(updatedAmountInAcreFeetAsDecimal * effectivePrecipMultiplier, 4, MidpointRounding.ToEven);
        var expectedEffectivePrecipValueInInches = Math.Round(updatedAmountInInches * effectivePrecipMultiplier, 4, MidpointRounding.ToEven);

        var updatedEffectivePrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID && x.UsageEntityName == usageEntityName);
        Assert.IsNotNull(updatedEffectivePrecipMeasurement);
        Assert.IsTrue(Math.Abs(expectedEffectivePrecipValueInAcreFeet - updatedEffectivePrecipMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0)) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedEffectivePrecipValueInAcreFeet}. Got: {updatedEffectivePrecipMeasurement.ReportedValueInAcreFeet}");
        Assert.IsTrue(Math.Abs(expectedEffectivePrecipValueInInches - updatedEffectivePrecipMeasurement.ReportedValue) <= _acceptableTolerance, $"Reported Value in Inches do not match within tolerance. Expected: {expectedEffectivePrecipValueInInches}. Got: {updatedEffectivePrecipMeasurement.ReportedValue}");

        #endregion

        #region Check ETMinusPrecipMinusTotalSurfaceWater

        var evapotranspirationWaterMeasurement = await _dbContext.WaterMeasurements
            .Where(x => x.WaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.ET.WaterMeasurementCategoryTypeID)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageEntityName == usageEntityName);

        var surfaceWaterTypeMeasurements = await _dbContext.WaterMeasurements
            .Where(x => x.WaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.CalculateSurfaceWaterConsumption.WaterMeasurementCalculationTypeID || x.WaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.SurfaceWater.WaterMeasurementCategoryTypeID)
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageEntityName == usageEntityName)
            .ToListAsync();

        var surfaceWaterTotalInAcreFeet = surfaceWaterTypeMeasurements.Sum(x => x.ReportedValueInAcreFeet);

        var expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet = Math.Round((evapotranspirationWaterMeasurement.ReportedValueInAcreFeet - updatedEffectivePrecipMeasurement.ReportedValueInAcreFeet - surfaceWaterTotalInAcreFeet).GetValueOrDefault(0), 4, MidpointRounding.ToEven);

        var expectedETMinusPrecipMinusTotalSurfaceWaterInInches = Math.Round(UnitConversionHelper.ConvertAcreFeetToMillimeters(expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet, (decimal)landIQPrecipMeasurement.UsageEntityArea), 4, MidpointRounding.ToEven);

        var updatedETMinusPrecipMinusTotalSurfaceWater = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageEntityName == usageEntityName && x.WaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.ETMinusPrecipMinusTotalSurfaceWater.WaterMeasurementCalculationTypeID);

        var updatedETMinusPrecipMinusTotalSurfaceWaterInInches = Math.Round(updatedETMinusPrecipMinusTotalSurfaceWater.ReportedValue, 4, MidpointRounding.ToEven);

        var updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet = Math.Round(updatedETMinusPrecipMinusTotalSurfaceWater.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedETMinusPrecipMinusTotalSurfaceWaterInInches - updatedETMinusPrecipMinusTotalSurfaceWaterInInches) <= _acceptableTolerance, $"Reported Value in Inches do not match within tolerance. Expected: {expectedETMinusPrecipMinusTotalSurfaceWaterInInches}. Got: {updatedETMinusPrecipMinusTotalSurfaceWaterInInches}");

        Assert.IsTrue(Math.Abs(expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet - updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet}. Got: {updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet}");

        #endregion
    }

    [DataTestMethod]
    [DataRow("20240331", "ETSGSA-9318-1297-2024", 1000.0000d)]
    [Description("ETSGSA - After updating the LandIQ ET it should reflect in all the dependant calculations.")]
    public async Task ETSGSA_CanUpdateLandETAndHaveDependantCalculationsModified(string dateAsString, string usageEntityName, double updatedAmountInAcreFeetAsDouble)
    {
        #region Hard Coded Assumptions

        var geographyID = 7; //7 == ETSGSA
        var landIQETMeasurementTypeID = 21; //21==LandIQ ET
        var effectivePrecipMeasurementTypeID = 34; //34==effectivePrecip
        var consumedGroundwaterMeasurementTypeID = 33; //33==ConsumedGroundwater

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var usageEntity = await _dbContext.UsageEntities.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageEntityName == usageEntityName);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var updatedAmountInInches = UnitConversionHelper.ConvertAcreFeetToMillimeters(updatedAmountInAcreFeetAsDecimal, (decimal)usageEntity.UsageEntityArea);

        #endregion

        #region Previous Values

        var landIQETMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == landIQETMeasurementTypeID && x.UsageEntityName == usageEntityName);

        var previousLandIQETValueInAcreFeet = landIQETMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousLandIQETValueInInches = landIQETMeasurement?.ReportedValue ?? 0;

        var previousConsumedGroundwaterMeasurement = await _dbContext.WaterMeasurements.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterMeasurementTypeID && x.UsageEntityName == usageEntityName);

        var previousConsumedGroundwaterValueInAcreFeet = previousConsumedGroundwaterMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousConsumedGroundwaterValueInInches = previousConsumedGroundwaterMeasurement?.ReportedValue ?? 0;

        #endregion

        #region Upsert LandIQ ET and Run Calculations

        if (landIQETMeasurement == null)
        {
            landIQETMeasurement = new WaterMeasurement()
            {
                UsageEntityName = usageEntityName,
                UsageEntityArea = (decimal?)usageEntity.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                ReportedDate = dateToCalculate,
                ReportedValue = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                WaterMeasurementTypeID = landIQETMeasurementTypeID,
                UnitTypeID = 1,
                GeographyID = geographyID
            };

            _dbContext.WaterMeasurements.Add(landIQETMeasurement);
        }
        else
        {
            landIQETMeasurement.ReportedValue = updatedAmountInInches;
            landIQETMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeography(_dbContext, geographyID, landIQETMeasurementTypeID, dateToCalculate);

        #endregion

        #region Check ET

        var updatedLandIQETMeasurement = await _dbContext.WaterMeasurements.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == landIQETMeasurementTypeID && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedLandIQETMeasurement);

        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedLandIQETMeasurement.ReportedValueInAcreFeet);
        Assert.AreEqual(Math.Round(updatedAmountInInches, 4, MidpointRounding.ToEven), updatedLandIQETMeasurement.ReportedValue);

        #endregion

        #region Check Consumed Groundwater

        var precipMeasurement = await _dbContext.WaterMeasurements.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementType.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID && x.UsageEntityName == usageEntityName);

        var previousPrecipInInches = precipMeasurement?.ReportedValue ?? 0;
        var previousPrecipInAcreFeet = precipMeasurement?.ReportedValueInAcreFeet ?? 0;

        var surfaceWaterMeasurements = await _dbContext.WaterMeasurements.AsNoTracking()
            .Include(x => x.WaterMeasurementType)
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageEntityName == usageEntityName && x.WaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.SurfaceWater.WaterMeasurementCategoryTypeID)
            .ToListAsync();

        var surfaceWaterTotalInInches = surfaceWaterMeasurements.Sum(x => x.ReportedValue);
        var surfaceWaterTotalInAcreFeet = surfaceWaterMeasurements.Sum(x => x.ReportedValueInAcreFeet.GetValueOrDefault(0));

        var consumedGroundwaterMeasurement = await _dbContext.WaterMeasurements.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterMeasurementTypeID && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(consumedGroundwaterMeasurement);

        var expectedConsumedGroundwaterValueInAcreFeet = Math.Round((updatedLandIQETMeasurement.ReportedValueInAcreFeet - previousPrecipInAcreFeet - surfaceWaterTotalInAcreFeet).GetValueOrDefault(0), 4, MidpointRounding.ToEven);

        var expectedConsumedGroundwaterValueInInches = Math.Round(UnitConversionHelper.ConvertAcreFeetToMillimeters(expectedConsumedGroundwaterValueInAcreFeet, (decimal)updatedLandIQETMeasurement.UsageEntityArea), 4, MidpointRounding.ToEven);

        var updatedConsumedGroundwaterValueInInches = Math.Round(consumedGroundwaterMeasurement.ReportedValue, 4, MidpointRounding.ToEven);
        var updatedConsumedGroundwaterValueInAcreFeet = Math.Round(consumedGroundwaterMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedConsumedGroundwaterValueInInches - updatedConsumedGroundwaterValueInInches) <= _acceptableTolerance, $"Reported Value in Inches do not match within tolerance. Expected: {expectedConsumedGroundwaterValueInInches}. Got: {updatedConsumedGroundwaterValueInInches}");
        Assert.IsTrue(Math.Abs(expectedConsumedGroundwaterValueInAcreFeet - updatedConsumedGroundwaterValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedConsumedGroundwaterValueInAcreFeet}. Got: {updatedConsumedGroundwaterValueInAcreFeet}");

        #endregion
    }


    [DataTestMethod]
    [DataRow("20240331", "ETSGSA-9318-1297-2024", 1000.0000d, 28)]
    [DataRow("20240331", "ETSGSA-9318-1297-2024", 1000.0000d, 29)]
    [DataRow("20240331", "ETSGSA-9318-1297-2024", 1000.0000d, 30)]
    [DataRow("20240331", "ETSGSA-9318-1297-2024", 1000.0000d, 31)]
    [DataRow("20240331", "ETSGSA-9318-1297-2024", 1000.0000d, 32)]
    [Description("ETSGSA - After updating a measurement of category surface water, it should reflect in all the dependant calculations.")]
    public async Task ETSGSA_CanUpdateSurfaceWaterAndHaveDependantCalculationsModified(string dateAsString, string usageEntityName, double updatedAmountInAcreFeetAsDouble, int surfaceWaterMeasurementTypeID)
    {
        #region Hard Coded Assumptions

        var geographyID = 7; //7 == ETSGSA
        var landIQETMeasurementTypeID = 21; //21==LandIQ ET
        var effectivePrecipMeasurementTypeID = 34; //34==effectivePrecip
        var consumedGroundwaterMeasurementTypeID = 33; //33==ConsumedGroundwater

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var usageEntity = await _dbContext.UsageEntities.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.UsageEntityName == usageEntityName);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var updatedAmountInInches = UnitConversionHelper.ConvertAcreFeetToMillimeters(updatedAmountInAcreFeetAsDecimal, (decimal)usageEntity.UsageEntityArea);

        #endregion

        #region Previous Values

        var surfaceWaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == surfaceWaterMeasurementTypeID && x.UsageEntityName == usageEntityName);

        var previousSurfaceWaterValueInAcreFeet = surfaceWaterMeasurement?.ReportedValueInAcreFeet ?? 0;
        var previousSurfaceWaterValueInInches = surfaceWaterMeasurement?.ReportedValue ?? 0;

        #endregion

        #region Upsert Surface Water and Run Calculations

        if (surfaceWaterMeasurement == null)
        {
            surfaceWaterMeasurement = new WaterMeasurement()
            {
                UsageEntityName = usageEntityName,
                UsageEntityArea = (decimal?)usageEntity.UsageEntityArea,
                LastUpdateDate = DateTime.UtcNow,
                ReportedDate = dateToCalculate,
                ReportedValue = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                WaterMeasurementTypeID = surfaceWaterMeasurementTypeID,
                UnitTypeID = 1,
                GeographyID = geographyID
            };

            _dbContext.WaterMeasurements.Add(surfaceWaterMeasurement);
        }
        else
        {
            surfaceWaterMeasurement.ReportedValue = updatedAmountInInches;
            surfaceWaterMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeography(_dbContext, geographyID, surfaceWaterMeasurementTypeID, dateToCalculate);

        #endregion

        #region Check Surface Water

        var updatedSurfaceWaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == surfaceWaterMeasurementTypeID && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedSurfaceWaterMeasurement);

        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedSurfaceWaterMeasurement.ReportedValueInAcreFeet);
        Assert.AreEqual(Math.Round(updatedAmountInInches, 4, MidpointRounding.ToEven), Math.Round(updatedSurfaceWaterMeasurement.ReportedValue, 4, MidpointRounding.ToEven));

        #endregion

        #region Check Consumed Groundwater

        var updatedLandIQETMeasurement = await _dbContext.WaterMeasurements.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == landIQETMeasurementTypeID && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(updatedLandIQETMeasurement);

        var precipMeasurement = await _dbContext.WaterMeasurements.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementType.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID && x.UsageEntityName == usageEntityName);

        var previousPrecipInAcreFeet = precipMeasurement?.ReportedValueInAcreFeet ?? 0;

        var surfaceWaterMeasurements = await _dbContext.WaterMeasurements.AsNoTracking()
            .Include(x => x.WaterMeasurementType)
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageEntityName == usageEntityName && x.WaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.SurfaceWater.WaterMeasurementCategoryTypeID)
            .ToListAsync();

        var surfaceWaterTotalInAcreFeet = surfaceWaterMeasurements.Sum(x => x.ReportedValueInAcreFeet.GetValueOrDefault(0));

        var consumedGroundwaterMeasurement = await _dbContext.WaterMeasurements.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterMeasurementTypeID && x.UsageEntityName == usageEntityName);

        Assert.IsNotNull(consumedGroundwaterMeasurement);

        var expectedConsumedGroundwaterValueInAcreFeet = Math.Round((updatedLandIQETMeasurement.ReportedValueInAcreFeet - previousPrecipInAcreFeet - surfaceWaterTotalInAcreFeet).GetValueOrDefault(0), 4, MidpointRounding.ToEven);

        var expectedConsumedGroundwaterValueInInches = Math.Round(UnitConversionHelper.ConvertAcreFeetToMillimeters(expectedConsumedGroundwaterValueInAcreFeet, (decimal)updatedLandIQETMeasurement.UsageEntityArea), 4, MidpointRounding.ToEven);

        var updatedConsumedGroundwaterValueInInches = Math.Round(consumedGroundwaterMeasurement.ReportedValue, 4, MidpointRounding.ToEven);
        var updatedConsumedGroundwaterValueInAcreFeet = Math.Round(consumedGroundwaterMeasurement.ReportedValueInAcreFeet.GetValueOrDefault(0), 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedConsumedGroundwaterValueInInches - updatedConsumedGroundwaterValueInInches) <= _acceptableTolerance, $"Reported Value in Inches do not match within tolerance. Expected: {expectedConsumedGroundwaterValueInInches}. Got: {updatedConsumedGroundwaterValueInInches}");
        Assert.IsTrue(Math.Abs(expectedConsumedGroundwaterValueInAcreFeet - updatedConsumedGroundwaterValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedConsumedGroundwaterValueInAcreFeet}. Got: {updatedConsumedGroundwaterValueInAcreFeet}");

        #endregion
    }

    #endregion
}