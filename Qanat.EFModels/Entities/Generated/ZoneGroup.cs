using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("ZoneGroup")]
[Index("ZoneGroupName", "GeographyID", Name = "AK_ZoneGroup_Unique_ZoneGroupName_GeographyID", IsUnique = true)]
[Index("ZoneGroupSlug", "GeographyID", Name = "AK_ZoneGroup_Unique_ZoneGroupSlug_GeographyID", IsUnique = true)]
public partial class ZoneGroup
{
    [Key]
    public int ZoneGroupID { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string ZoneGroupName { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string ZoneGroupSlug { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string ZoneGroupDescription { get; set; }

    public int SortOrder { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("ZoneGroups")]
    public virtual Geography Geography { get; set; }

    [InverseProperty("ZoneGroup")]
    public virtual ICollection<GeographyAllocationPlanConfiguration> GeographyAllocationPlanConfigurations { get; set; } = new List<GeographyAllocationPlanConfiguration>();

    [InverseProperty("ZoneGroup")]
    public virtual ICollection<Zone> Zones { get; set; } = new List<Zone>();
}
