namespace Qanat.Models.DataTransferObjects;

public class ReportingPeriodDto
{
    public int ReportingPeriodID { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool ReadyForAccountHolders { get; set; }
    public bool IsDefault { get; set; }

    public DateTime? CoverCropSelfReportStartDate { get; set; }
    public DateTime? CoverCropSelfReportEndDate { get; set; }
    public bool CoverCropSelfReportReadyForAccountHolders { get; set; }

    public DateTime? FallowSelfReportStartDate { get; set; }
    public DateTime? FallowSelfReportEndDate { get; set; }
    public bool FallowSelfReportReadyForAccountHolders { get; set; }


    public DateTime CreateDate { get; set; }
    public UserSimpleDto CreateUser { get; set; }
    public DateTime? UpdateDate { get; set; }
    public UserSimpleDto UpdateUser { get; set; }
}