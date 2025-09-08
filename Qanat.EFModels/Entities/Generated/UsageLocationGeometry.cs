using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Qanat.EFModels.Entities;

[Table("UsageLocationGeometry")]
[Index("UsageLocationID", Name = "IX_UsageLocationGeometry_UsageLocationID")]
[Index("Geometry4326", Name = "SPATIAL_UsageLocationGeometry_Geometry4326")]
public partial class UsageLocationGeometry
{
    [Key]
    public int UsageLocationID { get; set; }

    [Required]
    [Column(TypeName = "geometry")]
    public Geometry GeometryNative { get; set; }

    [Required]
    [Column(TypeName = "geometry")]
    public Geometry Geometry4326 { get; set; }

    [ForeignKey("UsageLocationID")]
    [InverseProperty("UsageLocationGeometry")]
    public virtual UsageLocation UsageLocation { get; set; }
}
