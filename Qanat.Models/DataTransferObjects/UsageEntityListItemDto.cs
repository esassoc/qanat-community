namespace Qanat.Models.DataTransferObjects;

public class UsageEntityListItemDto
{
    public int UsageEntityID { get; set; }
    public int GeographyID { get; set; }
    public int WaterAccountID { get; set; }
    public int ParcelID { get; set; }
    public List<string> CropNames { get; set; }
    public string UsageEntityName { get; set; }
    public string WaterAccountName { get; set; }
    public string ParcelNumber { get; set; }
    public double Area { get; set; }
}