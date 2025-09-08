using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("ParcelZone")]
[Index("ParcelID", "ZoneID", Name = "AK_ParcelZone_Unique_ParcelID_ZoneID", IsUnique = true)]
[Index("ParcelID", Name = "IX_ParcelZone_ParcelID")]
[Index("ZoneID", Name = "IX_ParcelZone_ZoneID")]
public partial class ParcelZone
{
    [Key]
    public int ParcelZoneID { get; set; }

    public int ZoneID { get; set; }

    public int ParcelID { get; set; }

    [ForeignKey("ParcelID")]
    [InverseProperty("ParcelZones")]
    public virtual Parcel Parcel { get; set; }

    [ForeignKey("ZoneID")]
    [InverseProperty("ParcelZones")]
    public virtual Zone Zone { get; set; }
}
