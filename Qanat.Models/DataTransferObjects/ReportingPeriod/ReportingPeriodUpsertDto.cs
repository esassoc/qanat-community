using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class ReportingPeriodUpsertDto
{
    [Required]
    public string Name { get; set; }

    [Required]
    public DateTime? StartDate { get; set; }

    public bool ReadyForAccountHolders { get; set; }
}