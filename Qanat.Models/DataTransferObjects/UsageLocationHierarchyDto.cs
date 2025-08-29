namespace Qanat.Models.DataTransferObjects;

public class UsageLocationHierarchyDto
{
    public int UsageLocationID { get; set; }
    public int GeographyID { get; set; }
    public int? WaterAccountID { get; set; }
    public int ParcelID { get; set; }
}