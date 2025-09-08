using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("ParcelWaterAccountHistory")]
[Index("CreateUserID", Name = "IX_ParcelWaterAccountHistory_CreateUserID")]
[Index("GeographyID", Name = "IX_ParcelWaterAccountHistory_GeographyID")]
[Index("ParcelID", Name = "IX_ParcelWaterAccountHistory_ParcelID")]
[Index("ReportingPeriodID", Name = "IX_ParcelWaterAccountHistory_ReportingPeriodID")]
public partial class ParcelWaterAccountHistory
{
    [Key]
    public int ParcelWaterAccountHistoryID { get; set; }

    public int GeographyID { get; set; }

    public int ParcelID { get; set; }

    public int ReportingPeriodID { get; set; }

    public int? FromWaterAccountID { get; set; }

    public int? FromWaterAccountNumber { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string FromWaterAccountName { get; set; }

    public int? ToWaterAccountID { get; set; }

    public int? ToWaterAccountNumber { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string ToWaterAccountName { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string Reason { get; set; }

    public int CreateUserID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [ForeignKey("CreateUserID")]
    [InverseProperty("ParcelWaterAccountHistories")]
    public virtual User CreateUser { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("ParcelWaterAccountHistories")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("ParcelID")]
    [InverseProperty("ParcelWaterAccountHistories")]
    public virtual Parcel Parcel { get; set; }

    [ForeignKey("ReportingPeriodID")]
    [InverseProperty("ParcelWaterAccountHistories")]
    public virtual ReportingPeriod ReportingPeriod { get; set; }
}
