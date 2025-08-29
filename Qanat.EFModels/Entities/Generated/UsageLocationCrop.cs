using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("UsageLocationCrop")]
[Index("UsageLocationID", Name = "IX_UsageLocationCrop_UsageLocationID")]
public partial class UsageLocationCrop
{
    [Key]
    public int UsageLocationCropID { get; set; }

    public int UsageLocationID { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [ForeignKey("UsageLocationID")]
    [InverseProperty("UsageLocationCrops")]
    public virtual UsageLocation UsageLocation { get; set; }
}
