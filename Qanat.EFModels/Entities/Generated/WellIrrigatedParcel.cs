using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WellIrrigatedParcel")]
[Index("WellID", "ParcelID", Name = "AK_WellIrrigatedParcel_WellID_ParcelID", IsUnique = true)]
public partial class WellIrrigatedParcel
{
    [Key]
    public int WellIrrigatedParcelID { get; set; }

    public int WellID { get; set; }

    public int ParcelID { get; set; }

    [ForeignKey("ParcelID")]
    [InverseProperty("WellIrrigatedParcels")]
    public virtual Parcel Parcel { get; set; }

    [ForeignKey("WellID")]
    [InverseProperty("WellIrrigatedParcels")]
    public virtual Well Well { get; set; }
}
