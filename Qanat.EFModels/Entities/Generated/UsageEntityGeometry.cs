using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Qanat.EFModels.Entities;

[Table("UsageEntityGeometry")]
[Index("Geometry4326", Name = "SPATIAL_UsageEntityGeometry_Geometry4326")]
public partial class UsageEntityGeometry
{
    [Key]
    public int UsageEntityID { get; set; }

    [Required]
    [Column(TypeName = "geometry")]
    public Geometry GeometryNative { get; set; }

    [Required]
    [Column(TypeName = "geometry")]
    public Geometry Geometry4326 { get; set; }

    [ForeignKey("UsageEntityID")]
    [InverseProperty("UsageEntityGeometry")]
    public virtual UsageEntity UsageEntity { get; set; }
}
