using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Qanat.EFModels.Entities;
using Qanat.Models.DataTransferObjects;
using Qanat.Models.DataTransferObjects.Geography;

namespace Qanat.API.Helpers.FeeCalculator;

public static class FeeCalculatorHelper
{
    private static readonly int _eTWaterMeasurementTypeID_ETSGSA = 21;
    private static readonly int _precipMeasurementTypeID_ETSGSA = 22;

    private static readonly decimal _precipitationAdjustmentMultiplierForBaseline = .71m;
    private static readonly decimal _precipitationAdjustmentMultiplierForCoverCrop = .85m;

    public static async Task<FeeCalculatorOutputDto> CalculateFee(QanatDbContext dbContext, GeographyPublicDto geography, FeeCalculatorInputDto input)
    {
        var result = new FeeCalculatorOutputDto();

        var reportingPeriod = await dbContext.ReportingPeriods.AsNoTracking().FirstOrDefaultAsync(x => x.ReportingPeriodID == input.ReportingPeriodID);
        if (reportingPeriod == null)
        {
            return new FeeCalculatorOutputDto();
        }

        var waterAccount = WaterAccounts.GetByIDAsDto(dbContext, input.WaterAccountID, reportingPeriod.ReportingPeriodID);
        var parcelIDs = waterAccount.Parcels.Select(x => x.ParcelID);

        var reportingPeriodStart = reportingPeriod.StartDate;
        var reportingPeriodEnd = reportingPeriod.EndDate;

        var waterMeasurements = await dbContext.vWaterMeasurements.AsNoTracking()
            .Where(x => x.GeographyID == geography.GeographyID)
            .Where(x => x.ReportedDate >= reportingPeriodStart && x.ReportedDate <= reportingPeriodEnd)
            .Where(x => parcelIDs.Contains(x.ParcelID))
            .Where(x => x.WaterMeasurementTypeID == _eTWaterMeasurementTypeID_ETSGSA || x.WaterMeasurementTypeID == _precipMeasurementTypeID_ETSGSA)
            .ToListAsync();

        var etMeasurements = waterMeasurements.Where(x => x.WaterMeasurementTypeID == _eTWaterMeasurementTypeID_ETSGSA).ToList();
        var precipMeasurements = waterMeasurements.Where(x => x.WaterMeasurementTypeID == _precipMeasurementTypeID_ETSGSA).ToList();

        result.BaselineScenario = CalculateFeeForScenario(input, waterAccount, etMeasurements, precipMeasurements, true, out _);
        result.LandUseChangeScenario = CalculateFeeForScenario(input, waterAccount, etMeasurements, precipMeasurements, false, out var savingsAndIncentives);

        savingsAndIncentives.FeeReductionTotal = result.BaselineScenario.EstimatedFeeTotal - result.LandUseChangeScenario.EstimatedFeeTotal;
        savingsAndIncentives.FeeReductionPerAcreFoot = result.BaselineScenario.EstimatedFeePerAcreFoot - result.LandUseChangeScenario.EstimatedFeePerAcreFoot;
        savingsAndIncentives.FeeReductionPerParcelAcre = result.BaselineScenario.EstimatedFeePerParcelAcre - result.LandUseChangeScenario.EstimatedFeePerParcelAcre;
        savingsAndIncentives.FeeReductionPerIrrigatedAcre = result.BaselineScenario.EstimatedFeePerIrrigatedAcre - result.LandUseChangeScenario.EstimatedFeePerIrrigatedAcre;

        result.SavingsAndIncentives = savingsAndIncentives;

        return result;
    }

