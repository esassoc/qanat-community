using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class MergeWaterAccountsDto
{
    //public int? PrimaryReportingPeriodYear { get; set; }

    public int? ReportingPeriodID { get; set; }

    [Required]
    public bool IsDeleteMerge { get; set; }
}