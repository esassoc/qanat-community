namespace Qanat.Models.DataTransferObjects;

public class WaterAccountWaterTypeMonthlySupplyDto
{
    public int WaterAccountID { get; set; }
    public int WaterTypeID { get; set; }
    public string WaterTypeName { get; set; }
    public string WaterTypeColor { get; set; }
    public int WaterTypeSortOrder { get; set; }
    public string WaterTypeDefinition { get; set; }
    public decimal? TotalSupply { get; set; }
    public Dictionary<int, decimal?> CumulativeSupplyByMonth { get; set; }
    public Dictionary<int, double?> CumulativeSupplyDepthByMonth { get; set; }
}