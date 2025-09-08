using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WaterAccountUser")]
[Index("WaterAccountID", "UserID", Name = "AK_WaterAccountUser_Unique_WaterAccountID_UserID", IsUnique = true)]
public partial class WaterAccountUser
{
    [Key]
    public int WaterAccountUserID { get; set; }

    public int UserID { get; set; }

    public int WaterAccountID { get; set; }

    public int WaterAccountRoleID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ClaimDate { get; set; }

    [ForeignKey("UserID")]
    [InverseProperty("WaterAccountUsers")]
    public virtual User User { get; set; }

    [ForeignKey("WaterAccountID")]
    [InverseProperty("WaterAccountUsers")]
    public virtual WaterAccount WaterAccount { get; set; }
}
