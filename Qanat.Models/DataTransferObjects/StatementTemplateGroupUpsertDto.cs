using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class StatementBatchUpsertDto
{
    [Required]
    public int? StatementTemplateID { get; set; }

    [Required]
    public int? ReportingPeriodID { get; set; }

    [Required]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string StatementBatchName { get; set; }

    public List<int> WaterAccountIDs { get; set; }
}