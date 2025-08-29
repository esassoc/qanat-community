namespace Qanat.Models.DataTransferObjects;

public class WaterAccountCoverCropStatusDto
{
    public int? WaterAccountCoverCropStatusID { get; set; }
    public GeographyMinimalDto Geography { get; set; }
    public SelfReportStatusSimpleDto SelfReportStatus { get; set; }
    public ReportingPeriodSimpleDto ReportingPeriod { get; set; }
    public WaterAccountMinimalDto WaterAccount { get; set; }
    public List<UsageLocationDto> UsageLocations { get; set; }

    public int CountOfCoverCroppedUsageLocations => UsageLocations.Count(x => x.UsageLocationType.CountsAsCoverCropped);
    public double AcresCoverCropped => UsageLocations.Where(x => x.UsageLocationType.CountsAsCoverCropped).Sum(x => x.Area);

    public bool CurrentUserCanEdit { get; set; }

    public UserWithFullNameDto SubmittedByUser { get; set; }
    public DateTime? SubmittedDate { get; set; }

    public UserWithFullNameDto ApprovedByUser { get; set; }
    public DateTime? ApprovedDate { get; set; }

    public UserWithFullNameDto ReturnedByUser { get; set; }
    public DateTime? ReturnedDate { get; set; }

    public UserWithFullNameDto CreateUser { get; set; }
    public DateTime? CreateDate { get; set; }
    public UserWithFullNameDto UpdateUser { get; set; }
    public DateTime? UpdateDate { get; set; }
}