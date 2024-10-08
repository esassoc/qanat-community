using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Qanat.EFModels.Entities;

[Table("ParcelGeometry")]
[Index("ParcelID", Name = "AK_ParcelGeometry_ParcelID", IsUnique = true)]
[Index("Geometry4326", Name = "SPATIAL_ParcelGeometry_Geometry4326")]
public partial class ParcelGeometry
{
    [Key]
    public int ParcelGeometryID { get; set; }

    public int GeographyID { get; set; }

    public int ParcelID { get; set; }

    [Required]
    [Column(TypeName = "geometry")]
    public Geometry GeometryNative { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry Geometry4326 { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("ParcelGeometries")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("ParcelID")]
    [InverseProperty("ParcelGeometry")]
    public virtual Parcel Parcel { get; set; }
}
