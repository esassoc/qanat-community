namespace Qanat.Models.DataTransferObjects;

public class WaterAccountGeoJSONDto
{
    public BoundingBoxDto BoundingBox { get; set; }
    public List<ParcelWithGeoJSONDto> Parcels { get; set; }
    public List<UsageEntityWithGeoJSONDto> UsageEntities { get; set; }
}

public class UsageEntityWithGeoJSONDto
{
    public int UsageEntityID { get; set; }
    public string UsageEntityName { get; set; }
    public int ParcelID { get; set; }
    public double Area { get; set; }
    public List<string> CropNames { get; set; }
    public string GeoJSON { get; set; }
}