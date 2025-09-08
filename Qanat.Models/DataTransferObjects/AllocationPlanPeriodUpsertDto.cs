using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class AllocationPlanPeriodUpsertDto
{
    [Required]
    public int AllocationPlanID { get; set; }
    public int AllocationPlanPeriodID { get; set; }
    [Required]
    public string AllocationPeriodName { get; set; }
    [Required]
    [Range(0, 1000)]
    public decimal AllocationAcreFeetPerAcre { get; set; }

    [Required]
    [Range(1, 100)]
    public int NumberOfYears { get; set; }
    [Required]
    public int StartYear { get; set; }
    [Required]
    public bool EnableCarryOver { get; set; }

    // todo: can we do conditional requirements? Might be hard to translate to typescript
    [Range(1, 100)]
    public int? CarryOverNumberOfYears { get; set; }

    //[Range(0.0,1.0)] todo: min and max
    [Range(0.0, 1.0)]
    public decimal? CarryOverDepreciationRate { get; set; }

    [Required]
    public bool EnableBorrowForward { get; set; }

    [Range(1, 100)]
    public int? BorrowForwardNumberOfYears { get; set; }

}