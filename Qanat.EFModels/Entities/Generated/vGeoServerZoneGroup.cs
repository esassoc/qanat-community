using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Qanat.EFModels.Entities;

[Keyless]
public partial class vGeoServerZoneGroup
{
    public int ParcelID { get; set; }

    public int ZoneGroupID { get; set; }

    public int ZoneID { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string ParcelNumber { get; set; }

    public double ParcelArea { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry ParcelGeometry { get; set; }

    [Required]
    [StringLength(7)]
    [Unicode(false)]
    public string ZoneColor { get; set; }
}
