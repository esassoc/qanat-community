using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("AllocationPlanPeriod")]
[Index("AllocationPlanID", "AllocationPeriodName", Name = "AK_AllocationPlanPeriod_AllocationPlanID_AllocationPeriodName", IsUnique = true)]
public partial class AllocationPlanPeriod
{
    [Key]
    public int AllocationPlanPeriodID { get; set; }

    public int AllocationPlanID { get; set; }

    [Required]
    [StringLength(250)]
    [Unicode(false)]
    public string AllocationPeriodName { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal AllocationAcreFeetPerAcre { get; set; }

    public int NumberOfYears { get; set; }

    public int StartYear { get; set; }

    public bool EnableCarryOver { get; set; }

    public int? CarryOverNumberOfYears { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? CarryOverDepreciationRate { get; set; }

    public bool EnableBorrowForward { get; set; }

    public int? BorrowForwardNumberOfYears { get; set; }

    [ForeignKey("AllocationPlanID")]
    [InverseProperty("AllocationPlanPeriods")]
    public virtual AllocationPlan AllocationPlan { get; set; }
}
