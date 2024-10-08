using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("UsageEntityCrop")]
public partial class UsageEntityCrop
{
    [Key]
    public int UsageEntityCropID { get; set; }

    public int UsageEntityID { get; set; }

    [Required]
    [StringLength(100)]
    public string UsageEntityCropName { get; set; }

    [ForeignKey("UsageEntityID")]
    [InverseProperty("UsageEntityCrops")]
    public virtual UsageEntity UsageEntity { get; set; }
}
