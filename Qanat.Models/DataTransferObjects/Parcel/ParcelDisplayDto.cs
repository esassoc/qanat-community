namespace Qanat.Models.DataTransferObjects;

public class ParcelDisplayDto
{
    public int ParcelID { get; set; }
    public string ParcelNumber { get; set; }
    public double ParcelArea { get; set; }
    public int GeographyID { get; set; }
    public int? WaterAccountID { get; set; }
}