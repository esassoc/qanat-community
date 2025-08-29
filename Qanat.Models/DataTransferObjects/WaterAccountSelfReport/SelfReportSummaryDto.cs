namespace Qanat.Models.DataTransferObjects;

public class SelfReportSummaryDto
{
    public int ReportingPeriodID { get; set; }
    public string ReportingPeriodName { get; set; }
    public string SelfReportType { get; set; }
    public int NotStartedCount { get; set; }
    public int InProgressCount { get; set; }
    public int SubmittedCount { get; set; }
    public int ReturnedCount { get; set; }
    public int ApprovedCount { get; set; }
}