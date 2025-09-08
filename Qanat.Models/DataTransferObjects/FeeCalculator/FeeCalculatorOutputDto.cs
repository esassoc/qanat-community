namespace Qanat.Models.DataTransferObjects;

public class FeeCalculatorOutputDto
{
    public FeeCalculatorOutputScenarioDto BaselineScenario { get; set; }
    public FeeCalculatorOutputScenarioDto LandUseChangeScenario { get; set; }
    public FeeCalculatorOutputSavingsAndIncentivesDto SavingsAndIncentives { get; set; }
}

public class FeeCalculatorOutputScenarioDto
{
    //Fees
    public decimal? EstimatedFeeTotal { get; set; }
    public decimal? EstimatedFeePerAcreFoot { get; set; }
    public decimal? EstimatedFeePerParcelAcre { get; set; }
    public decimal? EstimatedFeePerIrrigatedAcre { get; set; }

    //Area
    public decimal? Acres { get; set; }
    public decimal? IrrigatedAcres { get; set; }
    public decimal? TransitionedAcres { get; set; }

    //Average Consumed Groundwater
    public decimal? EstimatedConsumedGroundwaterInAcreFeetByParcelAcres { get; set; }
    public decimal? EstimatedConsumedGroundwaterInAcreFeetByIrrigatedAcres { get; set; }

    //Total Allocation
    public decimal? TotalAllocationInAcreFeet { get; set; }
    public decimal? TotalAllocationInAcreFeetByAcre { get; set; }
    public decimal? TotalRemainingAllocationInAcreFeet { get; set; }
    public decimal? TotalRemainingGroundwaterConsumption { get; set; }

    //Usage 
    public decimal? TotalETInAcreFeet { get; set; }
    public decimal? TotalPrecipInAcreFeet { get; set; }
    public decimal? SurfaceWaterConsumedInAcreFeet { get; set; }
    public decimal? TotalAnnualConsumedGroundwaterInAcreFeet { get; set; }

    public List<FeeCategoryOutputDto> CategoryBreakdown { get; set; }
}

public class FeeCalculatorOutputSavingsAndIncentivesDto
{
    //Fee Reductions
    public decimal? FeeReductionTotal { get; set; }
    public decimal? FeeReductionPerAcreFoot { get; set; }
    public decimal? FeeReductionPerParcelAcre { get; set; }
    public decimal? FeeReductionPerIrrigatedAcre { get; set; }

    //MLRP Incentives
    public decimal? MLRPIncentiveTotal { get; set; }
    public List<MLRPIncentiveBreakdownDto> MLRPIncentiveBreakdown { get; set; } = [];
}

public class MLRPIncentiveBreakdownDto(string name, decimal? incentivePayment)
{
    public string Name { get; set; } = name;
    public decimal? IncentivePayment { get; set; } = incentivePayment;
}

public class FeeCategoryOutputDto
{
    public string Name { get; set; }
    public decimal? TotalAllocationInAcreFeet { get; set; }
    public decimal? AllocationUsedInAcreFeet { get; set; }
    public decimal? AllocationUsedInPercent { get; set; }
    public decimal? AllocationRemainingInAcreFeet { get; set; }
    public decimal? AllocationRemainingInPercent { get; set; }
    public decimal? RemainingGroundwaterConsumptionInAcreFeet { get; set; }
    public decimal? Fee { get; set; }
}