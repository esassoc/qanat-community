using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Qanat.EFModels.Entities;

[Table("ModelBoundary")]
public partial class ModelBoundary
{
    [Key]
    public int ModelBoundaryID { get; set; }

    public int ModelID { get; set; }

    [Required]
    [Column(TypeName = "geometry")]
    public Geometry ModelBoundaryGeometry { get; set; }
}
