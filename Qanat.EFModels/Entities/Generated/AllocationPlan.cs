using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("AllocationPlan")]
[Index("ZoneID", "WaterTypeID", Name = "AK_AllocationPlan_ZoneID_WaterTypeID", IsUnique = true)]
public partial class AllocationPlan
{
    [Key]
    public int AllocationPlanID { get; set; }

    public int GeographyID { get; set; }

    public int GeographyAllocationPlanConfigurationID { get; set; }

    public int ZoneID { get; set; }

    public int WaterTypeID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime LastUpdated { get; set; }

    [InverseProperty("AllocationPlan")]
    public virtual ICollection<AllocationPlanPeriod> AllocationPlanPeriods { get; set; } = new List<AllocationPlanPeriod>();

    [ForeignKey("GeographyID")]
    [InverseProperty("AllocationPlans")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("GeographyAllocationPlanConfigurationID")]
    [InverseProperty("AllocationPlans")]
    public virtual GeographyAllocationPlanConfiguration GeographyAllocationPlanConfiguration { get; set; }

    [ForeignKey("WaterTypeID")]
    [InverseProperty("AllocationPlans")]
    public virtual WaterType WaterType { get; set; }

    [ForeignKey("ZoneID")]
    [InverseProperty("AllocationPlans")]
    public virtual Zone Zone { get; set; }
}
