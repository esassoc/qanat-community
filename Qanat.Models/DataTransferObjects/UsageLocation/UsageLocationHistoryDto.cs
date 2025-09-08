namespace Qanat.Models.DataTransferObjects;

public class UsageLocationHistoryDto
{
    public int UsageLocationHistoryID { get; set; }
    public int UsageLocationID { get; set; }
    public string UsageLocationName { get; set; }
    public string UsageLocationTypeName { get; set; }
    public string ReportingPeriodName { get; set; }
    public string Note { get; set; }
    public DateTime? CreateDate { get; set; }
    public string CreateUserFullName { get; set; }
}
