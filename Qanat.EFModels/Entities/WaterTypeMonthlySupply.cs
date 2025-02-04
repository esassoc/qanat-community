using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Keyless]
public class WaterTypeMonthlySupply
{
    public WaterTypeMonthlySupply()
    {
    }

    public int WaterTypeID { get; set; }
    public string WaterTypeName { get; set; }
    public string WaterTypeColor { get; set; }
    public int WaterTypeSortOrder { get; set; }
    public decimal? CurrentSupplyAmount { get; set; }
    public decimal? CurrentCumulativeSupplyAmount { get; set; }
    public double? CurrentCumulativeSupplyAmountDepth { get; set; }
    public DateTime EffectiveDate { get; set; }
    public int WaterAccountID { get; set; }
}