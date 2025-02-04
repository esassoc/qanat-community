namespace Qanat.Models.DataTransferObjects;

public class WaterMeasurementSelfReportDto
{
    public int WaterMeasurementSelfReportID { get; set; }
    public GeographySimpleDto Geography { get; set; }
    public WaterAccountSimpleDto WaterAccount { get; set; }
    public WaterMeasurementTypeSimpleDto WaterMeasurementType { get; set; }
    public int ReportingYear { get; set; }

    public WaterMeasurementSelfReportStatusSimpleDto WaterMeasurementSelfReportStatus { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? ReturnedDate { get; set; }

    public List<WaterMeasurementSelfReportLineItemSimpleDto> LineItems { get; set; }

    #region Create and Update Metadata

    public DateTime CreateDate { get; set; }
    public UserSimpleDto CreateUser { get; set; }
    public DateTime? UpdateDate { get; set; }
    public UserSimpleDto UpdateUser { get; set; }

    #endregion
}