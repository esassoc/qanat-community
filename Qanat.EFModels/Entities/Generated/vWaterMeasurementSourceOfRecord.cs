using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Keyless]
public partial class vWaterMeasurementSourceOfRecord
{
    public int WaterMeasurementID { get; set; }

    public int GeographyID { get; set; }

    public int? WaterMeasurementTypeID { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string WaterMeasurementTypeName { get; set; }

    public int? UnitTypeID { get; set; }

    public int ParcelID { get; set; }

    public int UsageLocationID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string UsageLocationName { get; set; }

    public int ReportingPeriodID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ReportedDate { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal? ReportedValueInNativeUnits { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal ReportedValueInAcreFeet { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal ReportedValueInFeet { get; set; }

    public double UsageLocationArea { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime LastUpdateDate { get; set; }

    public bool FromManualUpload { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string Comment { get; set; }
}
