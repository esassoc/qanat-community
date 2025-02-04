using System.Collections.Generic;

namespace Qanat.Models.DataTransferObjects;

public class WaterAccountBudgetReportDto
{
    public int GeographyID { get; set; }
    public string GeographyName { get; set; }
    public int WaterAccountID { get; set; }
    public int WaterAccountNumber { get; set; }
    public string WaterAccountName { get; set; }
    public int ReportingPeriodID { get; set; }
    public string ReportingPeriodName { get; set; }
    public double AcresManaged { get; set; }
    public decimal TotalSupply { get; set; }
    public decimal UsageToDate { get; set; }
    public decimal CurrentAvailable { get; set; }
    public Dictionary<int, decimal> WaterSupplyByWaterType { get; set; }
    public string WaterAccountUrl { get; set; }
}