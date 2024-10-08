using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WaterAccountCustomAttribute")]
[Index("WaterAccountID", Name = "AK_WaterAccountCustomAttribute_WaterAccount", IsUnique = true)]
public partial class WaterAccountCustomAttribute
{
    [Key]
    public int WaterAccountCustomAttributeID { get; set; }

    public int WaterAccountID { get; set; }

    [Required]
    [Unicode(false)]
    public string CustomAttributes { get; set; }

    [ForeignKey("WaterAccountID")]
    [InverseProperty("WaterAccountCustomAttribute")]
    public virtual WaterAccount WaterAccount { get; set; }
}
