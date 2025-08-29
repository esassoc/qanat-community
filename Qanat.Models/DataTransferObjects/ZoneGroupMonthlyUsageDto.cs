namespace Qanat.Models.DataTransferObjects;

public class ZoneGroupMonthlyUsageDto
{
    public string ZoneName { get; set; }
    public string ZoneColor { get; set; }
    public DateTime EffectiveDate { get; set; }
    public decimal? UsageAmount { get; set; }
    public decimal? UsageAmountDepth { get; set; }
}