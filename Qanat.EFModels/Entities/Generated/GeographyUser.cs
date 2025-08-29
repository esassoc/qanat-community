using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("GeographyUser")]
[Index("GeographyID", "UserID", Name = "AK_GeographyUser_Unique_GeographyID_UserID", IsUnique = true)]
public partial class GeographyUser
{
    [Key]
    public int GeographyUserID { get; set; }

    public int GeographyID { get; set; }

    public int UserID { get; set; }

    public int GeographyRoleID { get; set; }

    public bool ReceivesNotifications { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("GeographyUsers")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("UserID")]
    [InverseProperty("GeographyUsers")]
    public virtual User User { get; set; }
}
