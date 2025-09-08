using System;

namespace Qanat.Swagger.Models;

public class WaterMeasurementConsumerDto
{
    public int WaterMeasurementID { get; set; }
    public int WaterMeasurementTypeID { get; set; }
    public string WaterMeasurementTypeName { get; set; }
    public int UsageLocationID { get; set; }
    public string UsageLocationName { get; set; }
    public string UsageLocationType { get; set; }
    public int ParcelID { get; set; }
    public string ParcelNumber { get; set; }
    public DateTime ReportingDate { get; set; }
    public decimal? ReportedValueInFeet { get; set; }
    public decimal? ReportedValueInAcreFeet { get; set; }
    public int GeographyID { get; set; }
}