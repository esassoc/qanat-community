using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("ParcelHistory")]
[Index("GeographyID", Name = "IX_ParcelHistory_GeographyID")]
[Index("ParcelID", Name = "IX_ParcelHistory_ParcelID")]
[Index("ParcelStatusID", Name = "IX_ParcelHistory_ParcelStatusID")]
[Index("UpdateUserID", Name = "IX_ParcelHistory_UpdateUserID")]
public partial class ParcelHistory
{
    [Key]
    public int ParcelHistoryID { get; set; }

    public int GeographyID { get; set; }

    public int ParcelID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime UpdateDate { get; set; }

    public int UpdateUserID { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal ParcelArea { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string OwnerName { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string OwnerAddress { get; set; }

    public int ParcelStatusID { get; set; }

    public bool IsReviewed { get; set; }

    public bool IsManualOverride { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReviewDate { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("ParcelHistories")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("ParcelID")]
    [InverseProperty("ParcelHistories")]
    public virtual Parcel Parcel { get; set; }

    [ForeignKey("UpdateUserID")]
    [InverseProperty("ParcelHistories")]
    public virtual User UpdateUser { get; set; }
}
