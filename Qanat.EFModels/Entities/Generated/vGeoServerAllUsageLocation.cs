using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Qanat.EFModels.Entities;

[Keyless]
public partial class vGeoServerAllUsageLocation
{
    public int UsageLocationID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string UsageLocationName { get; set; }

    public int GeographyID { get; set; }

    public double Area { get; set; }

    public int ReportingPeriodID { get; set; }

    [Required]
    [Column(TypeName = "geometry")]
    public Geometry Geometry4326 { get; set; }

    [StringLength(7)]
    [Unicode(false)]
    public string UsageLocationTypeColor { get; set; }

    public int ParcelID { get; set; }

    public double ParcelArea { get; set; }

    public int? WaterAccountID { get; set; }

    public bool? IsCurrent { get; set; }
}
