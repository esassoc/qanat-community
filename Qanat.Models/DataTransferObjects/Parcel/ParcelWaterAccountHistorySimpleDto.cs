namespace Qanat.Models.DataTransferObjects;

public class ParcelWaterAccountHistorySimpleDto
{
    public int ParcelWaterAccountHistoryID { get; set; }
    public int GeographyID { get; set; }
    public int ParcelID { get; set; }
    public int ReportingPeriodID { get; set; }
    public int? FromWaterAccountID { get; set; }
    public int? FromWaterAccountNumber { get; set; }
    public string FromWaterAccountName { get; set; }
    public int? ToWaterAccountID { get; set; }
    public int? ToWaterAccountNumber { get; set; }
    public string ToWaterAccountName { get; set; }
    public string Reason { get; set; }
    public int CreateUserID { get; set; }
    public DateTime CreateDate { get; set; }
    public string ParcelNumber { get; set; }
    public string ReportingPeriodName { get; set; }
    public string CreateUserFullName { get; set; }

    public string FromWaterAccountNumberAndName => FromWaterAccountNumber != null
        ? FromWaterAccountName == null
            ? $"#{FromWaterAccountNumber}"
            : $"#{FromWaterAccountNumber} ({FromWaterAccountName})"
        : string.Empty;

    public string ToWaterAccountNumberAndName => ToWaterAccountNumber != null
        ? ToWaterAccountName == null
            ? $"#{ToWaterAccountNumber}"
            : $"#{ToWaterAccountNumber} ({ToWaterAccountName})"
        : string.Empty;
}
