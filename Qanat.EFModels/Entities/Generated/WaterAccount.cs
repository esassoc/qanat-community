using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WaterAccount")]
[Index("WaterAccountNumber", Name = "AK_Account_AccountNumber", IsUnique = true)]
[Index("WaterAccountID", "GeographyID", Name = "AK_WaterAccount_WaterAccountID_GeographyID", IsUnique = true)]
[Index("GeographyID", Name = "IX_WaterAccount_GeographyID")]
[Index("WaterAccountContactID", Name = "IX_WaterAccount_WaterAccountContactID")]
public partial class WaterAccount
{
    [Key]
    public int WaterAccountID { get; set; }

    public int GeographyID { get; set; }

    public int WaterAccountNumber { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string WaterAccountName { get; set; }

    [Unicode(false)]
    public string Notes { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    [StringLength(7)]
    [Unicode(false)]
    public string WaterAccountPIN { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public int? WaterAccountContactID { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("WaterAccounts")]
    public virtual Geography Geography { get; set; }

    [InverseProperty("WaterAccount")]
    public virtual ICollection<Parcel> Parcels { get; set; } = new List<Parcel>();

    [InverseProperty("WaterAccount")]
    public virtual ICollection<StatementBatchWaterAccount> StatementBatchWaterAccounts { get; set; } = new List<StatementBatchWaterAccount>();

    [InverseProperty("WaterAccount")]
    public virtual ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();

    [ForeignKey("WaterAccountContactID")]
    [InverseProperty("WaterAccounts")]
    public virtual WaterAccountContact WaterAccountContact { get; set; }

    [InverseProperty("WaterAccount")]
    public virtual ICollection<WaterAccountCoverCropStatus> WaterAccountCoverCropStatuses { get; set; } = new List<WaterAccountCoverCropStatus>();

    [InverseProperty("WaterAccount")]
    public virtual WaterAccountCustomAttribute WaterAccountCustomAttribute { get; set; }

    [InverseProperty("WaterAccount")]
    public virtual ICollection<WaterAccountFallowStatus> WaterAccountFallowStatuses { get; set; } = new List<WaterAccountFallowStatus>();

    [InverseProperty("WaterAccount")]
    public virtual ICollection<WaterAccountParcel> WaterAccountParcels { get; set; } = new List<WaterAccountParcel>();

    [InverseProperty("WaterAccountNavigation")]
    public virtual ICollection<WaterAccountReconciliation> WaterAccountReconciliationWaterAccountNavigations { get; set; } = new List<WaterAccountReconciliation>();

    [InverseProperty("WaterAccount")]
    public virtual ICollection<WaterAccountReconciliation> WaterAccountReconciliationWaterAccounts { get; set; } = new List<WaterAccountReconciliation>();

    [InverseProperty("WaterAccount")]
    public virtual ICollection<WaterAccountUserStaging> WaterAccountUserStagings { get; set; } = new List<WaterAccountUserStaging>();

    [InverseProperty("WaterAccount")]
    public virtual ICollection<WaterAccountUser> WaterAccountUsers { get; set; } = new List<WaterAccountUser>();

    [InverseProperty("WaterAccount")]
    public virtual ICollection<WaterMeasurementSelfReport> WaterMeasurementSelfReports { get; set; } = new List<WaterMeasurementSelfReport>();
}
