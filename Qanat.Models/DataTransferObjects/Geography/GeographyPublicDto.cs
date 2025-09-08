namespace Qanat.Models.DataTransferObjects.Geography;

public class GeographyPublicDto
{
    public int GeographyID { get; set; }
    public string GeographyName { get; set; }
    public string GeographyDisplayName { get; set; }
    public string GeographyDescription { get; set; }
    public int StartYear { get; set; }
    public bool IsDemoGeography { get; set; }
    public string Color { get; set; }
    public bool AllocationPlansEnabled { get; set; }
    public bool AllocationPlansVisibleToLandowners { get; set; }
    public bool AllocationPlansVisibleToPublic { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhoneNumber { get; set; }
    public bool LandingPageEnabled { get; set; }
    public bool MeterDataEnabled { get; set; }
    public bool WellRegistryEnabled { get; set; }
    public bool FeeCalculatorEnabled { get; set; }
    public bool AllowWaterMeasurementSelfReporting { get; set; }
}