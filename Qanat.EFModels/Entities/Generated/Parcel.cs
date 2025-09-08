using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("Parcel")]
[Index("ParcelID", "GeographyID", Name = "AK_Parcel_ParcelID_GeographyID", IsUnique = true)]
[Index("ParcelNumber", "GeographyID", Name = "AK_Parcel_ParcelNumber_GeographyID", IsUnique = true)]
[Index("GeographyID", Name = "IX_Parcel_GeographyID")]
[Index("ParcelStatusID", Name = "IX_Parcel_ParcelStatusID")]
[Index("WaterAccountID", Name = "IX_Parcel_WaterAccountID")]
public partial class Parcel
{
    [Key]
    public int ParcelID { get; set; }

    public int GeographyID { get; set; }

    public int? WaterAccountID { get; set; }

    [Required]
    [StringLength(64)]
    [Unicode(false)]
    public string ParcelNumber { get; set; }

    public double ParcelArea { get; set; }

    public int ParcelStatusID { get; set; }

    [Required]
    [StringLength(500)]
    [Unicode(false)]
    public string OwnerAddress { get; set; }

    [Required]
    [StringLength(500)]
    [Unicode(false)]
    public string OwnerName { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("Parcels")]
    public virtual Geography Geography { get; set; }

    [InverseProperty("Parcel")]
    public virtual ParcelCustomAttribute ParcelCustomAttribute { get; set; }

    [InverseProperty("Parcel")]
    public virtual ParcelGeometry ParcelGeometry { get; set; }

    [InverseProperty("Parcel")]
    public virtual ICollection<ParcelHistory> ParcelHistories { get; set; } = new List<ParcelHistory>();

    [InverseProperty("ParcelNavigation")]
    public virtual ICollection<ParcelSupply> ParcelSupplyParcelNavigations { get; set; } = new List<ParcelSupply>();

    [InverseProperty("Parcel")]
    public virtual ICollection<ParcelSupply> ParcelSupplyParcels { get; set; } = new List<ParcelSupply>();

    [InverseProperty("Parcel")]
    public virtual ICollection<ParcelWaterAccountHistory> ParcelWaterAccountHistories { get; set; } = new List<ParcelWaterAccountHistory>();

    [InverseProperty("Parcel")]
    public virtual ICollection<ParcelZone> ParcelZones { get; set; } = new List<ParcelZone>();

    [InverseProperty("Parcel")]
    public virtual ICollection<UsageLocation> UsageLocations { get; set; } = new List<UsageLocation>();

    [ForeignKey("WaterAccountID")]
    [InverseProperty("Parcels")]
    public virtual WaterAccount WaterAccount { get; set; }

    [InverseProperty("Parcel")]
    public virtual ICollection<WaterAccountParcel> WaterAccountParcels { get; set; } = new List<WaterAccountParcel>();

    [InverseProperty("ParcelNavigation")]
    public virtual ICollection<WaterAccountReconciliation> WaterAccountReconciliationParcelNavigations { get; set; } = new List<WaterAccountReconciliation>();

    [InverseProperty("Parcel")]
    public virtual ICollection<WaterAccountReconciliation> WaterAccountReconciliationParcels { get; set; } = new List<WaterAccountReconciliation>();

    [InverseProperty("Parcel")]
    public virtual ICollection<WaterMeasurementSelfReportLineItem> WaterMeasurementSelfReportLineItems { get; set; } = new List<WaterMeasurementSelfReportLineItem>();

    [InverseProperty("Parcel")]
    public virtual ICollection<WellIrrigatedParcel> WellIrrigatedParcels { get; set; } = new List<WellIrrigatedParcel>();

    [InverseProperty("Parcel")]
    public virtual ICollection<WellRegistrationIrrigatedParcel> WellRegistrationIrrigatedParcels { get; set; } = new List<WellRegistrationIrrigatedParcel>();

    [InverseProperty("Parcel")]
    public virtual ICollection<WellRegistration> WellRegistrations { get; set; } = new List<WellRegistration>();

    [InverseProperty("Parcel")]
    public virtual ICollection<Well> Wells { get; set; } = new List<Well>();
}
