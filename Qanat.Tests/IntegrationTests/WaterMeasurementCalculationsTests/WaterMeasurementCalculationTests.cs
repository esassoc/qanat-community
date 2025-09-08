using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.Common.Util;
using Qanat.EFModels.Entities;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static Qanat.EFModels.Entities.WaterMeasurementCalculations;

namespace Qanat.Tests.IntegrationTests.WaterMeasurementCalculationsTests;

[TestClass]
public class WaterMeasurementCalculationTests
{
    private static QanatDbContext _dbContext;

    //MK 8/23/2024 -- I think we need to be rounding all values (including acres) to 4 decimal places on WaterMeasurement save. I am going to apply a tolerance for now, but we should add in a story to remove the rounding and tolerance check from these tests.
    private readonly decimal _acceptableTolerance = .001m;

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

    [TestMethod]
    [DataRow("20230731", "067-070-033", 1000.0000d, true)]
    [DataRow("20230731", "067-070-033", 1000.0000d, false)]
    [Description("MIUGSA - After updating the ET Precip it should reflect in all the dependant calculations.")]
    public async Task MIUGSA_CanUpdateETPrecipAndHaveDependantCalculationsModified(string dateAsString, string usageLocationName, double updatedAmountInAcreFeetAsDouble, bool updateForAllUsageLocations)
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
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(_dbContext, geographyID, dateToCalculate.Year);
        var usageLocation = await _dbContext.UsageLocations.SingleAsync(x => x.GeographyID == geographyID && x.Name == usageLocationName && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var depthInFeet = Math.Round(updatedAmountInAcreFeetAsDecimal / (decimal)usageLocation.Area, 4, MidpointRounding.ToEven);
        var updatedAmountInInches = Math.Round(depthInFeet / 12, 4, MidpointRounding.ToEven);

        #endregion

        #region Previous Values

        var etPrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == openETPrecipMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousETPrecipValueInAcreFeet = etPrecipMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousEffectivePrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousEffectivePrecipValueInAcreFeet = previousEffectivePrecipMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousPrecipitationCreditMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == precipitationCreditMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousPrecipitationCreditValueInAcreFeet = previousPrecipitationCreditMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousConsumedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousConsumedGroundwaterValueInAcreFeet = previousConsumedGroundwaterMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousConsumedGroundwaterWithCreditMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterWithCreditMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousConsumedGroundwaterWithCreditValueInAcreFeet = previousConsumedGroundwaterWithCreditMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousUnadjustedExtractedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == unadjustedGroundwaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousUnadjustedExtractedGroundwaterValueInAcreFeet = previousUnadjustedExtractedGroundwaterMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousExtractedGroundwaterAdjustmentMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedGroundwaterAdjustmentMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousExtractedGroundwaterAdjustmentValueInAcreFeet = previousExtractedGroundwaterAdjustmentMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousExtractedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedGroundwaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousExtractedGroundwaterValueInAcreFeet = previousExtractedGroundwaterMeasurement?.ReportedValueInAcreFeet ?? 0;

        #endregion

        #region Upsert OpenET Precip and Run Calculations

        if (etPrecipMeasurement == null)
        {
            etPrecipMeasurement = new WaterMeasurement()
            {
                GeographyID = geographyID,
                UsageLocationID = usageLocation.UsageLocationID,
                WaterMeasurementTypeID = openETPrecipMeasurementTypeID,
                ReportedDate = dateToCalculate,
                ReportedValueInNativeUnits = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                ReportedValueInFeet = depthInFeet,
                LastUpdateDate = DateTime.UtcNow,
            };

            _dbContext.WaterMeasurements.Add(etPrecipMeasurement);
        }
        else
        {
            etPrecipMeasurement.ReportedValueInNativeUnits = updatedAmountInInches;
            etPrecipMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
            etPrecipMeasurement.ReportedValueInFeet = depthInFeet;
        }

        await _dbContext.SaveChangesAsync();

        if (updateForAllUsageLocations)
        {
            await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(_dbContext, geographyID, openETPrecipMeasurementTypeID, dateToCalculate);
        }
        else
        {
            await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(_dbContext, geographyID, openETPrecipMeasurementTypeID, dateToCalculate, [usageLocation.UsageLocationID]);
        }


        #endregion

        #region Check Precip

        var updatedETPrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == openETPrecipMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        Assert.IsNotNull(updatedETPrecipMeasurement);
        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedETPrecipMeasurement.ReportedValueInAcreFeet);

        var updatedAmountInFeet = Math.Round(updatedAmountInAcreFeetAsDecimal / (decimal)usageLocation.Area, 4, MidpointRounding.ToEven);
        Assert.AreEqual(updatedAmountInFeet, updatedETPrecipMeasurement.ReportedValueInFeet);

        #endregion

        #region Check Effective Precip by Zone

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

        var zoneIDsByUsageLocationID = _dbContext.UsageLocations.AsNoTracking()
            .Include(x => x.Parcel)
            .Where(x => x.GeographyID == geographyID)
            .AsEnumerable()
            .Where(x => zoneIDByParcelIDs.ContainsKey(x.ParcelID))
            .ToLookup(x => x.Name, x => zoneIDByParcelIDs[x.ParcelID]);

        var precipMultiplierByZoneID = await _dbContext.Zones.AsNoTracking()
            .Where(x => x.ZoneGroupID == geographyAllocationPlanConfiguration.ZoneGroupID)
            .ToDictionaryAsync(x => x.ZoneID, x => x.PrecipMultiplier);

        var zoneID = zoneIDsByUsageLocationID[usageLocationName].First();
        if (!precipMultiplierByZoneID.ContainsKey(zoneID) || !precipMultiplierByZoneID[zoneID].HasValue)
        {
            throw new Exception("Could not calculate Effective Precip because at least one Allocation Zone does not have a precipitation multiplier configured.");
        }

        var effectivePrecipMultiplier = precipMultiplierByZoneID[zoneID].Value;

        var expectedEffectivePrecipValueInAcreFeet = Math.Round(updatedAmountInAcreFeetAsDecimal * effectivePrecipMultiplier, 4, MidpointRounding.ToEven);

        var updatedEffectivePrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        Assert.IsNotNull(updatedEffectivePrecipMeasurement);
        Assert.IsTrue(Math.Abs(expectedEffectivePrecipValueInAcreFeet - updatedEffectivePrecipMeasurement.ReportedValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedEffectivePrecipValueInAcreFeet}. Got: {updatedEffectivePrecipMeasurement.ReportedValueInAcreFeet}");

        #endregion

        #region Check ETMinusPrecipMinusTotalSurfaceWater

        var evapotranspirationWaterMeasurement = await _dbContext.WaterMeasurements
            .Where(x => x.WaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.ET.WaterMeasurementCategoryTypeID)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageLocationID == usageLocation.UsageLocationID);

        var consumedGroundwaterWithCreditMeasurementType = await _dbContext.WaterMeasurementTypes.AsNoTracking()
            .Include(x => x.WaterMeasurementTypeDependencyWaterMeasurementTypes).ThenInclude(x => x.DependsOnWaterMeasurementType)
            .SingleOrDefaultAsync(x => x.WaterMeasurementTypeID == consumedGroundwaterWithCreditMeasurementTypeID);

        var surfaceWaterMeasurementTypeIDs = consumedGroundwaterWithCreditMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
            .Where(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.SurfaceWater.WaterMeasurementCategoryTypeID || x.DependsOnWaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.CalculateSurfaceWaterConsumption.WaterMeasurementCalculationTypeID)
            .Select(x => x.DependsOnWaterMeasurementType.WaterMeasurementTypeID)
            .ToList();

