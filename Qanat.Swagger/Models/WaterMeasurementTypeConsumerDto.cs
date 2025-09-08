namespace Qanat.Swagger.Models;

public class WaterMeasurementTypeConsumerDto
{
    public int WaterMeasurementTypeID { get; set; }
    public string WaterMeasurementTypeName { get; set; }
    public string WaterMeasurementCategoryType { get; set; }
    public bool IsActive { get; set; }
    public int GeographyID { get; set; }
}