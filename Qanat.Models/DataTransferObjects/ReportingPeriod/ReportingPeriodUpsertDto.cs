using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class ReportingPeriodUpsertDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public DateTime? StartDate { get; set; }

    public bool? ReadyForAccountHolders { get; set; }
    public bool? IsDefault { get; set; }
}

public class ReportingPeriodCoverCropSelfReportMetadataUpdateDto
{
    public DateTime? CoverCropSelfReportStartDate { get; set; }
    public DateTime? CoverCropSelfReportEndDate { get; set; }
    public bool? CoverCropSelfReportReadyForAccountHolders { get; set; }
}

public class ReportingPeriodFallowSelfReportMetadataUpdateDto
{
    public DateTime? FallowSelfReportStartDate { get; set; }
    public DateTime? FallowSelfReportEndDate { get; set; }
    public bool? FallowSelfReportReadyForAccountHolders { get; set; }
}