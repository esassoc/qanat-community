namespace Qanat.Models.DataTransferObjects;

public class UsageLocationParcelHistoryDto
{
    public int UsageLocationParcelHistoryID { get; set; }
    public int GeographyID { get; set; }
    public int UsageLocationID { get; set; }
    public string UsageLocationName { get; set; }
    public int ReportingPeriodID { get; set; }
    public string ReportingPeriodName { get; set; }
    public int? FromParcelID { get; set; }
    public string FromParcelNumber { get; set; }
    public int? ToParcelID { get; set; }
    public string ToParcelNumber { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreateUserFullName { get; set; }
}