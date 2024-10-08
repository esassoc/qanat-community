using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("ParcelSupply")]
public partial class ParcelSupply
{
    [Key]
    public int ParcelSupplyID { get; set; }

    public int GeographyID { get; set; }

    public int ParcelID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime TransactionDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EffectiveDate { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal TransactionAmount { get; set; }

    public int? WaterTypeID { get; set; }

    public int? UserID { get; set; }

    [Unicode(false)]
    public string UserComment { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string UploadedFileName { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("ParcelSupplies")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("ParcelID")]
    [InverseProperty("ParcelSupplyParcels")]
    public virtual Parcel Parcel { get; set; }

    [ForeignKey("ParcelID, GeographyID")]
    [InverseProperty("ParcelSupplyParcelNavigations")]
    public virtual Parcel ParcelNavigation { get; set; }

    [ForeignKey("UserID")]
    [InverseProperty("ParcelSupplies")]
    public virtual User User { get; set; }

    [ForeignKey("WaterTypeID")]
    [InverseProperty("ParcelSupplyWaterTypes")]
    public virtual WaterType WaterType { get; set; }

    [ForeignKey("WaterTypeID, GeographyID")]
    [InverseProperty("ParcelSupplyWaterTypeNavigations")]
    public virtual WaterType WaterTypeNavigation { get; set; }
}
