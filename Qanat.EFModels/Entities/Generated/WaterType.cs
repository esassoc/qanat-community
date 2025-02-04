using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WaterType")]
[Index("WaterTypeID", "GeographyID", Name = "AK_WaterType_Unique_WaterTypeID_GeographyID", IsUnique = true)]
[Index("WaterTypeName", "GeographyID", Name = "AK_WaterType_WaterTypeName_GeographyID", IsUnique = true)]
public partial class WaterType
{
    [Key]
    public int WaterTypeID { get; set; }

    public int GeographyID { get; set; }

    public bool IsActive { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string WaterTypeName { get; set; }

    public bool IsAppliedProportionally { get; set; }

    [Unicode(false)]
    public string WaterTypeDefinition { get; set; }

    public bool IsSourcedFromApi { get; set; }

    public int SortOrder { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string WaterTypeSlug { get; set; }

    [Required]
    [StringLength(7)]
    [Unicode(false)]
    public string WaterTypeColor { get; set; }

    [InverseProperty("WaterType")]
    public virtual ICollection<AllocationPlan> AllocationPlans { get; set; } = new List<AllocationPlan>();

    [ForeignKey("GeographyID")]
    [InverseProperty("WaterTypes")]
    public virtual Geography Geography { get; set; }

    [InverseProperty("WaterTypeNavigation")]
    public virtual ICollection<ParcelSupply> ParcelSupplyWaterTypeNavigations { get; set; } = new List<ParcelSupply>();

    [InverseProperty("WaterType")]
    public virtual ICollection<ParcelSupply> ParcelSupplyWaterTypes { get; set; } = new List<ParcelSupply>();
}
