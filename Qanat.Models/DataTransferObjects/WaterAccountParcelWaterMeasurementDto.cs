namespace Qanat.Models.DataTransferObjects;

public class WaterAccountParcelWaterMeasurementDto
{
    public int ParcelID { get; set; }
    public string ParcelNumber { get; set; }
    public decimal? UsageEntityArea { get; set; }
    public int WaterMeasurementTypeID { get; set; }
    public string WaterMeasurementTypeName { get; set; }
    public string WaterMeasurementCategoryTypeName { get; set; }
    public List<MonthlyUsageSummaryDto> WaterMeasurementMonthlyValues { get; set; }
    public decimal? WaterMeasurementTotalValue { get; set; }
}