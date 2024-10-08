namespace Qanat.Models.DataTransferObjects;

public class MonthlyUsageSummaryDto
{
    public DateTime EffectiveDate { get; set; }
    public int EffectiveMonth { get; set; }
    public decimal? CurrentUsageAmount { get; set; }
    public decimal? AverageUsageAmount { get; set; }
    public decimal? CurrentCumulativeUsageAmount { get; set; }
    public decimal? AverageCumulativeUsageAmount { get; set; }
    public decimal? CurrentUsageAmountDepth { get; set; }
    public decimal? AverageUsageAmountDepth { get; set; }
    // these 4 fields are calculated in the front end
    public decimal? CurrentCumulativeUsageAmountDepth { get; set; }
    public decimal? AverageCumulativeUsageAmountDepth { get; set; }
    public decimal? TotalSupply { get; set; }
    public decimal? TotalSupplyDepth { get; set; }
}