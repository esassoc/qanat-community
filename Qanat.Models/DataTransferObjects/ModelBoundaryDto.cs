namespace Qanat.Models.DataTransferObjects;

public class ModelBoundaryDto
{
    public int ModelBoundaryID { get; set; }
    public int ModelID { get; set; }
    public string GeoJson { get; set; }
}