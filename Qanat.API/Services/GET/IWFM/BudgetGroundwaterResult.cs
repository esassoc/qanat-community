using System.Collections.Generic;

namespace Qanat.API.Services.GET.IWFM;

public class BudgetGroundwaterResult
{
    public double StorageArea { get; set; }
    public List<BudgetGroundwaterPeriod> Periods { get; set; }
    public List<BudgetGroundwaterPeriod> BaselinePeriods { get; set; }
}