namespace Qanat.Models.DataTransferObjects;

public class GeographySourceOfRecordWaterMeasurementTypeMonthlyUsageSummaryDto
{
    public decimal? TotalUsageEntityArea { get; set; }
    public decimal? WaterMeasurementTotalValue { get; set; }
    public List<MonthlyUsageSummaryDto> WaterMeasurementMonthlyValues { get; set; }
}