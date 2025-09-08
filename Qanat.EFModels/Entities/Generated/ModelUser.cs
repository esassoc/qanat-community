using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("ModelUser")]
[Index("ModelID", "UserID", Name = "AK_ModelUser_ModelID_UserID", IsUnique = true)]
public partial class ModelUser
{
    [Key]
    public int ModelUserID { get; set; }

    public int ModelID { get; set; }

    public int UserID { get; set; }

    [ForeignKey("UserID")]
    [InverseProperty("ModelUsers")]
    public virtual User User { get; set; }
}
