namespace Qanat.Models.DataTransferObjects;

public class GeographyMinimalDto
{
    public int GeographyID { get; set; }
    public string GeographyName { get; set; }
    public string GeographyDisplayName { get; set; }
    public int DefaultDisplayYear { get; set; }
    public bool DisplayUsageGeometriesAsField { get; set; }
    public bool AllowLandownersToRequestAccountChanges { get; set; }
    public bool AllowWaterMeasurementSelfReporting { get; set; }
    public bool AllocationPlansEnabled { get; set; }
    public bool AllocationPlansVisibleToLandowners { get; set; }
    public bool AllocationPlansVisibleToPublic { get; set; }
    public GeographyConfigurationSimpleDto GeographyConfiguration { get; set; }
}