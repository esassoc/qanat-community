using System.Reflection.Metadata;

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
            Name = "Temporary Fallowing (Land Fallowing Program)",
            Description = "Crop removal, cessation of irrigation, and Cover Cropping or dry-land farming for at least three years.",
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
            Name = "Rotational Extended Fallowing (MLRP)",
            Description = "Extended Fallowing on a rotational basis for at least three years per plot over a period of 10 years. Promotion of multiple benefits by planting in beneficial cover crops such as pollinator seed mixes, establishment of hedge rows or other benefits.",
            DisplayOrder = 4,
            GroundwaterCredit = MLRPGroundwaterCredits.EffectivePrecipitationCredit,
            ContractDuration = "Ten year rotational or programmatic commitment*",
            KeepsAllocation = false,
            KeepIrrigateAcres = false,
            IncentivePayment = 850,
            IncentivePaymentLabel = "$850/Acre/Year",
            Acres = null,
            FieldDefinitionType = "FeeCalculatorRotationalExtendedFallowingMLRP",
            UsedToAdjustAnnualConsumedGroundwater = false
        },
        new MLRPIncentiveDto()
        {
            Name = "Rotational Extended Fallowing in Designated Buffer Zones (MLRP)",
            Description = "Crop removal, cessation of irrigation, and Cover Cropping in proximity to schools, communities, or other sensitive areas for at least ten years.",
            DisplayOrder = 5,
            GroundwaterCredit = MLRPGroundwaterCredits.EffectivePrecipitationCredit,
            ContractDuration = "Ten year commitment",
            KeepsAllocation = false,
            KeepIrrigateAcres = false,
            IncentivePayment = 850,
            IncentivePaymentLabel = "$850/Acre/Year",
            Acres = null,
            FieldDefinitionType = "FeeCalculatorRotationalExtendedFallowingInDesignatedBufferZonesMLRP",
            UsedToAdjustAnnualConsumedGroundwater = false
        },
        new MLRPIncentiveDto()
        {
            Name = "Orchard Swale Rewilding (MLRP)",
            Description = "Cessation of irrigation, orchard crop removal, and planting in pollinator-friendly or other beneficial cover crop.",
            DisplayOrder = 6,
            GroundwaterCredit = MLRPGroundwaterCredits.PossibleRechargeCredit,
            ContractDuration = "Ten year commitment",
            KeepsAllocation = false,
            KeepIrrigateAcres = false,
            IncentivePayment = 1040,
            IncentivePaymentLabel = "$1040/Acre/Year",
            Acres = null,
            FieldDefinitionType = "FeeCalculatorOrchardSwaleRewildingMLRP",
            UsedToAdjustAnnualConsumedGroundwater = false
        },
        new MLRPIncentiveDto()
        {
            Name = "Floodplain Reconnection and Related Spreading and Recharge (MLRP)",
            Description = "Modification of stream channels to promote restoration of natural flood hydrology. Cessation of irrigation, crop removal, and planting of riparian or other beneficial vegetation, land modification to promote seasonal flooding.",
            DisplayOrder = 7,
            GroundwaterCredit = MLRPGroundwaterCredits.PossibleRechargeCredit,
            ContractDuration = "Ten year commitment",
            KeepsAllocation = false,
            KeepIrrigateAcres = false,
            IncentivePayment = 1030,
            IncentivePaymentLabel = "$1030/Acre/Year",
            Acres = null,
            FieldDefinitionType = "FeeCalculatorFloodplainReconnectionAndRelatedSpreadingAndRechargeMLRP",
            UsedToAdjustAnnualConsumedGroundwater = false
        },
        new MLRPIncentiveDto()
        {
            Name = "Floodflow Spreading on Non-Floodplain Lands (MLRP)",
            Description = "Modification of drainageways or canals to promote spreading of flood flows. Cessation of irrigation, crop removal, planting in pollinator-friendly or other beneficial cover crop, and seasonal spreading of floodflows.",
            DisplayOrder = 8,
            GroundwaterCredit = MLRPGroundwaterCredits.PossibleRechargeCredit,
            ContractDuration = "Ten year commitment",
            KeepsAllocation = false,
            KeepIrrigateAcres = false,
            IncentivePayment = 1030,
            IncentivePaymentLabel = "$1030/Acre/Year",
            Acres = null,
            FieldDefinitionType = "FeeCalculatorFloodflowSpreadingOnNonFloodplainLandsMLRP",
            UsedToAdjustAnnualConsumedGroundwater = false
        },
        new MLRPIncentiveDto()
        {
            Name = "Storage or Recharge Basins (MLRP)",
            Description = "Cessation of irrigation, crop removal, and construction of storage or recharge basins within farm units.",
            DisplayOrder = 8,
            GroundwaterCredit = MLRPGroundwaterCredits.PossibleRechargeCredit,
            ContractDuration = "Ten year commitment",
            KeepsAllocation = false,
            KeepIrrigateAcres = false,
            IncentivePayment = 2960,
            IncentivePaymentLabel = "$2960/Acre/Year",
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