        var surfaceWaterMeasurements = await _dbContext.WaterMeasurements
            .Include(x => x.WaterMeasurementType)
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageLocationID == usageLocation.UsageLocationID && surfaceWaterMeasurementTypeIDs.Contains(x.WaterMeasurementTypeID.GetValueOrDefault(0)))
            .ToListAsync();

        var surfaceWaterTotalInAcreFeet = surfaceWaterMeasurements.Sum(x => x.ReportedValueInAcreFeet);

        var expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet = Math.Round((evapotranspirationWaterMeasurement.ReportedValueInAcreFeet - updatedEffectivePrecipMeasurement.ReportedValueInAcreFeet - surfaceWaterTotalInAcreFeet), 4, MidpointRounding.ToEven);

        var expectedETMinusPrecipMinusTotalSurfaceWaterInInches = Math.Round(UnitConversionHelper.ConvertAcreFeetToInches(expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet, (decimal)usageLocation.Area), 4, MidpointRounding.ToEven);

        var updatedETMinusPrecipMinusTotalSurfaceWater = await _dbContext.WaterMeasurements
            .Include(x => x.WaterMeasurementType)
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageLocationID == usageLocation.UsageLocationID && x.WaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.ETMinusPrecipMinusTotalSurfaceWater.WaterMeasurementCalculationTypeID);


        var updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet = Math.Round(updatedETMinusPrecipMinusTotalSurfaceWater.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet - updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet}. Got: {updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet}");

        #endregion

        #region Check Positive Consumed Groundwater and Precipitation Credit

        var updatedPrecipitationCreditMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == precipitationCreditMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        var updatedPositiveConsumedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        if (updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet < 0)
        {
            Assert.IsNotNull(updatedPrecipitationCreditMeasurement);
            Assert.AreEqual(updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet, updatedPrecipitationCreditMeasurement.ReportedValueInAcreFeet);

            Assert.IsNotNull(updatedPositiveConsumedGroundwaterMeasurement);
            Assert.AreEqual(0, updatedPositiveConsumedGroundwaterMeasurement.ReportedValueInAcreFeet);
        }
        else if (updatedPrecipitationCreditMeasurement != null)
        {
            Assert.AreEqual(0, updatedPrecipitationCreditMeasurement.ReportedValueInAcreFeet);
            Assert.AreEqual(0, updatedPrecipitationCreditMeasurement.ReportedValueInFeet);

            Assert.IsNotNull(updatedPositiveConsumedGroundwaterMeasurement);
            Assert.AreEqual(updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet, updatedPositiveConsumedGroundwaterMeasurement.ReportedValueInAcreFeet);
        }

        #endregion

        #region Check Extracted Groundwater Adjustment

        var extractedGroundwaterAdjustment = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedGroundwaterAdjustmentMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        #endregion

        #region Check Extracted Groundwater

        var extractedGroundwaterMeasurementType = await _dbContext.WaterMeasurementTypes.SingleOrDefaultAsync(x => x.WaterMeasurementTypeID == extractedGroundwaterMeasurementTypeID);
        var calculationJSON = System.Text.Json.JsonSerializer.Deserialize<WaterMeasurementCalculations.GroundwaterCalculationDto>(extractedGroundwaterMeasurementType.CalculationJSON);

        var groundwaterEfficiencyFactor = calculationJSON.GroundwaterEfficiencyFactor;
        var groundwaterAdjustment = await _dbContext.WaterMeasurements
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.ManualAdjustment.WaterMeasurementCategoryTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        var updatedExtractedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedGroundwaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedExtractedGroundwaterMeasurement);

        var expectedExtractedGroundwaterValueInAcreFeet = Math.Round(updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet / groundwaterEfficiencyFactor + (groundwaterAdjustment?.ReportedValueInAcreFeet ?? 0), 4, MidpointRounding.ToEven);

        var updatedExtractedGroundwaterValueInAcreFeet = Math.Round(updatedExtractedGroundwaterMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedExtractedGroundwaterValueInAcreFeet - updatedExtractedGroundwaterValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedExtractedGroundwaterValueInAcreFeet}. Got: {updatedExtractedGroundwaterValueInAcreFeet}");

        #endregion

        #region Check Unadjusted Extracted Groundwater

        var updatedUnadjustedExtractedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == unadjustedGroundwaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedUnadjustedExtractedGroundwaterMeasurement);

        if (extractedGroundwaterAdjustment != null)
        {
            var expectedUnadjustedExtractedGroundwaterValueInAcreFeet = Math.Round(updatedExtractedGroundwaterValueInAcreFeet - extractedGroundwaterAdjustment.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);
            var updatedUnadjustedExtractedGroundwaterValueInAcreFeet = Math.Round(updatedUnadjustedExtractedGroundwaterMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

            Assert.IsTrue(Math.Abs(expectedUnadjustedExtractedGroundwaterValueInAcreFeet - updatedUnadjustedExtractedGroundwaterValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedUnadjustedExtractedGroundwaterValueInAcreFeet}. Got: {updatedUnadjustedExtractedGroundwaterValueInAcreFeet}");
        }

        #endregion

        #region Check Extracted Against Supply

        var updatedExtractedAgainstSupplyMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedAgainstSupplyMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedExtractedAgainstSupplyMeasurement);

        //MK 8/23/2024 -- The value should be 0 if the cumulative extra water for the Water Account is less than 0. Otherwise it should be the same as the extracted groundwater. This is kind of a lazy check but it should be helpful for now.

        var updatedExtractedAgainstSupplyMeasurementRounded = Math.Round(updatedExtractedAgainstSupplyMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

        Assert.IsTrue(updatedExtractedAgainstSupplyMeasurement.ReportedValueInAcreFeet == 0 || Math.Abs(updatedExtractedAgainstSupplyMeasurementRounded - updatedExtractedGroundwaterValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {updatedExtractedAgainstSupplyMeasurementRounded}. Got: {updatedExtractedGroundwaterValueInAcreFeet}");

        #endregion
    }


    [TestMethod]
    [DataRow("20230731", "067-070-033", 1000.0000d)]
    [Description("MIUGSA - After updating the MID Surface Water Delivery it should reflect in all the dependant calculations.")]
    public async Task MIUGSA_CanUpdateMIDSurfaceWaterDeliveryAndHaveDependantCalculationsModified(string dateAsString, string usageLocationName, double updatedAmountInAcreFeetAsDouble)
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
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(_dbContext, geographyID, dateToCalculate.Year);
        var usageLocation = await _dbContext.UsageLocations.SingleAsync(x => x.GeographyID == geographyID && x.Name == usageLocationName && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var updatedAmountInInches = Math.Round(updatedAmountInAcreFeetAsDecimal / (decimal)usageLocation.Area, 4, MidpointRounding.ToEven);

        #endregion

        #region Previous Values

        var midSurfaceWaterDeliveryMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == midSurfaceWaterDeliveryMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousMidSurfaceWaterDeliveryValueInAcreFeet = midSurfaceWaterDeliveryMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousMidSurfaceWaterConsumptionMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == midSurfaceWaterConsumption && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousMidSurfaceWaterConsumptionValueInAcreFeet = previousMidSurfaceWaterConsumptionMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousEffectivePrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousEffectivePrecipValueInAcreFeet = previousEffectivePrecipMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousConsumedGroundwaterWithCreditMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterWithCreditMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousConsumedGroundwaterWithCreditValueInAcreFeet = previousConsumedGroundwaterWithCreditMeasurement?.ReportedValueInAcreFeet ?? 0;

        #endregion

        #region Upsert MID Surface Water Delivery and Run Calculations

        if (midSurfaceWaterDeliveryMeasurement == null)
        {
            midSurfaceWaterDeliveryMeasurement = new WaterMeasurement()
            {
                GeographyID = geographyID,
                UsageLocationID = usageLocation.UsageLocationID,
                WaterMeasurementTypeID = midSurfaceWaterDeliveryMeasurementTypeID,
                UnitTypeID = UnitType.Inches.UnitTypeID,
                ReportedDate = dateToCalculate,
                ReportedValueInNativeUnits = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                LastUpdateDate = DateTime.UtcNow,
            };

            _dbContext.WaterMeasurements.Add(midSurfaceWaterDeliveryMeasurement);
        }
        else
        {
            midSurfaceWaterDeliveryMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(_dbContext, geographyID, midSurfaceWaterDeliveryMeasurementTypeID, dateToCalculate);

        #endregion

        #region Check MID Surface Water Delivery

        var updatedMidSurfaceWaterDeliveryMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == midSurfaceWaterDeliveryMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedMidSurfaceWaterDeliveryMeasurement);
        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedMidSurfaceWaterDeliveryMeasurement.ReportedValueInAcreFeet);

        #endregion

        #region Check MID Surface Water Consumption

        var updatedMidSurfaceWaterConsumptionMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == midSurfaceWaterConsumption && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedMidSurfaceWaterConsumptionMeasurement);

        var consumedSurfaceWaterMeasurementType = await _dbContext.WaterMeasurementTypes.SingleOrDefaultAsync(x => x.WaterMeasurementTypeID == midSurfaceWaterConsumption);

        var calculationJSON = System.Text.Json.JsonSerializer.Deserialize<WaterMeasurementCalculations.SurfaceWaterConsumptionCalculationDto>(consumedSurfaceWaterMeasurementType.CalculationJSON);

        var surfaceWaterEfficiencyFactor = calculationJSON.SurfaceWaterEfficiencyFactor;
        var expectedMidSurfaceWaterConsumptionValueInAcreFeet = Math.Round(updatedAmountInAcreFeetAsDecimal * surfaceWaterEfficiencyFactor, 4, MidpointRounding.ToEven);
        var expectedMidSurfaceWaterConsumptionValueInInches = Math.Round(updatedAmountInInches * surfaceWaterEfficiencyFactor, 4, MidpointRounding.ToEven);

        var updatedMidSurfaceWaterConsumptionValueInAcreFeet = Math.Round(updatedMidSurfaceWaterConsumptionMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedMidSurfaceWaterConsumptionValueInAcreFeet - updatedMidSurfaceWaterConsumptionValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedMidSurfaceWaterConsumptionValueInAcreFeet}. Got: {updatedMidSurfaceWaterConsumptionValueInAcreFeet}");

        #endregion

        #region Check ETMinusPrecipMinusTotalSurfaceWater

        var evapotranspirationWaterMeasurement = await _dbContext.WaterMeasurements
            .Where(x => x.WaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.ET.WaterMeasurementCategoryTypeID)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageLocationID == usageLocation.UsageLocationID);

        var consumedGroundwaterWithCreditMeasurementType = await _dbContext.WaterMeasurementTypes.AsNoTracking()
            .Include(x => x.WaterMeasurementTypeDependencyWaterMeasurementTypes).ThenInclude(x => x.DependsOnWaterMeasurementType)
            .SingleOrDefaultAsync(x => x.WaterMeasurementTypeID == consumedGroundwaterWithCreditMeasurementTypeID);

        var surfaceWaterMeasurementTypeIDs = consumedGroundwaterWithCreditMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
            .Where(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.SurfaceWater.WaterMeasurementCategoryTypeID || x.DependsOnWaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.CalculateSurfaceWaterConsumption.WaterMeasurementCalculationTypeID)
            .Select(x => x.DependsOnWaterMeasurementType.WaterMeasurementTypeID)
            .ToList();

        var surfaceWaterMeasurements = await _dbContext.WaterMeasurements
            .Include(x => x.WaterMeasurementType)
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageLocationID == usageLocation.UsageLocationID && surfaceWaterMeasurementTypeIDs.Contains(x.WaterMeasurementTypeID.GetValueOrDefault(0)))
            .ToListAsync();

        var surfaceWaterTotalInAcreFeet = surfaceWaterMeasurements.Sum(x => x.ReportedValueInAcreFeet);

        var expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet = Math.Round((evapotranspirationWaterMeasurement.ReportedValueInAcreFeet - previousEffectivePrecipMeasurement.ReportedValueInAcreFeet - surfaceWaterTotalInAcreFeet), 4, MidpointRounding.ToEven);

        var expectedETMinusPrecipMinusTotalSurfaceWaterInInches = Math.Round(UnitConversionHelper.ConvertAcreFeetToInches(expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet, (decimal)usageLocation.Area), 4, MidpointRounding.ToEven);

        var updatedETMinusPrecipMinusTotalSurfaceWater = await _dbContext.WaterMeasurements
            .Include(x => x.WaterMeasurementType)
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageLocationID == usageLocation.UsageLocationID && x.WaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.ETMinusPrecipMinusTotalSurfaceWater.WaterMeasurementCalculationTypeID);

        var updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet = Math.Round(updatedETMinusPrecipMinusTotalSurfaceWater.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet - updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet}. Got: {updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet}");

        #endregion
    }

    [TestMethod]
    [DataRow("20230731", "067-070-033", 1000.0000d)]
    [Description("MIUGSA - After updating the Other Surface Water Delivery it should reflect in all the dependant calculations.")]
    public async Task MIUGSA_CanUpdateOtherSurfaceWaterDeliveryAndHaveDependantCalculationsModified(string dateAsString, string usageLocationName, double updatedAmountInAcreFeetAsDouble)
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
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(_dbContext, geographyID, dateToCalculate.Year);
        var usageLocation = await _dbContext.UsageLocations.SingleAsync(x => x.GeographyID == geographyID && x.Name == usageLocationName && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var updatedAmountInInches = Math.Round(updatedAmountInAcreFeetAsDecimal / (decimal)usageLocation.Area, 4, MidpointRounding.ToEven) / 12;
        var depthInFeet = updatedAmountInAcreFeetAsDecimal / (decimal)usageLocation.Area;

        #endregion

        #region Previous Values

        var otherSurfaceWaterDeliveryMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == otherSurfaceWaterDeliveryMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousOtherSurfaceWaterDeliveryValueInAcreFeet = otherSurfaceWaterDeliveryMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousOtherSurfaceWaterConsumptionMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == otherSurfaceWaterConsumption && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousOtherSurfaceWaterConsumptionValueInAcreFeet = previousOtherSurfaceWaterConsumptionMeasurement?.ReportedValueInAcreFeet ?? 0;


        var previousEffectivePrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousEffectivePrecipValueInAcreFeet = previousEffectivePrecipMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousConsumedGroundwaterWithCreditMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterWithCreditMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousConsumedGroundwaterWithCreditValueInAcreFeet = previousConsumedGroundwaterWithCreditMeasurement?.ReportedValueInAcreFeet ?? 0;

        #endregion

        #region Upsert Other Surface Water Delivery and Run Calculations

        if (otherSurfaceWaterDeliveryMeasurement == null)
        {
            otherSurfaceWaterDeliveryMeasurement = new WaterMeasurement()
            {
                GeographyID = geographyID,
                UsageLocationID = usageLocation.UsageLocationID,
                WaterMeasurementTypeID = otherSurfaceWaterDeliveryMeasurementTypeID,
                UnitTypeID = UnitType.Inches.UnitTypeID,
                ReportedDate = dateToCalculate,
                ReportedValueInNativeUnits = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                LastUpdateDate = DateTime.UtcNow,
            };

            _dbContext.WaterMeasurements.Add(otherSurfaceWaterDeliveryMeasurement);
        }
        else
        {
            otherSurfaceWaterDeliveryMeasurement.ReportedValueInAcreFeet = updatedAmountInInches;
            otherSurfaceWaterDeliveryMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
            otherSurfaceWaterDeliveryMeasurement.ReportedValueInFeet = depthInFeet;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(_dbContext, geographyID, otherSurfaceWaterDeliveryMeasurementTypeID, dateToCalculate);

        #endregion

        #region Check Other Surface Water Delivery

        var updatedOtherSurfaceWaterDeliveryMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == otherSurfaceWaterDeliveryMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedOtherSurfaceWaterDeliveryMeasurement);

        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedOtherSurfaceWaterDeliveryMeasurement.ReportedValueInAcreFeet);

        #endregion

        #region Check Other Surface Water Consumption

        var updatedOtherSurfaceWaterConsumptionMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == otherSurfaceWaterConsumption && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedOtherSurfaceWaterConsumptionMeasurement);

        var consumedSurfaceWaterMeasurementType = await _dbContext.WaterMeasurementTypes.SingleOrDefaultAsync(x => x.WaterMeasurementTypeID == otherSurfaceWaterConsumption);

        var calculationJSON = System.Text.Json.JsonSerializer.Deserialize<WaterMeasurementCalculations.SurfaceWaterConsumptionCalculationDto>(consumedSurfaceWaterMeasurementType.CalculationJSON);
        var surfaceWaterEfficiencyFactor = calculationJSON.SurfaceWaterEfficiencyFactor;

        var expectedOtherSurfaceWaterConsumptionValueInAcreFeet = Math.Round(updatedAmountInAcreFeetAsDecimal * surfaceWaterEfficiencyFactor, 4, MidpointRounding.ToEven);
        var expectedOtherSurfaceWaterConsumptionValueInInches = Math.Round(updatedAmountInInches * surfaceWaterEfficiencyFactor, 4, MidpointRounding.ToEven);

        var updatedOtherSurfaceWaterConsumptionValueInAcreFeet = Math.Round(updatedOtherSurfaceWaterConsumptionMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);
        Assert.IsTrue(Math.Abs(expectedOtherSurfaceWaterConsumptionValueInAcreFeet - updatedOtherSurfaceWaterConsumptionValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedOtherSurfaceWaterConsumptionValueInAcreFeet}. Got: {updatedOtherSurfaceWaterConsumptionValueInAcreFeet}");

        #endregion

        #region Check ETMinusPrecipMinusTotalSurfaceWater

        var evapotranspirationWaterMeasurement = await _dbContext.WaterMeasurements
            .Where(x => x.WaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.ET.WaterMeasurementCategoryTypeID)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageLocationID == usageLocation.UsageLocationID);

        var consumedGroundwaterWithCreditMeasurementType = await _dbContext.WaterMeasurementTypes.AsNoTracking()
            .Include(x => x.WaterMeasurementTypeDependencyWaterMeasurementTypes).ThenInclude(x => x.DependsOnWaterMeasurementType)
            .SingleOrDefaultAsync(x => x.WaterMeasurementTypeID == consumedGroundwaterWithCreditMeasurementTypeID);

        var surfaceWaterMeasurementTypeIDs = consumedGroundwaterWithCreditMeasurementType.WaterMeasurementTypeDependencyWaterMeasurementTypes
            .Where(x => x.DependsOnWaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.SurfaceWater.WaterMeasurementCategoryTypeID || x.DependsOnWaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.CalculateSurfaceWaterConsumption.WaterMeasurementCalculationTypeID)
            .Select(x => x.DependsOnWaterMeasurementType.WaterMeasurementTypeID)
            .ToList();

        var surfaceWaterMeasurements = await _dbContext.WaterMeasurements
            .Include(x => x.WaterMeasurementType)
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageLocationID == usageLocation.UsageLocationID && surfaceWaterMeasurementTypeIDs.Contains(x.WaterMeasurementTypeID.GetValueOrDefault(0)))
            .ToListAsync();

        var surfaceWaterTotalInAcreFeet = surfaceWaterMeasurements.Sum(x => x.ReportedValueInAcreFeet);

        var expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet = Math.Round((evapotranspirationWaterMeasurement.ReportedValueInAcreFeet - previousEffectivePrecipMeasurement.ReportedValueInAcreFeet - surfaceWaterTotalInAcreFeet), 4, MidpointRounding.ToEven);

        var expectedETMinusPrecipMinusTotalSurfaceWaterInInches = Math.Round(UnitConversionHelper.ConvertAcreFeetToInches(expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet, (decimal)usageLocation.Area), 4, MidpointRounding.ToEven);

        var updatedETMinusPrecipMinusTotalSurfaceWater = await _dbContext.WaterMeasurements
            .Include(x => x.WaterMeasurementType)
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageLocationID == usageLocation.UsageLocationID && x.WaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.ETMinusPrecipMinusTotalSurfaceWater.WaterMeasurementCalculationTypeID);

        var updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet = Math.Round(updatedETMinusPrecipMinusTotalSurfaceWater.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);
        Assert.IsTrue(Math.Abs(expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet - updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet}. Got: {updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet}");

        #endregion
    }

    [TestMethod]
    [DataRow("20230731", "067-070-033", 1000.0d)]
    [Description("MIUGSA - After updating the extracted groundwater adjustment, the extracted groundwater should be updated to reflect that change, but the Unadjusted Extracted Groundwater should remain the same.")]
    public async Task MIUGSA_CanUpdateExtractedGroundwaterAdjustmentAndHaveDependantCalculationsModified(string dateAsString, string usageLocationName, double updatedAmountInAcreFeetAsDouble)
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
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(_dbContext, geographyID, dateToCalculate.Year);
        var usageLocation = await _dbContext.UsageLocations.SingleAsync(x => x.GeographyID == geographyID && x.Name == usageLocationName && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var updatedAmountInInches = Math.Round(updatedAmountInAcreFeetAsDecimal / (decimal)usageLocation.Area, 4, MidpointRounding.ToEven) / 12;
        var depthInFeet = updatedAmountInAcreFeetAsDecimal / (decimal)usageLocation.Area;

        #endregion

        #region Previous Values

        var extractedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedGroundwaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        var previousExtractedGroundwaterValueInAcreFeet = extractedGroundwaterMeasurement?.ReportedValueInAcreFeet ?? 0;

        var extractedGroundwaterAdjustment = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedGroundwaterAdjustmentMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        var previousAdjustmentAmountInAcreFeet = extractedGroundwaterAdjustment?.ReportedValueInAcreFeet ?? 0;

        var unadjustedExtractedGroundwater = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == unadjustedGroundwaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        var previousUnadjustedExtractedGroundwaterValueInAcreFeet = unadjustedExtractedGroundwater?.ReportedValueInAcreFeet ?? 0;

        #endregion

        #region Upsert Adjustment with new Value and Run Calculations

        if (extractedGroundwaterAdjustment == null)
        {
            extractedGroundwaterAdjustment = new WaterMeasurement()
            {
                GeographyID = geographyID,
                UsageLocationID = usageLocation.UsageLocationID,
                WaterMeasurementTypeID = extractedGroundwaterAdjustmentMeasurementTypeID,
                UnitTypeID = UnitType.Inches.UnitTypeID,
                ReportedDate = dateToCalculate,
                ReportedValueInNativeUnits = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                ReportedValueInFeet = depthInFeet,
                LastUpdateDate = DateTime.UtcNow,
            };

            _dbContext.WaterMeasurements.Add(extractedGroundwaterAdjustment);
        }
        else
        {
            extractedGroundwaterAdjustment.ReportedValueInNativeUnits = updatedAmountInInches;
            extractedGroundwaterAdjustment.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
            extractedGroundwaterAdjustment.ReportedValueInFeet = depthInFeet;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(_dbContext, geographyID, extractedGroundwaterAdjustmentMeasurementTypeID, dateToCalculate);

        #endregion

        #region Verify Updated Extracted Groundwater Adjustment

        var updatedExtractedGroundwaterAdjustment = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedGroundwaterAdjustmentMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedExtractedGroundwaterAdjustment);
        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedExtractedGroundwaterAdjustment.ReportedValueInAcreFeet);

        #endregion

        #region Verify Extracted Groundwater

        var updatedExtractedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedGroundwaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedExtractedGroundwaterMeasurement);

        var expectedExtractedGroundwaterInAcreFeet = Math.Round(previousExtractedGroundwaterValueInAcreFeet - previousAdjustmentAmountInAcreFeet + updatedAmountInAcreFeetAsDecimal, 4, MidpointRounding.ToEven);

        var updatedExtractedGroundwaterInAcreFeetRounded = Math.Round(updatedExtractedGroundwaterMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedExtractedGroundwaterInAcreFeet - updatedExtractedGroundwaterInAcreFeetRounded) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedExtractedGroundwaterInAcreFeet}. Got: {updatedExtractedGroundwaterInAcreFeetRounded}");
        #endregion

        #region Verify Unadjusted Extracted Groundwater

        var unadjustedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == unadjustedGroundwaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(unadjustedGroundwaterMeasurement);
        var unadjustedValueInAcreFeet = Math.Round(unadjustedGroundwaterMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);
        var expectedUnadjustedGroundwaterInAcreFeet = Math.Round(updatedExtractedGroundwaterInAcreFeetRounded - updatedAmountInAcreFeetAsDecimal, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedUnadjustedGroundwaterInAcreFeet - unadjustedValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedUnadjustedGroundwaterInAcreFeet}. Got: {unadjustedValueInAcreFeet}");

        #endregion

        #region Check Extracted Against Supply

        var updatedExtractedAgainstSupplyMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == extractedAgainstSupplyMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedExtractedAgainstSupplyMeasurement);

        //MK 8/23/2024 -- The value should be 0 if the cumulative extra water for the Water Account is less than 0. Otherwise it should be the same as the extracted groundwater.
        var delta = Math.Round(updatedExtractedAgainstSupplyMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven) - Math.Round(updatedExtractedGroundwaterMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);
        Assert.IsTrue(updatedExtractedAgainstSupplyMeasurement.ReportedValueInAcreFeet == 0 || Math.Abs(delta) < _acceptableTolerance);

        #endregion
    }


    #endregion

    #region Demo Specific Tests

    [TestMethod]
    [DataRow("20230731", "555-038-36", 1000.0000d)]
    [Description("Demo - After updating the ET Evapotranspiration it should reflect in all the dependant calculations.")]
    public async Task Demo_CanUpdateETEvapotranspirationAndHaveDependantCalculationsModified(string dateAsString, string usageLocationName, double updatedAmountInAcreFeetAsDouble)
    {
        #region Hard Coded Assumptions

        var geographyID = 5; //5 ==Demo
        var etEvapotranspirationMeasurementTypeID = 5; //5==ETEvapotranspiration
        var etPrecipMeasurementType = 13; //13==ETPrecip
        var consumptiveUseMeasurementType = 19; //19==ConsumptiveUse

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(_dbContext, geographyID, dateToCalculate.Year);
        var usageLocation = await _dbContext.UsageLocations.SingleAsync(x => x.GeographyID == geographyID && x.Name == usageLocationName && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var depthInFeet = updatedAmountInAcreFeetAsDecimal / (decimal)usageLocation.Area;
        var updatedAmountInInches = Math.Round(depthInFeet, 4, MidpointRounding.ToEven) / 12;

        #endregion

        #region Previous Values

        var etEvapotranspirationMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etEvapotranspirationMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousETEvapotranspirationValueInAcreFeet = etEvapotranspirationMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousConsumptiveUseMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumptiveUseMeasurementType && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousConsumptiveUseValueInAcreFeet = previousConsumptiveUseMeasurement?.ReportedValueInAcreFeet ?? 0;

        #endregion

        #region Upsert ET Evapotranspiration and Run Calculations

        if (etEvapotranspirationMeasurement == null)
        {
            etEvapotranspirationMeasurement = new WaterMeasurement()
            {
                GeographyID = geographyID,
                UsageLocationID = usageLocation.UsageLocationID,
                WaterMeasurementTypeID = etEvapotranspirationMeasurementTypeID,
                UnitTypeID = UnitType.Inches.UnitTypeID,
                ReportedDate = dateToCalculate,
                ReportedValueInNativeUnits = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                ReportedValueInFeet = depthInFeet,
                LastUpdateDate = DateTime.UtcNow,
            };

            _dbContext.WaterMeasurements.Add(etEvapotranspirationMeasurement);
        }
        else
        {
            etEvapotranspirationMeasurement.ReportedValueInNativeUnits = updatedAmountInInches;
            etEvapotranspirationMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
            etEvapotranspirationMeasurement.ReportedValueInFeet = depthInFeet;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(_dbContext, geographyID, etEvapotranspirationMeasurementTypeID, dateToCalculate);

        #endregion

        #region Check ET Evapotranspiration

        var updatedETEvapotranspirationMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etEvapotranspirationMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedETEvapotranspirationMeasurement);

        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedETEvapotranspirationMeasurement.ReportedValueInAcreFeet);

        #endregion

        #region Check Consumptive Use

        var etPrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etPrecipMeasurementType && x.UsageLocationID == usageLocation.UsageLocationID);
        var updatedConsumptiveUseMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumptiveUseMeasurementType && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedConsumptiveUseMeasurement);

        var expectedConsumptiveUseValueInAcreFeet = Math.Round(updatedAmountInAcreFeetAsDecimal - etPrecipMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

        var updatedConsumptiveUseValueInAcreFeet = Math.Round(updatedConsumptiveUseMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedConsumptiveUseValueInAcreFeet - updatedConsumptiveUseValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedConsumptiveUseValueInAcreFeet}. Got: {updatedConsumptiveUseValueInAcreFeet}");

        #endregion
    }

    [TestMethod]
    [DataRow("20230731", "555-038-36", 1000.0000d)]
    [Description("Demo - After updating the ET Precip it should reflect in all the dependant calculations.")]
    public async Task Demo_CanUpdateETPrecipAndHaveDependantCalculationsModified(string dateAsString, string usageLocationName, double updatedAmountInAcreFeetAsDouble)
    {
        #region Hard Coded Assumptions

        var geographyID = 5; //5 ==Demo
        var etEvapotranspirationMeasurementTypeID = 5; //5==ETEvapotranspiration
        var etPrecipMeasurementType = 13; //13==ETPrecip
        var consumptiveUseMeasurementType = 19; //19==ConsumptiveUse

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(_dbContext, geographyID, dateToCalculate.Year);
        var usageLocation = await _dbContext.UsageLocations.SingleAsync(x => x.GeographyID == geographyID && x.Name == usageLocationName && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var depthInFeet = updatedAmountInAcreFeetAsDecimal / (decimal)usageLocation.Area;
        var updatedAmountInInches = depthInFeet / 12;

        #endregion

        #region Previous Values

        var etPrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etPrecipMeasurementType && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousETPrecipValueInAcreFeet = etPrecipMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousConsumptiveUseMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumptiveUseMeasurementType && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousConsumptiveUseValueInAcreFeet = previousConsumptiveUseMeasurement?.ReportedValueInAcreFeet ?? 0;

        #endregion

        #region Upsert ET Precip and Run Calculations

        if (etPrecipMeasurement == null)
        {
            etPrecipMeasurement = new WaterMeasurement()
            {
                GeographyID = geographyID,
                UsageLocationID = usageLocation.UsageLocationID,
                WaterMeasurementTypeID = etPrecipMeasurementType,
                UnitTypeID = UnitType.Inches.UnitTypeID,
                ReportedDate = dateToCalculate,
                ReportedValueInNativeUnits = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                ReportedValueInFeet = depthInFeet,
                LastUpdateDate = DateTime.UtcNow,
            };

            _dbContext.WaterMeasurements.Add(etPrecipMeasurement);
        }
        else
        {
            etPrecipMeasurement.ReportedValueInNativeUnits = updatedAmountInInches;
            etPrecipMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
            etPrecipMeasurement.ReportedValueInFeet = depthInFeet;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(_dbContext, geographyID, etPrecipMeasurementType, dateToCalculate);

        #endregion

        #region Check ET Precip

        var updatedETPrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etPrecipMeasurementType && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedETPrecipMeasurement);

        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedETPrecipMeasurement.ReportedValueInAcreFeet);

        #endregion

        #region Check Consumptive Use

        var etEvapotranspirationMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etEvapotranspirationMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        var updatedConsumptiveUseMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumptiveUseMeasurementType && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedConsumptiveUseMeasurement);

        var expectedConsumptiveUseValueInAcreFeet = Math.Round(etEvapotranspirationMeasurement.ReportedValueInAcreFeet - updatedAmountInAcreFeetAsDecimal, 4, MidpointRounding.ToEven);

        var updatedConsumptiveUseValueInAcreFeet = Math.Round(updatedConsumptiveUseMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedConsumptiveUseValueInAcreFeet - updatedConsumptiveUseValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedConsumptiveUseValueInAcreFeet}. Got: {updatedConsumptiveUseValueInAcreFeet}");

        #endregion
    }

    #endregion

    #region MSGSA Specific Tests

    [TestMethod]
    [DataRow("20230731", "049-150-001", 1000.0000d)]
    [Description("MSGSA - After updating the ET Evapotranspiration it should reflect in all the dependant calculations.")]
    public async Task MSGSA_CanUpdateETEvapotranspirationAndHaveDependantCalculationsModified(string dateAsString, string usageLocationName, double updatedAmountInAcreFeetAsDouble)
    {
        #region Hard Coded Assumptions

        var geographyID = 6; //6 == MSGSA
        var etEvapotranspirationMeasurementTypeID = 6; //6==ETEvapotranspiration
        var eligibleInchesOfRain = 52; //52==Eligible Inches of Rain
        var consumedGroundwaterMeasurementType = 20; //20==ConsumedGroundwater

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(_dbContext, geographyID, dateToCalculate.Year);
        var usageLocation = await _dbContext.UsageLocations.SingleAsync(x => x.GeographyID == geographyID && x.Name == usageLocationName && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var depthInFeet = updatedAmountInAcreFeetAsDecimal / (decimal)usageLocation.Area;
        var updatedAmountInInches = depthInFeet / 12;

        #endregion

        #region Previous Values

        var etEvapotranspirationMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etEvapotranspirationMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousETEvapotranspirationValueInAcreFeet = etEvapotranspirationMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousConsumedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterMeasurementType && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousConsumedGroundwaterValueInAcreFeet = previousConsumedGroundwaterMeasurement?.ReportedValueInAcreFeet ?? 0;

        #endregion

        #region Upsert ET Evapotranspiration and Run Calculations

        if (etEvapotranspirationMeasurement == null)
        {
            etEvapotranspirationMeasurement = new WaterMeasurement()
            {
                GeographyID = geographyID,
                UsageLocationID = usageLocation.UsageLocationID,
                WaterMeasurementTypeID = etEvapotranspirationMeasurementTypeID,
                UnitTypeID = UnitType.Inches.UnitTypeID,
                ReportedDate = dateToCalculate,
                ReportedValueInNativeUnits = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                ReportedValueInFeet = depthInFeet,
                LastUpdateDate = DateTime.UtcNow,
            };

            _dbContext.WaterMeasurements.Add(etEvapotranspirationMeasurement);
        }
        else
        {
            etEvapotranspirationMeasurement.ReportedValueInNativeUnits = updatedAmountInInches;
            etEvapotranspirationMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
            etEvapotranspirationMeasurement.ReportedValueInFeet = depthInFeet;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(_dbContext, geographyID, etEvapotranspirationMeasurementTypeID, dateToCalculate);

        #endregion

        #region Check ET Evapotranspiration

        var updatedETEvapotranspirationMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etEvapotranspirationMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedETEvapotranspirationMeasurement);

        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedETEvapotranspirationMeasurement.ReportedValueInAcreFeet);

        #endregion

        #region Check Consumed Groundwater

        var eligibleInchesOfRainMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == eligibleInchesOfRain && x.UsageLocationID == usageLocation.UsageLocationID);
        var updatedConsumedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterMeasurementType && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedConsumedGroundwaterMeasurement);

        var expectedConsumedGroundwaterValueInAcreFeet = Math.Round(updatedAmountInAcreFeetAsDecimal - (eligibleInchesOfRainMeasurement?.ReportedValueInAcreFeet ?? 0), 4, MidpointRounding.ToEven);

        var updatedConsumedGroundwaterValueInAcreFeet = Math.Round(updatedConsumedGroundwaterMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedConsumedGroundwaterValueInAcreFeet - updatedConsumedGroundwaterValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedConsumedGroundwaterValueInAcreFeet}. Got: {updatedConsumedGroundwaterValueInAcreFeet}");

        #endregion
    }

    [TestMethod]
    [DataRow("20230731", "049-150-001", 1000.0000d)]
    [Description("MSGSA - After updating the Eligible Inches of Rain it should reflect in all the dependant calculations.")]
    public async Task MSGSA_CanUpdateEligibleInchesOfRainAndHaveDependantCalculationsModified(string dateAsString, string usageLocationName, double updatedAmountInAcreFeetAsDouble)
    {
        #region Hard Coded Assumptions

        var geographyID = 6; //6 == MSGSA
        var etEvapotranspirationMeasurementTypeID = 6; //6==ETEvapotranspiration
        var eligibleInchesOfRainMeasurementTypeID = 52; //52==Eligible Inches of Rain
        var consumedGroundwaterMeasurementTypeID = 20; //20==ConsumedGroundwater

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(_dbContext, geographyID, dateToCalculate.Year);
        var usageLocation = await _dbContext.UsageLocations.SingleAsync(x => x.GeographyID == geographyID && x.Name == usageLocationName && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var depthInFeet = updatedAmountInAcreFeetAsDecimal / (decimal)usageLocation.Area;
        var updatedAmountInInches = depthInFeet / 12;

        #endregion

        #region Previous Values

        var eligibleInchesOfRainMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == eligibleInchesOfRainMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousETPrecipValueInAcreFeet = eligibleInchesOfRainMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousConsumedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousConsumedGroundwaterValueInAcreFeet = previousConsumedGroundwaterMeasurement?.ReportedValueInAcreFeet ?? 0;

        #endregion

        #region Upsert Eligible Inches of Rain and Run Calculations

        if (eligibleInchesOfRainMeasurement == null)
        {
            eligibleInchesOfRainMeasurement = new WaterMeasurement()
            {
                GeographyID = geographyID,
                UsageLocationID = usageLocation.UsageLocationID,
                WaterMeasurementTypeID = eligibleInchesOfRainMeasurementTypeID,
                UnitTypeID = UnitType.Inches.UnitTypeID,
                ReportedDate = dateToCalculate,
                ReportedValueInNativeUnits = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                ReportedValueInFeet = depthInFeet,
                LastUpdateDate = DateTime.UtcNow,
            };

            _dbContext.WaterMeasurements.Add(eligibleInchesOfRainMeasurement);
        }
        else
        {
            eligibleInchesOfRainMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(_dbContext, geographyID, eligibleInchesOfRainMeasurementTypeID, dateToCalculate);

        #endregion

        #region Check EligibleInchesOfRain

        var updatedEligibleInchesOfRainMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == eligibleInchesOfRainMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedEligibleInchesOfRainMeasurement);

        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedEligibleInchesOfRainMeasurement.ReportedValueInAcreFeet);

        #endregion

        #region Check Consumptive Use

        var etEvapotranspirationMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etEvapotranspirationMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        var updatedConsumedGroundwaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedConsumedGroundwaterMeasurement);

        var expectedConsumedGroundwaterValueInAcreFeet = Math.Round(etEvapotranspirationMeasurement.ReportedValueInAcreFeet - updatedAmountInAcreFeetAsDecimal, 4, MidpointRounding.ToEven);

        var updatedConsumedGroundwaterValueInAcreFeet = Math.Round(updatedConsumedGroundwaterMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedConsumedGroundwaterValueInAcreFeet - updatedConsumedGroundwaterValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedConsumedGroundwaterValueInAcreFeet}. Got: {updatedConsumedGroundwaterValueInAcreFeet}");

        #endregion
    }

    #endregion

    #region ETSGSA Specific Tests 

    [TestMethod]
    [DataRow("20240331", "ETSGSA-9318-1297-2024", 1000.0000d)]
    [Description("ETSGSA - After updating the LandIQ Precip it should reflect in all the dependant calculations.")]
    public async Task ETSGSA_CanUpdateLandIQPrecipAndHaveDependantCalculationsModified(string dateAsString, string usageLocationName, double updatedAmountInAcreFeetAsDouble)
    {
        #region Hard Coded Assumptions

        var geographyID = 7; //7 == ETSGSA
        var landIQPrecipMeasurementTypeID = 22; //22==LandIQPrecip
        var effectivePrecipMeasurementTypeID = 34; //34==EffectivePrecip

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(_dbContext, geographyID, dateToCalculate.Year);
        var usageLocation = await _dbContext.UsageLocations.Include(usageLocation => usageLocation.UsageLocationType).SingleAsync(x => x.GeographyID == geographyID && x.Name == usageLocationName && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var depthInFeet = updatedAmountInAcreFeetAsDecimal / (decimal)usageLocation.Area;
        var updatedAmountInMillimeters = UnitConversionHelper.ConvertAcreFeetToMillimeters(updatedAmountInAcreFeetAsDecimal, (decimal)usageLocation.Area);

        #endregion

        #region Previous Values

        var landIQPrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == landIQPrecipMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousLandIQPrecipValueInAcreFeet = landIQPrecipMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousEffectivePrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousEffectivePrecipValueInAcreFeet = previousEffectivePrecipMeasurement?.ReportedValueInAcreFeet ?? 0;

        #endregion

        #region Upsert LandIQ Precip and Run Calculations

        if (landIQPrecipMeasurement == null)
        {
            landIQPrecipMeasurement = new WaterMeasurement()
            {
                GeographyID = geographyID,
                WaterMeasurementTypeID = landIQPrecipMeasurementTypeID,
                UnitTypeID = UnitType.Millimeters.UnitTypeID,
                ReportedDate = dateToCalculate,
                ReportedValueInNativeUnits = updatedAmountInMillimeters,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                ReportedValueInFeet = depthInFeet,
                LastUpdateDate = DateTime.UtcNow,
            };

            _dbContext.WaterMeasurements.Add(landIQPrecipMeasurement);
        }
        else
        {
            landIQPrecipMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(_dbContext, geographyID, landIQPrecipMeasurementTypeID, dateToCalculate);

        #endregion

        #region Check Precip

        var updatedLandIQPrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == landIQPrecipMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        Assert.IsNotNull(updatedLandIQPrecipMeasurement);
        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedLandIQPrecipMeasurement.ReportedValueInAcreFeet);

        #endregion

        #region Check Effective Precip

        var effectivePrecipMeasurementType = await _dbContext.WaterMeasurementTypes.SingleOrDefaultAsync(x => x.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID);
        Assert.IsNotNull(effectivePrecipMeasurementType, $"Effective Precip Measurement Type with ID {effectivePrecipMeasurementTypeID} not found.");

        var calculationJSON = JsonSerializer.Deserialize<EffectivePrecipitationCalculationDto>(effectivePrecipMeasurementType.CalculationJSON);

        var effectivePrecipMultiplier = usageLocation.UsageLocationType.WaterMeasurementTypeID.HasValue 
            ? calculationJSON.CoverCropEffectivePrecipitationMultiplier 
            : calculationJSON.EffectivePrecipitationMultiplier;

        var expectedEffectivePrecipValueInAcreFeet = Math.Round(updatedAmountInAcreFeetAsDecimal * effectivePrecipMultiplier, 4, MidpointRounding.ToEven);

        var updatedEffectivePrecipMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        Assert.IsNotNull(updatedEffectivePrecipMeasurement);
        Assert.IsTrue(Math.Abs(expectedEffectivePrecipValueInAcreFeet - updatedEffectivePrecipMeasurement.ReportedValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedEffectivePrecipValueInAcreFeet}. Got: {updatedEffectivePrecipMeasurement.ReportedValueInAcreFeet}");

        #endregion

        #region Check ETMinusPrecipMinusTotalSurfaceWater

        var evapotranspirationWaterMeasurement = await _dbContext.WaterMeasurements
            .Where(x => x.WaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.ET.WaterMeasurementCategoryTypeID)
            .FirstOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageLocationID == usageLocation.UsageLocationID);

        var surfaceWaterTypeMeasurements = await _dbContext.WaterMeasurements
            .Where(x => x.WaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.CalculateSurfaceWaterConsumption.WaterMeasurementCalculationTypeID || x.WaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.SurfaceWater.WaterMeasurementCategoryTypeID)
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageLocationID == usageLocation.UsageLocationID)
            .ToListAsync();

        var surfaceWaterTotalInAcreFeet = surfaceWaterTypeMeasurements.Sum(x => x.ReportedValueInAcreFeet);

        var expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet = Math.Round((evapotranspirationWaterMeasurement.ReportedValueInAcreFeet - updatedEffectivePrecipMeasurement.ReportedValueInAcreFeet - surfaceWaterTotalInAcreFeet), 4, MidpointRounding.ToEven);

        var updatedETMinusPrecipMinusTotalSurfaceWater = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageLocationID == usageLocation.UsageLocationID && x.WaterMeasurementType.WaterMeasurementCalculationTypeID == WaterMeasurementCalculationType.ETMinusPrecipMinusTotalSurfaceWater.WaterMeasurementCalculationTypeID);

        var updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet = Math.Round(updatedETMinusPrecipMinusTotalSurfaceWater.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet - updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet}. Got: {updatedETMinusPrecipMinusTotalSurfaceWaterInAcreFeet}");

        #endregion
    }

    [TestMethod]
    [DataRow("20240331", "ETSGSA-9318-1297-2024", 1000.0000d)]
    [Description("ETSGSA - After updating the LandIQ ET it should reflect in all the dependant calculations.")]
    public async Task ETSGSA_CanUpdateLandETAndHaveDependantCalculationsModified(string dateAsString, string usageLocationName, double updatedAmountInAcreFeetAsDouble)
    {
        #region Hard Coded Assumptions

        var geographyID = 7; //7 == ETSGSA
        var landIQETMeasurementTypeID = 21; //21==LandIQ ET
        var effectivePrecipMeasurementTypeID = 34; //34==effectivePrecip
        var consumedGroundwaterMeasurementTypeID = 33; //33==ConsumedGroundwater

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(_dbContext, geographyID, dateToCalculate.Year);
        var usageLocation = await _dbContext.UsageLocations.SingleAsync(x => x.GeographyID == geographyID && x.Name == usageLocationName && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var depthInFeet = updatedAmountInAcreFeetAsDecimal / (decimal)usageLocation.Area;
        var updatedAmountInMillimeters = UnitConversionHelper.ConvertAcreFeetToMillimeters(updatedAmountInAcreFeetAsDecimal, (decimal)usageLocation.Area);

        #endregion

        #region Previous Values

        var landIQETMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == landIQETMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        var previousLandIQETValueInAcreFeet = landIQETMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousConsumedGroundwaterMeasurement = await _dbContext.WaterMeasurements.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        var previousConsumedGroundwaterValueInAcreFeet = previousConsumedGroundwaterMeasurement?.ReportedValueInAcreFeet ?? 0;

        #endregion

        #region Upsert LandIQ ET and Run Calculations

        if (landIQETMeasurement == null)
        {
            landIQETMeasurement = new WaterMeasurement()
            {
                GeographyID = geographyID,
                UsageLocationID = usageLocation.UsageLocationID,
                WaterMeasurementTypeID = landIQETMeasurementTypeID,
                UnitTypeID = UnitType.Millimeters.UnitTypeID,
                ReportedDate = dateToCalculate,
                ReportedValueInNativeUnits = updatedAmountInMillimeters,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                ReportedValueInFeet = depthInFeet,
                LastUpdateDate = DateTime.UtcNow,
            };

            _dbContext.WaterMeasurements.Add(landIQETMeasurement);
        }
        else
        {
            landIQETMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(_dbContext, geographyID, landIQETMeasurementTypeID, dateToCalculate);

        #endregion

        #region Check ET

        var updatedLandIQETMeasurement = await _dbContext.WaterMeasurements.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == landIQETMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedLandIQETMeasurement);

        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedLandIQETMeasurement.ReportedValueInAcreFeet);

        #endregion

        #region Check Consumed Groundwater

        var precipMeasurement = await _dbContext.WaterMeasurements.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementType.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        var previousPrecipInAcreFeet = precipMeasurement?.ReportedValueInAcreFeet ?? 0;

        var surfaceWaterMeasurements = await _dbContext.WaterMeasurements.AsNoTracking()
            .Include(x => x.WaterMeasurementType)
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageLocationID == usageLocation.UsageLocationID && x.WaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.SurfaceWater.WaterMeasurementCategoryTypeID)
            .ToListAsync();

        var surfaceWaterTotalInAcreFeet = surfaceWaterMeasurements.Sum(x => x.ReportedValueInAcreFeet);

        var consumedGroundwaterMeasurement = await _dbContext.WaterMeasurements.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(consumedGroundwaterMeasurement);

        var expectedConsumedGroundwaterValueInAcreFeet = Math.Round((updatedLandIQETMeasurement.ReportedValueInAcreFeet - previousPrecipInAcreFeet - surfaceWaterTotalInAcreFeet), 4, MidpointRounding.ToEven);

        var expectedConsumedGroundwaterValueInInches = Math.Round(UnitConversionHelper.ConvertAcreFeetToMillimeters(expectedConsumedGroundwaterValueInAcreFeet, (decimal)usageLocation.Area), 4, MidpointRounding.ToEven);

        var updatedConsumedGroundwaterValueInAcreFeet = Math.Round(consumedGroundwaterMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedConsumedGroundwaterValueInAcreFeet - updatedConsumedGroundwaterValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedConsumedGroundwaterValueInAcreFeet}. Got: {updatedConsumedGroundwaterValueInAcreFeet}");

        #endregion
    }


    [TestMethod]
    [DataRow("20240331", "ETSGSA-9318-1297-2024", 1000.0000d, 28)]
    [DataRow("20240331", "ETSGSA-9318-1297-2024", 1000.0000d, 29)]
    [DataRow("20240331", "ETSGSA-9318-1297-2024", 1000.0000d, 30)]
    [DataRow("20240331", "ETSGSA-9318-1297-2024", 1000.0000d, 31)]
    [DataRow("20240331", "ETSGSA-9318-1297-2024", 1000.0000d, 32)]
    [Description("ETSGSA - After updating a measurement of category surface water, it should reflect in all the dependant calculations.")]
    public async Task ETSGSA_CanUpdateSurfaceWaterAndHaveDependantCalculationsModified(string dateAsString, string usageLocationName, double updatedAmountInAcreFeetAsDouble, int surfaceWaterMeasurementTypeID)
    {
        #region Hard Coded Assumptions

        var geographyID = 7; //7 == ETSGSA
        var landIQETMeasurementTypeID = 21; //21==LandIQ ET
        var effectivePrecipMeasurementTypeID = 34; //34==effectivePrecip
        var consumedGroundwaterMeasurementTypeID = 33; //33==ConsumedGroundwater

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(_dbContext, geographyID, dateToCalculate.Year);
        var usageLocation = await _dbContext.UsageLocations.SingleAsync(x => x.GeographyID == geographyID && x.Name == usageLocationName && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID);
        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var depthInFeet = updatedAmountInAcreFeetAsDecimal / (decimal)usageLocation.Area;
        var updatedAmountInMillimeters = UnitConversionHelper.ConvertAcreFeetToMillimeters(updatedAmountInAcreFeetAsDecimal, (decimal)usageLocation.Area);

        #endregion

        #region Previous Values

        var surfaceWaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == surfaceWaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        var previousSurfaceWaterValueInAcreFeet = surfaceWaterMeasurement?.ReportedValueInAcreFeet ?? 0;

        #endregion

        #region Upsert Surface Water and Run Calculations

        if (surfaceWaterMeasurement == null)
        {
            surfaceWaterMeasurement = new WaterMeasurement()
            {
                GeographyID = geographyID,
                UsageLocationID = usageLocation.UsageLocationID,
                WaterMeasurementTypeID = surfaceWaterMeasurementTypeID,
                UnitTypeID = UnitType.Millimeters.UnitTypeID,
                ReportedDate = dateToCalculate,
                ReportedValueInNativeUnits = updatedAmountInMillimeters,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                ReportedValueInFeet = depthInFeet,
                LastUpdateDate = DateTime.UtcNow,
            };

            _dbContext.WaterMeasurements.Add(surfaceWaterMeasurement);
        }
        else
        {
            surfaceWaterMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(_dbContext, geographyID, surfaceWaterMeasurementTypeID, dateToCalculate);

        #endregion

        #region Check Surface Water

        var updatedSurfaceWaterMeasurement = await _dbContext.WaterMeasurements.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == surfaceWaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedSurfaceWaterMeasurement);

        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedSurfaceWaterMeasurement.ReportedValueInAcreFeet);

        #endregion

        #region Check Consumed Groundwater

        var updatedLandIQETMeasurement = await _dbContext.WaterMeasurements.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == landIQETMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedLandIQETMeasurement);

        var precipMeasurement = await _dbContext.WaterMeasurements.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementType.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        var previousPrecipInAcreFeet = precipMeasurement?.ReportedValueInAcreFeet ?? 0;

        var surfaceWaterMeasurements = await _dbContext.WaterMeasurements.AsNoTracking()
            .Include(x => x.WaterMeasurementType)
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageLocationID == usageLocation.UsageLocationID && x.WaterMeasurementType.WaterMeasurementCategoryTypeID == WaterMeasurementCategoryType.SurfaceWater.WaterMeasurementCategoryTypeID)
            .ToListAsync();

        var surfaceWaterTotalInAcreFeet = surfaceWaterMeasurements.Sum(x => x.ReportedValueInAcreFeet);

        var consumedGroundwaterMeasurement = await _dbContext.WaterMeasurements.AsNoTracking()
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(consumedGroundwaterMeasurement);

        var expectedConsumedGroundwaterValueInAcreFeet = Math.Round((updatedLandIQETMeasurement.ReportedValueInAcreFeet - previousPrecipInAcreFeet - surfaceWaterTotalInAcreFeet), 4, MidpointRounding.ToEven);

        var updatedConsumedGroundwaterValueInAcreFeet = Math.Round(consumedGroundwaterMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedConsumedGroundwaterValueInAcreFeet - updatedConsumedGroundwaterValueInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedConsumedGroundwaterValueInAcreFeet}. Got: {updatedConsumedGroundwaterValueInAcreFeet}");

        #endregion
    }

    #endregion

    #region SCNY Specific Tests

    [TestMethod]
    [DataRow("20250131", "000-000-000", 1000.0000d)]
    [Description("SCNY - After updating the ET Precip it should reflect in all the dependant calculations.")]
    public async Task SCNY_CanUpdateMeasurementsAndHaveDependantCalculationsModified(string dateAsString, string usageLocationName, double updatedAmountInAcreFeetAsDouble)
    {
        #region Hard Coded Assumptions

        var geographyID = 8; //8 == SCNY
        var etEvapotranspirationMeasurementTypeID = 57;
        var etPrecipMeasurementType = 58;
        var effectivePrecipMeasurementTypeID = 59;
        var deliveredSurfaceWaterMeasurementTypeID = 60;
        var consumedSurfaceWaterMeasurementTypeID = 61;
        var consumedGroundwaterTypeID = 62;

        #endregion

        #region Inputs

        var dateToCalculate = DateTime.ParseExact(dateAsString, "yyyyMMdd", null);
        var reportingPeriod = await ReportingPeriods.GetByGeographyIDAndYearAsync(_dbContext, geographyID, dateToCalculate.Year);
        var usageLocation = await _dbContext.UsageLocations.SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.Name == usageLocationName && x.ReportingPeriodID == reportingPeriod.ReportingPeriodID);
        if (usageLocation == null)
        {
            var parcel = await _dbContext.Parcels.FirstOrDefaultAsync(x => x.GeographyID == geographyID);
            var defaultUsageLocationType = await _dbContext.UsageLocationTypes.FirstOrDefaultAsync(x => x.IsDefault && x.GeographyID == geographyID);
            usageLocation = new UsageLocation()
            {
                GeographyID = geographyID,
                ParcelID = parcel.ParcelID,
                ReportingPeriodID = reportingPeriod.ReportingPeriodID,
                UsageLocationTypeID = defaultUsageLocationType.UsageLocationTypeID,
                Name = usageLocationName,
                Area = 100,
                CreateUserID = Users.QanatSystemAdminUserID,
                CreateDate = DateTime.UtcNow,
            };

            await _dbContext.UsageLocations.AddAsync(usageLocation);
            await _dbContext.SaveChangesAsync();
        }

        var updatedAmountInAcreFeetAsDecimal = Math.Round((decimal)updatedAmountInAcreFeetAsDouble, 4, MidpointRounding.ToEven);
        var depthInFeet = updatedAmountInAcreFeetAsDecimal / (decimal)usageLocation.Area;
        var updatedAmountInInches = depthInFeet / 12;

        #endregion

        #region Cleanup to ensure multiple runs are smooth

        await _dbContext.WaterMeasurements
            .Where(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.UsageLocationID == usageLocation.UsageLocationID && (x.WaterMeasurementTypeID == deliveredSurfaceWaterMeasurementTypeID || x.WaterMeasurementTypeID == consumedSurfaceWaterMeasurementTypeID))
            .ExecuteDeleteAsync();

        #endregion

        #region Previous Values

        var previousETEvapotranspirationMeasurement = await _dbContext.WaterMeasurements
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etEvapotranspirationMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousETEvapotranspirationValueInAcreFeet = previousETEvapotranspirationMeasurement?.ReportedValueInAcreFeet ?? 0;

        var etPrecipMeasurement = await _dbContext.WaterMeasurements
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etPrecipMeasurementType && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousETPrecipValueInAcreFeet = etPrecipMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousEffectivePrecipMeasurement = await _dbContext.WaterMeasurements
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousEffectivePrecipValueInAcreFeet = previousEffectivePrecipMeasurement?.ReportedValueInAcreFeet ?? 0;

        var previousConsumedGroundwaterMeasurement = await _dbContext.WaterMeasurements
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        var previousConsumptiveUseValueInAcreFeet = previousConsumedGroundwaterMeasurement?.ReportedValueInAcreFeet ?? 0;

        #endregion

        #region Upsert ET Evapo and  ET Precip and Run Calculations

        if (previousETEvapotranspirationMeasurement == null)
        {
            previousETEvapotranspirationMeasurement = new WaterMeasurement()
            {
                GeographyID = geographyID,
                UsageLocationID = usageLocation.UsageLocationID,
                WaterMeasurementTypeID = etEvapotranspirationMeasurementTypeID,
                UnitTypeID = UnitType.Inches.UnitTypeID,
                ReportedDate = dateToCalculate,
                ReportedValueInNativeUnits = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                ReportedValueInFeet = depthInFeet,
                LastUpdateDate = DateTime.UtcNow,
            };

            _dbContext.WaterMeasurements.Add(previousETEvapotranspirationMeasurement);
        }

        if (etPrecipMeasurement == null)
        {
            etPrecipMeasurement = new WaterMeasurement()
            {
                GeographyID = geographyID,
                UsageLocationID = usageLocation.UsageLocationID,
                WaterMeasurementTypeID = etPrecipMeasurementType,
                UnitTypeID = UnitType.Inches.UnitTypeID,
                ReportedDate = dateToCalculate,
                ReportedValueInNativeUnits = updatedAmountInInches,
                ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal,
                ReportedValueInFeet = depthInFeet,
                LastUpdateDate = DateTime.UtcNow,
            };

            _dbContext.WaterMeasurements.Add(etPrecipMeasurement);
        }
        else
        {
            etPrecipMeasurement.ReportedValueInNativeUnits = updatedAmountInInches;
            etPrecipMeasurement.ReportedValueInAcreFeet = updatedAmountInAcreFeetAsDecimal;
            etPrecipMeasurement.ReportedValueInFeet = depthInFeet;
        }

        await _dbContext.SaveChangesAsync();
        await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(_dbContext, geographyID, etPrecipMeasurementType, dateToCalculate);

        #endregion

        #region Check ET Precip

        var updatedETPrecipMeasurement = await _dbContext.WaterMeasurements
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == etPrecipMeasurementType && x.UsageLocationID == usageLocation.UsageLocationID);

        Assert.IsNotNull(updatedETPrecipMeasurement);
        Assert.AreEqual(updatedAmountInAcreFeetAsDecimal, updatedETPrecipMeasurement.ReportedValueInAcreFeet);

        #endregion

        #region Check Effective Precip

        var effectivePrecipMeasurement = await _dbContext.WaterMeasurements
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        Assert.IsNotNull(effectivePrecipMeasurement);

        var effectivePrecipMeasurementType = await _dbContext.WaterMeasurementTypes.SingleAsync(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == effectivePrecipMeasurementTypeID);

        var calculationJSON = System.Text.Json.JsonSerializer.Deserialize<WaterMeasurementCalculations.EffectivePrecipitationCalculationDto>(effectivePrecipMeasurementType.CalculationJSON);

        var precipitationMultiplier = calculationJSON.EffectivePrecipitationMultiplier;
        Assert.IsNotNull(precipitationMultiplier);

        var expectedEffectivePrecipMeasurementInAcreFeet = Math.Round(updatedAmountInAcreFeetAsDecimal * precipitationMultiplier, 4, MidpointRounding.ToEven);
        var updatedEffectivePrecipMeasurementInAcreFeet = Math.Round(effectivePrecipMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedEffectivePrecipMeasurementInAcreFeet - updatedEffectivePrecipMeasurementInAcreFeet) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedEffectivePrecipMeasurementInAcreFeet}. Got: {updatedEffectivePrecipMeasurementInAcreFeet}");

        #endregion

        #region Check Consumed Groundwater

        var updatedConsumedGroundwaterMeasurement = await _dbContext.WaterMeasurements
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        Assert.IsNotNull(effectivePrecipMeasurement);

        var expectedConsumedGroundwater = Math.Round(previousETEvapotranspirationMeasurement.ReportedValueInAcreFeet - effectivePrecipMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);
        var updatedConsumedGroundwater = Math.Round(updatedConsumedGroundwaterMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedConsumedGroundwater - updatedConsumedGroundwater) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedConsumedGroundwater}. Got: {updatedConsumedGroundwater}");

        #endregion


        #region Upsert Delivered Surface Water and Run Calculations

        var previousDeliveredWaterMeasurement = await _dbContext.WaterMeasurements
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == deliveredSurfaceWaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        var deliveredSurfaceWaterVolume = 100m;
        var deliveredSurfaceWaterDepth = deliveredSurfaceWaterVolume / (decimal)usageLocation.Area;
        if (previousDeliveredWaterMeasurement == null)
        {

            previousDeliveredWaterMeasurement = new WaterMeasurement()
            {
                GeographyID = geographyID,
                UsageLocationID = usageLocation.UsageLocationID,
                WaterMeasurementTypeID = deliveredSurfaceWaterMeasurementTypeID,
                ReportedDate = dateToCalculate,
                ReportedValueInAcreFeet = deliveredSurfaceWaterVolume,
                ReportedValueInFeet = deliveredSurfaceWaterDepth,
                LastUpdateDate = DateTime.UtcNow
            };

            await _dbContext.WaterMeasurements.AddAsync(previousDeliveredWaterMeasurement);
        }
        else
        {
            previousDeliveredWaterMeasurement.ReportedValueInAcreFeet = deliveredSurfaceWaterVolume;
            previousDeliveredWaterMeasurement.ReportedValueInFeet = deliveredSurfaceWaterDepth;
        }

        await _dbContext.SaveChangesAsync();

        await WaterMeasurementCalculations.RunMeasurementTypeForGeographyAsync(_dbContext, geographyID, deliveredSurfaceWaterMeasurementTypeID, dateToCalculate);

        #endregion

        #region Check Consumed Surface Water and Consumed Groundwater

        var consumedSurfaceWaterMeasurementType = await _dbContext.WaterMeasurementTypes
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.WaterMeasurementTypeID == consumedSurfaceWaterMeasurementTypeID);

        var surfaceWaterConsumptionCalculationDto = System.Text.Json.JsonSerializer.Deserialize<WaterMeasurementCalculations.SurfaceWaterConsumptionCalculationDto>(consumedSurfaceWaterMeasurementType.CalculationJSON);

        var surfaceWaterEfficiencyFactor = surfaceWaterConsumptionCalculationDto.SurfaceWaterEfficiencyFactor;
        Assert.IsNotNull(surfaceWaterEfficiencyFactor);

        var consumedSurfaceWaterMeasurement = await _dbContext.WaterMeasurements
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedSurfaceWaterMeasurementTypeID && x.UsageLocationID == usageLocation.UsageLocationID);
        Assert.IsNotNull(consumedSurfaceWaterMeasurement);

        var expectedConsumedSurfaceWater = Math.Round(previousDeliveredWaterMeasurement.ReportedValueInAcreFeet * surfaceWaterEfficiencyFactor, 4, MidpointRounding.ToEven);
        var updatedConsumedSurfaceWater = Math.Round(consumedSurfaceWaterMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedConsumedSurfaceWater - updatedConsumedSurfaceWater) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedConsumedSurfaceWater}. Got: {updatedConsumedSurfaceWater}");

        updatedConsumedGroundwaterMeasurement = await _dbContext.WaterMeasurements
            .SingleOrDefaultAsync(x => x.GeographyID == geographyID && x.ReportedDate.Date == dateToCalculate.Date && x.WaterMeasurementTypeID == consumedGroundwaterTypeID && x.UsageLocationID == usageLocation.UsageLocationID);

        var expectedConsumedGroundwaterAfterSurfaceWater = Math.Round(previousETEvapotranspirationMeasurement.ReportedValueInAcreFeet - effectivePrecipMeasurement.ReportedValueInAcreFeet - consumedSurfaceWaterMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);
        var updatedConsumedGroundwaterAfterSurfaceWater = Math.Round(updatedConsumedGroundwaterMeasurement.ReportedValueInAcreFeet, 4, MidpointRounding.ToEven);

        Assert.IsTrue(Math.Abs(expectedConsumedGroundwaterAfterSurfaceWater - updatedConsumedGroundwaterAfterSurfaceWater) <= _acceptableTolerance, $"Reported Value in Acre Feet do not match within tolerance. Expected: {expectedConsumedGroundwaterAfterSurfaceWater}. Got: {updatedConsumedGroundwaterAfterSurfaceWater}");

        #endregion
    }

    #endregion
}