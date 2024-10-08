namespace Qanat.Models.DataTransferObjects;

public class GeographyDto
{
    public int GeographyID { get; set; }
    public GeographyConfigurationSimpleDto GeographyConfiguration { get; set; }
    public string GeographyName { get; set; }
    public int StartYear { get; set; }
    public string GeographyDisplayName { get; set; }
    public string GeographyDescription { get; set; }
    public string APNRegexPattern { get; set; }
    public string APNRegexPatternDisplay { get; set; }
    public bool IsOpenETActive { get; set; }
    public string OpenETShapeFilePath { get; set; }
    public string OpenETRasterTimeseriesMultipolygonColumnToUseAsIdentifier { get; set; }
    public int CoordinateSystem { get; set; }
    public int AreaToAcresConversionFactor { get; set; }
    public bool IsDemoGeography { get; set; }
    public int? GSACanonicalID { get; set; }
    public string Color { get; set; }
    public WaterMeasurementTypeSimpleDto SourceOfRecordWaterMeasurementType { get; set; }
    public string SourceOfRecordExplanation { get; set; }

    public List<UserDto> WaterManagers { get; set; }
    public bool AllocationPlansEnabled { get; set; }
    public bool AllocationPlansVisibleToLandowners { get; set; }
    public bool AllocationPlansVisibleToPublic { get; set; }
    public BoundingBoxDto BoundingBox { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhoneNumber { get; set; }
    public bool WellRegistryEnabled { get; set; }
    public int DefaultDisplayYear { get; set; }
    public string LandownerDashboardSupplyLabel { get; set; }
    public string LandownerDashboardUsageLabel { get; set; }
    public bool LandingPageEnabled { get; set; }
    public bool MeterDataEnabled { get; set; }
    public bool DisplayUsageGeometriesAsField { get; set; }
    public bool AllowLandownersToRequestAccountChanges { get; set; }
}