using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("UsageLocationHistory")]
[Index("CreateUserID", Name = "IX_UsageLocationHistory_CreateUserID")]
[Index("GeographyID", Name = "IX_UsageLocationHistory_GeographyID")]
[Index("UsageLocationID", Name = "IX_UsageLocationHistory_UsageLocationID")]
[Index("UsageLocationTypeID", Name = "IX_UsageLocationHistory_UsageLocationTypeID")]
public partial class UsageLocationHistory
{
    [Key]
    public int UsageLocationHistoryID { get; set; }

    public int GeographyID { get; set; }

    public int UsageLocationID { get; set; }

    public int? UsageLocationTypeID { get; set; }

    [Unicode(false)]
    public string Note { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public int CreateUserID { get; set; }

    [ForeignKey("CreateUserID")]
    [InverseProperty("UsageLocationHistories")]
    public virtual User CreateUser { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("UsageLocationHistories")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("UsageLocationID")]
    [InverseProperty("UsageLocationHistories")]
    public virtual UsageLocation UsageLocation { get; set; }

    [ForeignKey("UsageLocationTypeID")]
    [InverseProperty("UsageLocationHistories")]
    public virtual UsageLocationType UsageLocationType { get; set; }
}
