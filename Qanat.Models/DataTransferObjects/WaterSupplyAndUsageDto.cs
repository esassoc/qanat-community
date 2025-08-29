namespace Qanat.Models.DataTransferObjects;

public class WaterSupplyAndUsageDto
{
    public decimal TotalSupply { get; set; }
    public decimal UsageToDate { get; set; }
    public DateTime? SupplyDate { get; set; }
    public DateTime? UsageDate { get; set; }
}