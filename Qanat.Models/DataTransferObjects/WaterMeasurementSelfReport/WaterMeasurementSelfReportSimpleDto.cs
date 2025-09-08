namespace Qanat.Models.DataTransferObjects;

public class WaterMeasurementSelfReportSimpleDto
{
    public int WaterMeasurementSelfReportID { get; set; }
    public int GeographyID { get; set; }
    public int WaterAccountID { get; set; }
    public int WaterMeasurementTypeID { get; set; }
    public int ReportingPeriodID { get; set; }
    public int WaterMeasurementSelfReportStatusID { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? ReturnedDate { get; set; }
    public DateTime CreateDate { get; set; }
    public int CreateUserID { get; set; }
    public DateTime? UpdateDate { get; set; }
    public int? UpdateUserID { get; set; }
    public string WaterAccountNumberAndName { get; set; }
    public string ReportingPeriodName { get; set; }
    public string WaterMeasurementTypeName { get; set; }
    public string WaterMeasurementSelfReportStatusDisplayName { get; set; }
    public decimal? TotalVolume { get; set; }
    public int FileCount { get; set; }
    public string CreateUserFullName { get; set; }
    public string UpdateUserFullName { get; set; }
}