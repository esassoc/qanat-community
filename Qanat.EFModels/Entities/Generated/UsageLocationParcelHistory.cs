using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("UsageLocationParcelHistory")]
[Index("CreateUserID", Name = "IX_ParcelWaterAccountHistory_CreateUserID")]
[Index("FromParcelID", Name = "IX_ParcelWaterAccountHistory_FromParcelID")]
[Index("ReportingPeriodID", Name = "IX_ParcelWaterAccountHistory_ReportingPeriodID")]
[Index("ToParcelID", Name = "IX_ParcelWaterAccountHistory_ToParcelID")]
[Index("GeographyID", Name = "IX_UsageLocationParcelHistory_GeographyID")]
[Index("UsageLocationID", Name = "IX_UsageLocationParcelHistory_UsageLocationID")]
public partial class UsageLocationParcelHistory
{
    [Key]
    public int UsageLocationParcelHistoryID { get; set; }

    public int GeographyID { get; set; }

    public int UsageLocationID { get; set; }

    public int ReportingPeriodID { get; set; }

    public int? FromParcelID { get; set; }

    [StringLength(64)]
    [Unicode(false)]
    public string FromParcelNumber { get; set; }

    public int? ToParcelID { get; set; }

    [StringLength(64)]
    [Unicode(false)]
    public string ToParcelNumber { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string Reason { get; set; }

    public int CreateUserID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [ForeignKey("CreateUserID")]
    [InverseProperty("UsageLocationParcelHistories")]
    public virtual User CreateUser { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("UsageLocationParcelHistories")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("ReportingPeriodID")]
    [InverseProperty("UsageLocationParcelHistories")]
    public virtual ReportingPeriod ReportingPeriod { get; set; }

    [ForeignKey("UsageLocationID")]
    [InverseProperty("UsageLocationParcelHistories")]
    public virtual UsageLocation UsageLocation { get; set; }
}
