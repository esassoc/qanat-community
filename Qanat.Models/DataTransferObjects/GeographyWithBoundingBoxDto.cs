namespace Qanat.Models.DataTransferObjects;

public class GeographyWithBoundingBoxDto
{
    public int GeographyID { get; set; }
    public string GeographyName { get; set; }
    public string GeographyDisplayName { get; set; }
    public BoundingBoxDto BoundingBox { get; set; }
}