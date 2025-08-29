using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Qanat.EFModels.Entities;

[Keyless]
public partial class vGeoServerAllParcel
{
    public int ParcelID { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [StringLength(64)]
    [Unicode(false)]
    public string ParcelNumber { get; set; }

    public double ParcelArea { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry ParcelGeometry { get; set; }

    public int? CurrentWaterAccountID { get; set; }

    public int ParcelStatusID { get; set; }

    public int? ReportingPeriodID { get; set; }

    public int? WaterAccountID { get; set; }

    public bool? IsCurrent { get; set; }
}
