using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WaterAccountReconciliation")]
[Index("ParcelID", "WaterAccountID", Name = "AK_WaterAccountReconciliation_ParcelID_WaterAccountID", IsUnique = true)]
public partial class WaterAccountReconciliation
{
    [Key]
    public int WaterAccountReconciliationID { get; set; }

    public int GeographyID { get; set; }

    public int ParcelID { get; set; }

    public int WaterAccountID { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("WaterAccountReconciliations")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("ParcelID")]
    [InverseProperty("WaterAccountReconciliationParcels")]
    public virtual Parcel Parcel { get; set; }

    [ForeignKey("ParcelID, GeographyID")]
    [InverseProperty("WaterAccountReconciliationParcelNavigations")]
    public virtual Parcel ParcelNavigation { get; set; }

    [ForeignKey("WaterAccountID")]
    [InverseProperty("WaterAccountReconciliationWaterAccounts")]
    public virtual WaterAccount WaterAccount { get; set; }

    [ForeignKey("WaterAccountID, GeographyID")]
    [InverseProperty("WaterAccountReconciliationWaterAccountNavigations")]
    public virtual WaterAccount WaterAccountNavigation { get; set; }
}
