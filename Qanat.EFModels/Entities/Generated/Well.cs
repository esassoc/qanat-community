using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Qanat.EFModels.Entities;

[Table("Well")]
public partial class Well
{
    [Key]
    public int WellID { get; set; }

    public int GeographyID { get; set; }

    public int? ParcelID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string WellName { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry LocationPoint { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry LocationPoint4326 { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string StateWCRNumber { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string CountyWellPermitNumber { get; set; }

    public DateOnly? DateDrilled { get; set; }

    public int? WellDepth { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    public int WellStatusID { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string Notes { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("Wells")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("ParcelID")]
    [InverseProperty("Wells")]
    public virtual Parcel Parcel { get; set; }

    [InverseProperty("Well")]
    public virtual ICollection<WellIrrigatedParcel> WellIrrigatedParcels { get; set; } = new List<WellIrrigatedParcel>();

    [InverseProperty("Well")]
    public virtual ICollection<WellMeter> WellMeters { get; set; } = new List<WellMeter>();

    [InverseProperty("Well")]
    public virtual ICollection<WellRegistration> WellRegistrations { get; set; } = new List<WellRegistration>();
}
