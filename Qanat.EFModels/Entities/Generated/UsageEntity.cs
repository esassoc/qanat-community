using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("UsageEntity")]
[Index("UsageEntityName", "GeographyID", Name = "AK_UsageEntity_UsageEntityName_GeographyID", IsUnique = true)]
public partial class UsageEntity
{
    [Key]
    public int UsageEntityID { get; set; }

    public int ParcelID { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string UsageEntityName { get; set; }

    public double UsageEntityArea { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("UsageEntities")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("ParcelID")]
    [InverseProperty("UsageEntities")]
    public virtual Parcel Parcel { get; set; }

    [InverseProperty("UsageEntity")]
    public virtual ICollection<UsageEntityCrop> UsageEntityCrops { get; set; } = new List<UsageEntityCrop>();

    [InverseProperty("UsageEntity")]
    public virtual UsageEntityGeometry UsageEntityGeometry { get; set; }
}
