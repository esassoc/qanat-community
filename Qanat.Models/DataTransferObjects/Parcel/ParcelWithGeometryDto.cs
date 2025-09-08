namespace Qanat.Models.DataTransferObjects;

public class ParcelWithGeometryDto
{
    public int ParcelID { get; set; }
    public int GeographyID { get; set; }
    public string ParcelNumber { get; set; }
    public double ParcelArea { get; set; }
    public WaterAccountLinkDisplayDto WaterAccount { get; set; }
    public string ParcelStatus { get; set; }
    public string GeometryWKT { get; set; }
}