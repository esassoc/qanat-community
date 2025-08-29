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

    [Column(TypeName = "datetime")]
    public DateTime ReportedDate { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal? ReportedValueInNativeUnits { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal ReportedValueInAcreFeet { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal ReportedValueInFeet { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime LastUpdateDate { get; set; }

    public bool FromManualUpload { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string Comment { get; set; }

    public int WaterMeasurementTypeID { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string WaterMeasurementTypeName { get; set; }

    public int? UnitTypeID { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string UnitTypeDisplayName { get; set; }

    public int UsageLocationID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string UsageLocationName { get; set; }

    public double UsageLocationArea { get; set; }

    public int ReportingPeriodID { get; set; }

    public int UsageLocationTypeID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string UsageLocationTypeName { get; set; }

    public int ParcelID { get; set; }

    [Required]
    [StringLength(64)]
    [Unicode(false)]
    public string ParcelNumber { get; set; }

    public int? WaterAccountID { get; set; }

    public int? WaterAccountNumber { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string WaterAccountName { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string CoverCropStatus { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string FallowStatus { get; set; }
}
