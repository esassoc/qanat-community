using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WaterAccountParcel")]
[Index("GeographyID", "ParcelID", "ReportingPeriodID", Name = "AK_WaterAccountParcel_GeographyID_ParcelID_ReportingPeriodID", IsUnique = true)]
[Index("GeographyID", Name = "IX_WaterAccountParcel_GeographyID")]
[Index("ParcelID", Name = "IX_WaterAccountParcel_ParcelID")]
[Index("ReportingPeriodID", Name = "IX_WaterAccountParcel_ReportingPeriodID")]
[Index("WaterAccountID", Name = "IX_WaterAccountParcel_WaterAccountID")]
public partial class WaterAccountParcel
{
    [Key]
    public int WaterAccountParcelID { get; set; }

    public int GeographyID { get; set; }

    public int WaterAccountID { get; set; }

    public int ParcelID { get; set; }

    public int ReportingPeriodID { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("WaterAccountParcels")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("ParcelID")]
    [InverseProperty("WaterAccountParcels")]
    public virtual Parcel Parcel { get; set; }

    [ForeignKey("ReportingPeriodID")]
    [InverseProperty("WaterAccountParcels")]
    public virtual ReportingPeriod ReportingPeriod { get; set; }

    [ForeignKey("WaterAccountID")]
    [InverseProperty("WaterAccountParcels")]
    public virtual WaterAccount WaterAccount { get; set; }
}
