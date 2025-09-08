using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qanat.API.Helpers.FeeCalculator;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;

namespace Qanat.Tests.IntegrationTests.FeeCalculator;

[TestClass]
public class FeeCalculatorTests
{
    private static QanatDbContext _dbContext;

    private static readonly decimal _tolerance = .1m;

    [TestInitialize]
    public void TestInitialize()
    {
        var dbCS = AssemblySteps.Configuration["sqlConnectionString"];
        _dbContext = new QanatDbContext(dbCS);
    }

    [TestMethod]
    [DataRow("etsgsa", 7214, 46)] //46 = ETSGSA 2024
    public async Task CanCalculateFeeWithoutIncentives(string geographyName, int waterAccountID, int reportingPeriodID)
    {
        var geography = Geographies.GetByNameAsPublicDto(_dbContext, geographyName);
        var feeStructures = FeeStructuresDtos.ETSGSA_FeeStructures;
        var selectedFeeStructure = feeStructures.First();

        var callingUser = new UserDto()
        {
            Flags = new Dictionary<string, bool>() { { Flag.IsSystemAdmin.FlagName, true } }
        };

        var reportingPeriods = await ReportingPeriods.ListByGeographyIDAsync(_dbContext, geography.GeographyID, callingUser);
        var reportingPeriod = reportingPeriods.First(x => x.ReportingPeriodID == reportingPeriodID);
        var input = new FeeCalculatorInputDto()
        {
            WaterAccountID = waterAccountID,
            ReportingPeriodID = reportingPeriod.ReportingPeriodID,
            FeeStructureID = selectedFeeStructure.FeeStructureID,
            SurfaceWaterDelivered = 3,
            SurfaceWaterIrrigationEfficiency = 90, //Entered as a percentage
            MLRPIncentives = new List<MLRPIncentiveDto>()
        };

        var output = await FeeCalculatorHelper.CalculateFee(_dbContext, geography, input);

        Assert.IsNotNull(output);
        Assert.IsNotNull(output.BaselineScenario);

        var prettyPrintedResultJSON = JsonSerializer.Serialize(output, new JsonSerializerOptions { WriteIndented = true });

        Console.WriteLine(prettyPrintedResultJSON);

        var waterAccount = WaterAccounts.GetByIDAsDto(_dbContext, waterAccountID, reportingPeriodID);
        CheckScenario(waterAccount, selectedFeeStructure, output.BaselineScenario, new List<MLRPIncentiveDto>());
        CheckScenario(waterAccount, selectedFeeStructure, output.LandUseChangeScenario, new List<MLRPIncentiveDto>());

        AssertScenariosAreEquivalent(output.BaselineScenario, output.LandUseChangeScenario);

        Assert.AreEqual(0, output.SavingsAndIncentives.FeeReductionTotal);
        Assert.AreEqual(0, output.SavingsAndIncentives.FeeReductionPerAcreFoot);
        Assert.AreEqual(0, output.SavingsAndIncentives.FeeReductionPerParcelAcre);

        Assert.AreEqual(0, output.SavingsAndIncentives.MLRPIncentiveTotal);
        Assert.AreEqual(0, output.SavingsAndIncentives.MLRPIncentiveBreakdown.Count);
    }

    [TestMethod]
    [DataRow("etsgsa", 7214, 46)] //46 = ETSGSA 2024
    public async Task CanCalculateFeeWithIncentives(string geographyName, int waterAccountID, int reportingPeriodID)
    {
        var geography = Geographies.GetByNameAsPublicDto(_dbContext, geographyName);
        var feeStructures = FeeStructuresDtos.ETSGSA_FeeStructures;
        var selectedFeeStructure = feeStructures.First();

        var coverCropIncentive = MLRPIncentiveDtos.ETSGSA_MLRPIncentives.First(x => x.Name == "Cover Cropping (Self-Directed)");
        coverCropIncentive.Acres = 100;

        var orchardIncentive = MLRPIncentiveDtos.ETSGSA_MLRPIncentives.First(x => x.Name == "MLRP Orchard Swale Rewilding");
        orchardIncentive.Acres = 100;

        var floodFlowIncentive = MLRPIncentiveDtos.ETSGSA_MLRPIncentives.First(x => x.Name == "MLRP Flood Flow Spreading");
        floodFlowIncentive.Acres = 100;

        var incentives = new List<MLRPIncentiveDto>()
        {
            coverCropIncentive,
            orchardIncentive,
            floodFlowIncentive
        };

        var callingUser = new UserDto()
        {
            Flags = new Dictionary<string, bool>() { { Flag.IsSystemAdmin.FlagName, true } }
        };

        var reportingPeriods = await ReportingPeriods.ListByGeographyIDAsync(_dbContext, geography.GeographyID, callingUser);
        var reportingPeriod = reportingPeriods.First(x => x.ReportingPeriodID == reportingPeriodID);

        var input = new FeeCalculatorInputDto()
        {
            WaterAccountID = waterAccountID,
            ReportingPeriodID = reportingPeriod.ReportingPeriodID,
            FeeStructureID = selectedFeeStructure.FeeStructureID,
            SurfaceWaterDelivered = 3,
            SurfaceWaterIrrigationEfficiency = 90, //Entered as a percentage
            MLRPIncentives = incentives
        };

        var output = await FeeCalculatorHelper.CalculateFee(_dbContext, geography, input);

        Assert.IsNotNull(output);
        Assert.IsNotNull(output.BaselineScenario);

        var prettyPrintedResultJSON = JsonSerializer.Serialize(output, new JsonSerializerOptions { WriteIndented = true });

        Console.WriteLine(prettyPrintedResultJSON);

        var waterAccount = WaterAccounts.GetByIDAsDto(_dbContext, waterAccountID, reportingPeriodID);
        CheckScenario(waterAccount, selectedFeeStructure, output.BaselineScenario, new List<MLRPIncentiveDto>());
        CheckScenario(waterAccount, selectedFeeStructure, output.LandUseChangeScenario, incentives);

        CheckSavingsAndIncentives(output.BaselineScenario, output.LandUseChangeScenario, incentives);
    }


