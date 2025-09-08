using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Keyless]
public class fReportingPeriod
{
    public int GeographyID { get; set; }
    public string GeographyName { get; set; }
    public string GeographyDisplayName { get; set; }
    public int ReportingPeriodID { get; set; }
    public string ReportingPeriodName { get; set; }
    public int StartMonth { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}