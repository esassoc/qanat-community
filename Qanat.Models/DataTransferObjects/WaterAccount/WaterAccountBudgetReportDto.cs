namespace Qanat.Models.DataTransferObjects;

public class WaterAccountBudgetReportDto
{
    public int GeographyID { get; set; }
    public string GeographyName { get; set; }
    public int WaterAccountID { get; set; }
    public int WaterAccountNumber { get; set; }
    public string WaterAccountName { get; set; }
    public double ParcelArea { get; set; }
    public int ParcelCount { get; set; }
    public double UsageLocationArea { get; set; }
    public double TotalSupply { get; set; }
    public double UsageToDate { get; set; }
    public double CurrentAvailable { get; set; }
    public Dictionary<int, decimal> WaterSupplyByWaterType { get; set; }
    public string WaterAccountUrl { get; set; }
    public string ZoneIDs { get; set; }
}