    private void CheckScenario(WaterAccountDto waterAccount, FeeStructureDto selectedFeeStructure, FeeCalculatorOutputScenarioDto scenario, List<MLRPIncentiveDto> incentivesToApply)
    {
        if (incentivesToApply.Any())
        {
            var parcelAcres = ((decimal)waterAccount.Acres).Round(4);
            var expectedAcres = parcelAcres - incentivesToApply.Where(x => !x.KeepsAllocation).Sum(x => x.Acres).GetValueOrDefault(0);
            Assert.AreEqual(expectedAcres, scenario.Acres);

            var irrigatedAcres = ((decimal)waterAccount.IrrigatedAcres).Round(4);
            var expectedIrrigatedAcres = irrigatedAcres - incentivesToApply.Where(x => !x.KeepIrrigateAcres).Sum(x => x.Acres).GetValueOrDefault(0);
            Assert.AreEqual(expectedIrrigatedAcres, scenario.IrrigatedAcres);
        }
        else
        {
            var parcelAcres = ((decimal)waterAccount.Acres).Round(4);
            Assert.AreEqual(parcelAcres, scenario.Acres);

            var irrigatedAcres = ((decimal)waterAccount.IrrigatedAcres).Round(4);
            Assert.AreEqual(irrigatedAcres, scenario.IrrigatedAcres);
        }

        Assert.AreEqual(selectedFeeStructure.Categories.Count, scenario.CategoryBreakdown.Count);
        Assert.AreEqual(scenario.EstimatedFeeTotal, scenario.CategoryBreakdown.Sum(c => c.Fee.GetValueOrDefault(0)), "Estimated fee total should match the sum of all category fees.");
        Assert.AreEqual(scenario.TotalAnnualConsumedGroundwaterInAcreFeet.Round(2), scenario.CategoryBreakdown.Sum(c => c.AllocationUsedInAcreFeet.GetValueOrDefault(0)).Round(2), "Total consumed groundwater should match the sum of all allocations used.");

        var lastCategory = scenario.CategoryBreakdown.Last();
        Assert.AreEqual(0, lastCategory.RemainingGroundwaterConsumptionInAcreFeet, "Remaining groundwater consumption should be zero in the last category.");

        foreach (var category in scenario.CategoryBreakdown)
        {
            var inputCategory = selectedFeeStructure.Categories.FirstOrDefault(x => x.Name == category.Name);
            Assert.IsNotNull(inputCategory, $"Category {category.Name} should be in the input fee structure.");

            Assert.AreEqual(category.AllocationRemainingInAcreFeet, category.TotalAllocationInAcreFeet - category.AllocationUsedInAcreFeet, $"Remaining allocation in {category.Name} should match total minus used allocation.");

            if (category.Fee.HasValue && inputCategory.TotalFeePerAcreFoot.HasValue)
            {
                var difference = category.Fee - (inputCategory.TotalFeePerAcreFoot.Value * category.AllocationUsedInAcreFeet);
                Assert.IsTrue(difference < _tolerance, $"Fee in {category.Name} should match rate times used allocation within {_tolerance}.");
            }
        }

        var tolerance = .0001m;
        var categoryTotal = scenario.CategoryBreakdown.Sum(c => c.TotalAllocationInAcreFeet.GetValueOrDefault(0));
        var diff = scenario.TotalAllocationInAcreFeet.GetValueOrDefault(0) - categoryTotal;
        Assert.IsTrue(Math.Abs(diff) <= tolerance, $"Total allocation should match the sum of all category allocations. Diff: {diff}");
    }

    private void AssertScenariosAreEquivalent(FeeCalculatorOutputScenarioDto scenarioA, FeeCalculatorOutputScenarioDto scenarioB)
    {
        var properties = typeof(FeeCalculatorOutputScenarioDto).GetProperties();
        foreach (var property in properties)
        {
            var valueA = property.GetValue(scenarioA);
            var valueB = property.GetValue(scenarioB);

            if (valueA is decimal decimalA && valueB is decimal decimalB)
            {
                Assert.AreEqual(decimalA.Round(4), decimalB.Round(4), $"{property.Name} should be the same.");
            }
        }

        var categoryOutputProperties = typeof(FeeCategoryOutputDto).GetProperties();
        foreach (var property in categoryOutputProperties)
        {
            foreach (var categoryBreakdownA in scenarioA.CategoryBreakdown)
            {
                var categoryBreakdownB = scenarioB.CategoryBreakdown.FirstOrDefault(x => x.Name == categoryBreakdownA.Name);

                var valueA = property.GetValue(categoryBreakdownA);
                var valueB = property.GetValue(categoryBreakdownB);

                if (valueA is decimal decimalA && valueB is decimal decimalB)
                {
                    Assert.AreEqual(decimalA.Round(4), decimalB.Round(4), $"{property.Name} should be the same.");
                }
            }
        }
    }

    private void CheckSavingsAndIncentives(FeeCalculatorOutputScenarioDto outputBaselineScenario, FeeCalculatorOutputScenarioDto outputLandUseChangeScenario, List<MLRPIncentiveDto> incentives)
    {
        //TODO: Fill this out
    }
}