namespace Qanat.Models.DataTransferObjects;

public class MLRPIncentiveDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string GroundwaterCredit { get; set; }
    public string ContractDuration { get; set; }
    public bool KeepsAllocation { get; set; }
    public bool KeepIrrigateAcres { get; set; }
    public decimal? IncentivePayment { get; set; }
    public string IncentivePaymentLabel { get; set; }
    public decimal? Acres { get; set; }
    public decimal DisplayOrder { get; set; }
    public string FieldDefinitionType { get; set; }

    public bool UsedToAdjustAnnualConsumedGroundwater { get; set; }
}

public static class MLRPIncentiveDtos
{
    public static List<MLRPIncentiveDto> ETSGSA_MLRPIncentives =
    [
        new MLRPIncentiveDto()
        {
            Name = "Fallowing (Self-Directed)",
            Description = "Crop removal and cessation of irrigation for a minimum of one year.",
            DisplayOrder = 1,
            GroundwaterCredit = MLRPGroundwaterCredits.None,
            ContractDuration = "Not Applicable",
            KeepsAllocation = true,
            KeepIrrigateAcres = false,
            IncentivePayment = null,
            IncentivePaymentLabel = "None",
            Acres = null,
            FieldDefinitionType = "FeeCalculatorFallowingSelfDirected",
            UsedToAdjustAnnualConsumedGroundwater = false
        },
        new MLRPIncentiveDto()
        {
            Name = "Cover Cropping (Self-Directed)",
            Description = "Maintenance of a cover crop on irrigated land for at least five consecutive months between November and April.",
            DisplayOrder = 2,
            GroundwaterCredit = MLRPGroundwaterCredits.EffectivePrecipitationCredit,
            ContractDuration = "Not Applicable",
            KeepsAllocation = true,
            KeepIrrigateAcres = true,
            IncentivePayment = null,
            IncentivePaymentLabel = "None",
            Acres = null,
            FieldDefinitionType = "FeeCalculatorCoverCroppingSelfDirected",
            UsedToAdjustAnnualConsumedGroundwater = true
        },
        new MLRPIncentiveDto()
        {
            Name = "Incentivized Fallowing Program",
            Description = "Fallowing for at least three years, maintenance of vegetative cover (can be resident vegetation, non-irrigated crop or conservation cover seed mix).",
            DisplayOrder = 3,
            GroundwaterCredit = MLRPGroundwaterCredits.EffectivePrecipitationCredit,
            ContractDuration = "Three-year minimum",
            KeepsAllocation = false,
            KeepIrrigateAcres = false,
            IncentivePayment = 730,
            IncentivePaymentLabel = "$730/Acre/Year",
            Acres = null,
            FieldDefinitionType = "FeeCalculatorTemporaryFallowingLandFallowingProgram",
            UsedToAdjustAnnualConsumedGroundwater = false
        },
        new MLRPIncentiveDto()
        {
            Name = "MLRP Rotational/Extended Fallowing",
            Description = "Fallowing for at least three years and maintenance of beneficial conservation cover using a commercially-available seed mix (e.g., pollinator mix, native veg mix, etc.).",
            DisplayOrder = 4,
            GroundwaterCredit = MLRPGroundwaterCredits.EffectivePrecipitationCredit,
            ContractDuration = "Ten year rotational or programmatic commitment*",
            KeepsAllocation = false,
            KeepIrrigateAcres = false,
            IncentivePayment = 890,
            IncentivePaymentLabel = "$890/Acre/Year",
            Acres = null,
            FieldDefinitionType = "FeeCalculatorRotationalExtendedFallowingMLRP",
            UsedToAdjustAnnualConsumedGroundwater = false
        },
        new MLRPIncentiveDto()
        {
            Name = "MLRP Orchard Swale Rewilding",
            Description = "Tree removal, establishment and maintenance of beneficial conservation cover/hedgerows for at least 10 years. May be combined with one-time incentive payment for construction of stormwater retention basins in swales or topographic depressions.",
            DisplayOrder = 6,
            GroundwaterCredit = MLRPGroundwaterCredits.PossibleRechargeCredit,
            ContractDuration = "Ten year commitment",
            KeepsAllocation = false,
            KeepIrrigateAcres = false,
            IncentivePayment = 1300,
            IncentivePaymentLabel = "$1300/Acre/Year",
            Acres = null,
            FieldDefinitionType = "FeeCalculatorOrchardSwaleRewildingMLRP",
            UsedToAdjustAnnualConsumedGroundwater = false
        },
        new MLRPIncentiveDto()
        {
            Name = "MLRP Floodplain Reconnection and Related Spreading and Recharge",
            Description = "Crop removal, establishment and maintenance of herbaceous riparian cover or grassed waterway for at least 10 years. May be combined with one-time incentive payment for construction of in-stream flow modifications (e.g. beaver dam analogs, woody revetments) and flow spreading and retention structures (berms, ditches and ponds).",
            DisplayOrder = 7,
            GroundwaterCredit = MLRPGroundwaterCredits.PossibleRechargeCredit,
            ContractDuration = "Ten year commitment",
            KeepsAllocation = false,
            KeepIrrigateAcres = false,
            IncentivePayment = 1450,
            IncentivePaymentLabel = "$1450/Acre/Year",
            Acres = null,
            FieldDefinitionType = "FeeCalculatorFloodplainReconnectionAndRelatedSpreadingAndRechargeMLRP",
            UsedToAdjustAnnualConsumedGroundwater = false
        },
        new MLRPIncentiveDto()
        {
            Name = "MLRP Flood Flow Spreading",
            Description = "Crop removal , establishment and maintenance of herbaceous riparian cover, grassed waterway or other conservation cover for at least 10 years. Maintenance of erosion and sedimentation, periodic discing to maintain soil conditions suitable for recharge.  May be combined with one-time incentive payment for construction of flood flow conveyance infrastructure (e.g. pipelines, pump stations, weirs, etc.) and flow spreading and retention structures (berms, ditches and ponds).",
            DisplayOrder = 8,
            GroundwaterCredit = MLRPGroundwaterCredits.PossibleRechargeCredit,
            ContractDuration = "Ten year commitment",
            KeepsAllocation = false,
            KeepIrrigateAcres = false,
            IncentivePayment = 1590,
            IncentivePaymentLabel = "$1590/Acre/Year",
            Acres = null,
            FieldDefinitionType = "FeeCalculatorFloodflowSpreadingOnNonFloodplainLandsMLRP",
            UsedToAdjustAnnualConsumedGroundwater = false
        },
        new MLRPIncentiveDto()
        {
            Name = "MLRP Storage/Recharge Basins",
            Description = "Crop removal, maintenance of storage or recharge basin for at least 10 years, including vegetation and vector management, and periodic regrading/surface conditioning as needed to maintain recharge function. May be combined with one-time incentive payment for basin and conveyance infrastructure construction.",
            DisplayOrder = 8,
            GroundwaterCredit = MLRPGroundwaterCredits.PossibleRechargeCredit,
            ContractDuration = "Ten year commitment",
            KeepsAllocation = false,
            KeepIrrigateAcres = false,
            IncentivePayment = 1030,
            IncentivePaymentLabel = "$1030/Acre/Year",
            Acres = null,
            FieldDefinitionType = "FeeCalculatorStorageOrRechargeBasinsMLRP",
            UsedToAdjustAnnualConsumedGroundwater = false
        },
    ];
}

public static class MLRPGroundwaterCredits
{
    public static string None = "None";
    public static string EffectivePrecipitationCredit = "Effective Precipitation Credit";
    public static string PossibleRechargeCredit = "Possible Recharge Credit";
}