    private static FeeCalculatorOutputScenarioDto CalculateFeeForScenario(FeeCalculatorInputDto input, WaterAccountDto waterAccount, List<vWaterMeasurement> etMeasurements, List<vWaterMeasurement> precipMeasurements, bool isBaselineScenario, out FeeCalculatorOutputSavingsAndIncentivesDto savingsAndIncentives)
    {
        var parcelAcres = (decimal)waterAccount.Acres;
        var irrigatedAcres = (decimal)waterAccount.IrrigatedAcres;

        if (!isBaselineScenario)
        {
            var disposedAllocationAcres = input.MLRPIncentives.Where(x => !x.KeepsAllocation).Sum(x => x.Acres).GetValueOrDefault(0);
            parcelAcres -= disposedAllocationAcres;

            var disposedIrrigatedAcres = input.MLRPIncentives.Where(x => !x.KeepIrrigateAcres).Sum(x => x.Acres).GetValueOrDefault(0);
            irrigatedAcres -= disposedIrrigatedAcres;
        }

        var totalET = etMeasurements.Sum(x => x.ReportedValueInAcreFeet);
        var totalPrecip = precipMeasurements.Sum(x => x.ReportedValueInAcreFeet);

        if (!isBaselineScenario)
        {
            var irrigatedAcreRatio = waterAccount.IrrigatedAcres != 0
                ? irrigatedAcres / (decimal)waterAccount.IrrigatedAcres
                : 0;

            totalET *= irrigatedAcreRatio;
            totalPrecip *= irrigatedAcreRatio;
        }

        var totalSurfaceWaterConsumed = ((input.SurfaceWaterDelivered * input.SurfaceWaterIrrigationEfficiency).GetValueOrDefault(0) / 100);

        var precipitationAdjustmentMultiplier = _precipitationAdjustmentMultiplierForBaseline;
        var effectivePrecip = totalPrecip * precipitationAdjustmentMultiplier;

        if (!isBaselineScenario)
        {
            var mlrpUsedToAdjustTotalAnnualConsumedGroundwater = input.MLRPIncentives.SingleOrDefault(x => x.UsedToAdjustAnnualConsumedGroundwater);
            if (mlrpUsedToAdjustTotalAnnualConsumedGroundwater is { Acres: not null } && irrigatedAcres != 0)
            {
                var coverCropFraction = (mlrpUsedToAdjustTotalAnnualConsumedGroundwater.Acres.Value) / irrigatedAcres;
                effectivePrecip = totalPrecip * coverCropFraction * _precipitationAdjustmentMultiplierForCoverCrop + totalPrecip * (1 - coverCropFraction) * _precipitationAdjustmentMultiplierForBaseline;
            }
        }

        decimal? totalAnnualConsumedGroundwaterInAcreFeet = totalET - effectivePrecip - totalSurfaceWaterConsumed;

        var estimatedConsumedGroundwaterInAcreFeetByParcelAcres = parcelAcres != 0
            ? (totalAnnualConsumedGroundwaterInAcreFeet / parcelAcres).Round(4)
            : 0m;

        var estimatedConsumedGroundwaterInAcreFeetByIrrigatedAcres = irrigatedAcres != 0
            ? (totalAnnualConsumedGroundwaterInAcreFeet / irrigatedAcres).Round(4)
            : 0m;

        var totalAllocation = 0m;
        var feeStructure = FeeStructuresDtos.ETSGSA_FeeStructures.First(x => x.FeeStructureID == input.FeeStructureID);

        var consumedGroundwaterDrainDown = totalAnnualConsumedGroundwaterInAcreFeet;

        var feeCategoryOutputs = new List<FeeCategoryOutputDto>();
        foreach (var feeStructureCategory in feeStructure.Categories)
        {
            // Calculate allocation for this category based on water account acres and the category's specified allocation multiplier
            var categoryAllocation = parcelAcres * feeStructureCategory.AllocationMultiplier;
            totalAllocation += categoryAllocation.GetValueOrDefault(0);

            // Check if this is the last category (no threshold ceiling) and allocate all remaining groundwater
            var allocationUsedInAcreFeet = feeStructureCategory.ThresholdCeiling == null
                ? consumedGroundwaterDrainDown // Allocate all remaining groundwater to this final category
                : Math.Min(consumedGroundwaterDrainDown.GetValueOrDefault(0), categoryAllocation.GetValueOrDefault(0));

            var allocationUsedInPercent = categoryAllocation.HasValue && categoryAllocation.GetValueOrDefault(0) != 0
                ? (allocationUsedInAcreFeet / categoryAllocation.Value) * 100
                : null;

            if (feeStructureCategory.ThresholdCeiling == null)
            {
                //On the last category allocation used in percent should show the total overage percent
                if (totalAllocation != 0)
                {
                    allocationUsedInPercent = Math.Max(0, -(1 - (totalAnnualConsumedGroundwaterInAcreFeet.GetValueOrDefault(0) / totalAllocation)) * 100);
                }
                else
                {
                    allocationUsedInPercent = null;
                }
            }

            consumedGroundwaterDrainDown -= allocationUsedInAcreFeet;

            var allocationRemaining = categoryAllocation - allocationUsedInAcreFeet;
            var allocationRemainingInPercent = categoryAllocation.HasValue && categoryAllocation.GetValueOrDefault(0) != 0
                ? (allocationRemaining / categoryAllocation.Value) * 100
                : null;

            // Calculate the remaining groundwater consumption specific to this category’s allocation
            var remainingGroundwaterConsumption = Math.Max(0, consumedGroundwaterDrainDown.GetValueOrDefault(0));

            var categoryFee = feeStructureCategory.TotalFeePerAcreFoot * allocationUsedInAcreFeet;
            var categoryOutput = new FeeCategoryOutputDto()
            {
                Name = feeStructureCategory.Name,
                TotalAllocationInAcreFeet = categoryAllocation.Round(4),
                AllocationUsedInAcreFeet = allocationUsedInAcreFeet.Round(4),
                AllocationUsedInPercent = allocationUsedInPercent.Round(0),
                AllocationRemainingInAcreFeet = allocationRemaining.Round(4),
                AllocationRemainingInPercent = allocationRemainingInPercent.Round(0),
                RemainingGroundwaterConsumptionInAcreFeet = remainingGroundwaterConsumption.Round(4),
                Fee = categoryFee.Round(2)
            };

            feeCategoryOutputs.Add(categoryOutput);
        }

        savingsAndIncentives = null;

        if (!isBaselineScenario)
        {
            savingsAndIncentives = new FeeCalculatorOutputSavingsAndIncentivesDto();
            foreach (var inputMLRPIncentive in input.MLRPIncentives.Where(x => x.Acres.HasValue && x.IncentivePayment.HasValue).OrderBy(x => x.DisplayOrder))
            {
                var incentiveSavings = inputMLRPIncentive.Acres * inputMLRPIncentive.IncentivePayment;
                savingsAndIncentives.MLRPIncentiveBreakdown.Add(new MLRPIncentiveBreakdownDto(inputMLRPIncentive.Name, incentiveSavings));
            }

            savingsAndIncentives.MLRPIncentiveTotal = savingsAndIncentives.MLRPIncentiveBreakdown.Sum(x => x.IncentivePayment);
        }

        var totalFee = feeCategoryOutputs.Sum(x => x.Fee.GetValueOrDefault(0));

        var feePerAcreFoot = totalAnnualConsumedGroundwaterInAcreFeet != 0
                ? totalFee / totalAnnualConsumedGroundwaterInAcreFeet
                : 0;

        var feePerParcelAcre = parcelAcres != 0
            ? totalFee / parcelAcres
            : 0;

        var feePerIrrigatedAcre = irrigatedAcres != 0
            ? totalFee / irrigatedAcres
            : 0;

        var totalAllocationInAcreFeetByAcre = parcelAcres != 0
            ? totalAllocation / parcelAcres
            : 0;

        var totalRemainingAllocationsInAcreFeet = feeCategoryOutputs.Sum(x => x.AllocationRemainingInAcreFeet.GetValueOrDefault(0));

        var lastCategoryUsed = feeCategoryOutputs.LastOrDefault(x => x.AllocationUsedInAcreFeet.HasValue);
        var totalRemainingGroundwaterConsumption = lastCategoryUsed.RemainingGroundwaterConsumptionInAcreFeet == 0
            ? null
            : lastCategoryUsed.RemainingGroundwaterConsumptionInAcreFeet;

        var scenario = new FeeCalculatorOutputScenarioDto()
        {
            EstimatedFeeTotal = totalFee.Round(2),
            EstimatedFeePerAcreFoot = feePerAcreFoot.Round(2),
            EstimatedFeePerParcelAcre = feePerParcelAcre.Round(2),
            EstimatedFeePerIrrigatedAcre = feePerIrrigatedAcre.Round(2),

            Acres = parcelAcres.Round(4),
            IrrigatedAcres = irrigatedAcres.Round(4),
            TransitionedAcres = isBaselineScenario
                ? null
                : input?.MLRPIncentives?.Sum(x => x.Acres),

            EstimatedConsumedGroundwaterInAcreFeetByParcelAcres = estimatedConsumedGroundwaterInAcreFeetByParcelAcres,
            EstimatedConsumedGroundwaterInAcreFeetByIrrigatedAcres = estimatedConsumedGroundwaterInAcreFeetByIrrigatedAcres,

            TotalAllocationInAcreFeet = totalAllocation.Round(4),
            TotalAllocationInAcreFeetByAcre = totalAllocationInAcreFeetByAcre.Round(4),

            TotalETInAcreFeet = totalET.Round(4),
            TotalPrecipInAcreFeet = totalPrecip.Round(4),
            SurfaceWaterConsumedInAcreFeet = totalSurfaceWaterConsumed.Round(4),
            TotalAnnualConsumedGroundwaterInAcreFeet = totalAnnualConsumedGroundwaterInAcreFeet.Round(4),

            TotalRemainingAllocationInAcreFeet = totalRemainingAllocationsInAcreFeet.Round(4),
            TotalRemainingGroundwaterConsumption = totalRemainingGroundwaterConsumption.Round(4),

            CategoryBreakdown = feeCategoryOutputs
        };

        return scenario;
    }
}

public static class DecimalExtensions
{
    public static decimal? Round(this decimal? value, int decimalPlaces)
    {
        return value?.Round(decimalPlaces);
    }

    public static decimal? Round(this decimal value, int decimalPlaces)
    {
        return Math.Round(value, decimalPlaces, MidpointRounding.ToEven);
    }
}