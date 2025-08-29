namespace Qanat.Models.DataTransferObjects;

public class UsageStatementWaterAccountDto
{
    public int WaterAccountID { get; set; }
    public int WaterAccountNumber { get; set; }
    public string ZoneName { get; set; }
    public string ContactName { get; set; }
    public string ContactAddress { get; set; }
    public string ContactSecondaryAddress { get; set; }
    public string ContactCity { get; set; }
    public string ContactState { get; set; }
    public string ContactZipCode { get; set; }

    public string WaterAccountPIN { get; set; }
    public List<int> ParcelIDs { get; set; }
    public List<string> ParcelNumbers { get; set; }

    public double ParcelArea { get; set; }
    public double UsageArea { get; set; }

    public string ParcelAreaFormatted { get; set; }
    public string UsageAreaFormatted { get; set; }

    public string ParcelGeoJSON { get; set; }

    // depth values are in ac-ft/ac, volume is in ac/ft
    public string SupplyDepthFormatted { get; set; }
    public string UsageDepthFormatted { get; set; }
    public string UsageVolumeFormatted { get; set; }
    public string BalanceDepthFormatted { get; set; }
    public string BalanceVolumeFormatted { get; set; }

    public List<UsageStatementSupplyTableRowDto> SupplyTableRowDtos { get; set; }

    public string StatementTitle { get; set; }
    public DateTime StatementDate { get; set; }
    public DateTime ReportingPeriodStartDate { get; set; }
    public DateTime ReportingPeriodEndDate { get; set; }

    public int GeographyID { get; set; }
    public string GeographyDisplayName { get; set; }
    public string GeographyAddressLine1 { get; set; }
    public string GeographyAddressLine2 { get; set; }
    public string GeographyPhoneNumber { get; set; }
    public string GeographyEmail { get; set; }

    public Dictionary<string, string> CustomFields { get; set; }
    public Dictionary<string, string> CustomLabels { get; set; }

    public List<MonthlyUsageChartDataDto> UsageChartDataDtos { get; set; }
    public string VegaSpec { get; set; }
}

public class UsageStatementSupplyTableRowDto
{
    public string SupplyType { get; set; }
    public decimal? StartingAllocation { get; set; }
    public string StartingAllocationFormatted { get; set; }
    public string UsageValueFormatted { get; set; }
    public string RemainingBalanceFormatted { get; set; }
}