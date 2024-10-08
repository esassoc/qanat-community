using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Qanat.EFModels.Entities;

[Keyless]
public partial class vGeoserverGeographyGSABoundary
{
    public int PrimaryKey { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string GeographyName { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string GeographyDisplayName { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry GSABoundary { get; set; }

    [StringLength(9)]
    [Unicode(false)]
    public string GeographyColor { get; set; }
}
