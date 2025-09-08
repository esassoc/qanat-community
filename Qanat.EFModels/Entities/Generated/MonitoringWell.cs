using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Qanat.EFModels.Entities;

[Table("MonitoringWell")]
public partial class MonitoringWell
{
    [Key]
    public int MonitoringWellID { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string SiteCode { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string MonitoringWellName { get; set; }

    public int MonitoringWellSourceTypeID { get; set; }

    [Required]
    [Column(TypeName = "geometry")]
    public Geometry Geometry { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("MonitoringWells")]
    public virtual Geography Geography { get; set; }

    [InverseProperty("MonitoringWell")]
    public virtual ICollection<MonitoringWellMeasurement> MonitoringWellMeasurements { get; set; } = new List<MonitoringWellMeasurement>();
}
