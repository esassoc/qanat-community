using System.Collections.Generic;

namespace Qanat.Swagger.Models;

public class UsageLocationConsumerDto
{
    public int UsageLocationID { get; set; }
    public string Name { get; set; }
    public double Area { get; set; }
    public string UsageLocationType { get; set; }
    public int? WaterAccountID { get; set; }
    public int? WaterAccountNumber { get; set; }
    public int ParcelID { get; set; }
    public string ParcelNumber { get; set; }
    public List<ZoneConsumerDto> ParcelZones { get; set; }
    public int ReportingPeriodID { get; set; }
    public string ReportingPeriodName { get; set; }
    public int GeographyID { get; set; }
}