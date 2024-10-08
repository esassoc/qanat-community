using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Keyless]
public partial class vWaterMeasurement
{
    public int WaterMeasurementID { get; set; }

    public int GeographyID { get; set; }

    public int? WaterMeasurementTypeID { get; set; }

    public int? UnitTypeID { get; set; }

    public int ParcelID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string UsageEntityName { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ReportedDate { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal ReportedValue { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal? ReportedValueInAcreFeet { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal? UsageEntityArea { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime LastUpdateDate { get; set; }

    public bool FromManualUpload { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string Comment { get; set; }
}
