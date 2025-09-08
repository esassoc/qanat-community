using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WellRegistrationIrrigatedParcel")]
[Index("WellRegistrationID", "ParcelID", Name = "AK_WellRegistrationIrrigatedParcel_WellRegistrationID_ParcelID", IsUnique = true)]
public partial class WellRegistrationIrrigatedParcel
{
    [Key]
    public int WellRegistrationIrrigatedParcelID { get; set; }

    public int WellRegistrationID { get; set; }

    public int ParcelID { get; set; }

    [ForeignKey("ParcelID")]
    [InverseProperty("WellRegistrationIrrigatedParcels")]
    public virtual Parcel Parcel { get; set; }

    [ForeignKey("WellRegistrationID")]
    [InverseProperty("WellRegistrationIrrigatedParcels")]
    public virtual WellRegistration WellRegistration { get; set; }
}
