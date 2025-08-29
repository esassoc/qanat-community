namespace Qanat.Models.DataTransferObjects.Geography;

public class GeographyWithBoundingBoxDto
{
    public int GeographyID { get; set; }
    public string GeographyName { get; set; }
    public string GeographyDisplayName { get; set; }
    public BoundingBoxDto BoundingBox { get; set; }

    public bool AllocationPlansVisibleToLandowners { get; set; }
    public bool AllocationPlansVisibleToPublic { get; set; }
}