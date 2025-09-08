using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("GeographyAllocationPlanConfiguration")]
[Index("GeographyID", Name = "AK_GeographyAllocationPlanConfiguration_GeographyID", IsUnique = true)]
public partial class GeographyAllocationPlanConfiguration
{
    [Key]
    public int GeographyAllocationPlanConfigurationID { get; set; }

    public int GeographyID { get; set; }

    public int ZoneGroupID { get; set; }

    public int StartYear { get; set; }

    public int EndYear { get; set; }

    public bool IsActive { get; set; }

    public bool IsVisibleToLandowners { get; set; }

    public bool? IsVisibleToPublic { get; set; }

    [Unicode(false)]
    public string AllocationPlansDescription { get; set; }

    [InverseProperty("GeographyAllocationPlanConfiguration")]
    public virtual ICollection<AllocationPlan> AllocationPlans { get; set; } = new List<AllocationPlan>();

    [ForeignKey("GeographyID")]
    [InverseProperty("GeographyAllocationPlanConfiguration")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("ZoneGroupID")]
    [InverseProperty("GeographyAllocationPlanConfigurations")]
    public virtual ZoneGroup ZoneGroup { get; set; }
}
