using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WaterAccountParcel")]
[Index("WaterAccountID", "ParcelID", "EffectiveYear", Name = "AK_WaterAccountParcel_WaterAccountID_ParcelID_EffectiveYear", IsUnique = true)]
public partial class WaterAccountParcel
{
    [Key]
    public int WaterAccountParcelID { get; set; }

    public int GeographyID { get; set; }

    public int WaterAccountID { get; set; }

    public int ParcelID { get; set; }

    public int EffectiveYear { get; set; }

    public int? EndYear { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("WaterAccountParcels")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("ParcelID")]
    [InverseProperty("WaterAccountParcelParcels")]
    public virtual Parcel Parcel { get; set; }

    [ForeignKey("ParcelID, GeographyID")]
    [InverseProperty("WaterAccountParcelParcelNavigations")]
    public virtual Parcel ParcelNavigation { get; set; }

    [ForeignKey("WaterAccountID")]
    [InverseProperty("WaterAccountParcelWaterAccounts")]
    public virtual WaterAccount WaterAccount { get; set; }

    [ForeignKey("WaterAccountID, GeographyID")]
    [InverseProperty("WaterAccountParcelWaterAccountNavigations")]
    public virtual WaterAccount WaterAccountNavigation { get; set; }
}
