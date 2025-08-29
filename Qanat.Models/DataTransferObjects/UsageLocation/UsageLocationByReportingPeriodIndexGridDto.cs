namespace Qanat.Models.DataTransferObjects;

public class UsageLocationByReportingPeriodIndexGridDto
{
    public int UsageLocationID { get; set; }

    public int? UsageLocationTypeID { get; set; } //MK 6/19/2025 TODO: Make not nullable after a production release.
    public string UsageLocationTypeName { get; set; }

    public int? WaterAccountID { get; set; }
    public string? WaterAccountNumberAndName { get; set; }

    public int ParcelID { get; set; }
    public string ParcelNumber { get; set; }

    public int ReportingPeriodID { get; set; }
    public string ReportingPeriodName { get; set; }

    public string Name { get; set; }
    public double Area { get; set; }


    public List<string> Crops { get; set; }
}