using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("Zone")]
[Index("ZoneName", "ZoneGroupID", Name = "AK_Zone_Unique_ZoneName_ZoneGroupID", IsUnique = true)]
[Index("ZoneSlug", "ZoneGroupID", Name = "AK_Zone_Unique_ZoneSlug_ZoneGroupID", IsUnique = true)]
public partial class Zone
{
    [Key]
    public int ZoneID { get; set; }

    public int ZoneGroupID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string ZoneName { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string ZoneSlug { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string ZoneDescription { get; set; }

    [Required]
    [StringLength(7)]
    [Unicode(false)]
    public string ZoneColor { get; set; }

    [Required]
    [StringLength(7)]
    [Unicode(false)]
    public string ZoneAccentColor { get; set; }

    [Column(TypeName = "decimal(4, 2)")]
    public decimal? PrecipMultiplier { get; set; }

    public int SortOrder { get; set; }

    [InverseProperty("Zone")]
    public virtual ICollection<AllocationPlan> AllocationPlans { get; set; } = new List<AllocationPlan>();

    [InverseProperty("Zone")]
    public virtual ICollection<ParcelZone> ParcelZones { get; set; } = new List<ParcelZone>();

    [ForeignKey("ZoneGroupID")]
    [InverseProperty("Zones")]
    public virtual ZoneGroup ZoneGroup { get; set; }
}
