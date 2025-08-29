namespace Qanat.Models.DataTransferObjects;

public class WaterMeasurementQualityAssuranceDto
{
    public int UsageLocationID { get; set; }
    public string UsageLocationName { get; set; }
    public double UsageLocationArea { get; set; }
    public decimal? SummedValueInFeet { get; set; }
    public decimal? SummedValueInAcreFeet { get; set; }
    public int? PercentileBucket { get; set; } //MK 6/25/2025: Used on the QA/QC Usage Location Bulk Actions page.

    public int UsageLocationTypeID { get; set; }
    public string UsageLocationTypeName { get; set; }
    public int ParcelID { get; set; }
    public string ParcelNumber { get; set; }
    public int? WaterAccountID { get; set; }
    public string WaterAccountNumberAndName { get; set; }

    public Dictionary<int, decimal> ReportedValueInFeetByMonth { get; set; }

    public string CoverCropStatus { get; set; }
    public string FallowStatus { get; set; }
}