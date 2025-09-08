using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Qanat.EFModels.Entities;

[Keyless]
public partial class vGeoServerMonitoringWell
{
    public int PrimaryKey { get; set; }

    public int GeographyID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string MonitoringWellName { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string SiteCode { get; set; }

    public int MonitoringWellSourceTypeID { get; set; }

    [Required]
    [Column(TypeName = "geometry")]
    public Geometry Geometry { get; set; }
}
