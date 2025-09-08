using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("IrrigationMethod")]
public partial class IrrigationMethod
{
    [Key]
    public int IrrigationMethodID { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string Name { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string SystemType { get; set; }

    public int EfficiencyAsPercentage { get; set; }

    public int DisplayOrder { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("IrrigationMethods")]
    public virtual Geography Geography { get; set; }

    [InverseProperty("IrrigationMethod")]
    public virtual ICollection<WaterMeasurementSelfReportLineItem> WaterMeasurementSelfReportLineItems { get; set; } = new List<WaterMeasurementSelfReportLineItem>();
}
