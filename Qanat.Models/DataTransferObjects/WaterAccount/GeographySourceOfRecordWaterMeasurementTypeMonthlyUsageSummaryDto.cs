namespace Qanat.Models.DataTransferObjects;

public class GeographySourceOfRecordWaterMeasurementTypeMonthlyUsageSummaryDto
{
    public decimal? TotalUsageLocationArea { get; set; }
    public decimal? TotalParcelArea { get; set; }
    public decimal? WaterMeasurementTotalValue { get; set; }
    public List<MonthlyUsageSummaryDto> WaterMeasurementMonthlyValues { get; set; }
}