namespace Qanat.Models.DataTransferObjects;

public class GeographyDto
{
    // Keys/Related Objects
    public int GeographyID { get; set; }
    public GeographyConfigurationSimpleDto GeographyConfiguration { get; set; }

    // Basic/Misc Data
    public string GeographyName { get; set; }
    public string GeographyDisplayName { get; set; }
    public string GeographyDescription { get; set; }
    public string APNRegexPattern { get; set; }
    public string APNRegexPatternDisplay { get; set; }
    public int? GSACanonicalID { get; set; }
    public string Color { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhoneNumber { get; set; }
    public string ContactAddressLine1 { get; set; }
    public string ContactAddressLine2 { get; set; }
    public string LandownerDashboardSupplyLabel { get; set; }
    public string LandownerDashboardUsageLabel { get; set; }

    // Spatial Data
    public BoundingBoxDto BoundingBox { get; set; }
    public int CoordinateSystem { get; set; }
    public int AreaToAcresConversionFactor { get; set; }
        
    // OpenET Configuration
    public bool IsOpenETActive { get; set; }

    // Source of Record
    public WaterMeasurementTypeSimpleDto SourceOfRecordWaterMeasurementType { get; set; }
    public string SourceOfRecordExplanation { get; set; }

    // Water Budget
    public bool ShowSupplyOnWaterBudgetComponent { get; set; }
    public string WaterBudgetSlotAHeader { get; set; }
    public string WaterBudgetSlotBHeader { get; set; }
    public string WaterBudgetSlotCHeader { get; set; }

    // Misc Configuration
    public bool IsDemoGeography { get; set; }
    public bool AllocationPlansEnabled { get; set; }
    public bool AllocationPlansVisibleToLandowners { get; set; }
    public bool AllocationPlansVisibleToPublic { get; set; }
    public bool WellRegistryEnabled { get; set; }
    public bool LandingPageEnabled { get; set; }
    public bool MeterDataEnabled { get; set; }
    public bool AllowLandownersToRequestAccountChanges { get; set; }
    public bool AllowWaterMeasurementSelfReporting { get; set; }
    public bool AllowFallowSelfReporting { get; set; }
    public bool FeeCalculatorEnabled { get; set; }
}