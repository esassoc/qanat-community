namespace Qanat.Models.DataTransferObjects;

public class GeographyMinimalDto
{
    public int GeographyID { get; set; }
    public string GeographyName { get; set; }
    public string GeographyDisplayName { get; set; }
    public bool IsOpenETActive { get; set; }
    public bool AllowLandownersToRequestAccountChanges { get; set; }
    public bool AllowWaterMeasurementSelfReporting { get; set; }
    public bool AllowFallowSelfReporting { get; set; }
    public bool AllowCoverCropSelfReporting { get; set; }
    public bool AllocationPlansEnabled { get; set; }
    public bool AllocationPlansVisibleToLandowners { get; set; }
    public bool AllocationPlansVisibleToPublic { get; set; }
    public int? SourceOfRecordWaterMeasurementTypeID { get; set; }
    public GeographyConfigurationSimpleDto GeographyConfiguration { get; set; }
}