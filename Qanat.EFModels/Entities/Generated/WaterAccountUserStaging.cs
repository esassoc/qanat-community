using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WaterAccountUserStaging")]
public partial class WaterAccountUserStaging
{
    [Key]
    public int WaterAccountUserStagingID { get; set; }

    public int UserID { get; set; }

    public int WaterAccountID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ClaimDate { get; set; }

    [ForeignKey("UserID")]
    [InverseProperty("WaterAccountUserStagings")]
    public virtual User User { get; set; }

    [ForeignKey("WaterAccountID")]
    [InverseProperty("WaterAccountUserStagings")]
    public virtual WaterAccount WaterAccount { get; set; }
}
