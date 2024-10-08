using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class MergeWaterAccountsDto
{
    public int? PrimaryReportingPeriodYear { get; set; }
    [Required]
    public bool IsDeleteMerge { get; set; }
}