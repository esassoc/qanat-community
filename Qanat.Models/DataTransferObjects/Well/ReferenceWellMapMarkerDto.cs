namespace Qanat.Models.DataTransferObjects;

public class ReferenceWellMapMarkerDto
{
    public int ReferenceWellID { get; set; }
    public int GeographyID { get; set; }
    public string CountyWellPermitNo { get; set; }
    public string StateWCRNumber { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }

}