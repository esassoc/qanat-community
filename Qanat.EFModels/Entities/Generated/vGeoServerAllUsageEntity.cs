using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Qanat.EFModels.Entities;

[Keyless]
public partial class vGeoServerAllUsageEntity
{
    public int UsageEntityID { get; set; }

    public int GeographyID { get; set; }

    public double UsageEntityArea { get; set; }

    [Required]
    [Column(TypeName = "geometry")]
    public Geometry Geometry4326 { get; set; }

    public int ParcelID { get; set; }

    public double ParcelArea { get; set; }

    public int? WaterAccountID { get; set; }
}
