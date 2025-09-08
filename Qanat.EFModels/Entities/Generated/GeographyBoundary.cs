using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Qanat.EFModels.Entities;

[Table("GeographyBoundary")]
[Index("GeographyID", Name = "AK_GeographyBoundary_GeographyID", IsUnique = true)]
public partial class GeographyBoundary
{
    [Key]
    public int GeographyBoundaryID { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [Column(TypeName = "geometry")]
    public Geometry BoundingBox { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry GSABoundary { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? GSABoundaryLastUpdated { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("GeographyBoundary")]
    public virtual Geography Geography { get; set; }
}
