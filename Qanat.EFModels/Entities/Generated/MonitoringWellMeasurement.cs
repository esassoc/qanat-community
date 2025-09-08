using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("MonitoringWellMeasurement")]
[Index("GeographyID", Name = "IX_MonitoringWellMeasurement_GeographyID")]
[Index("MonitoringWellID", Name = "IX_MonitoringWellMeasurement_MonitoringWellID")]
public partial class MonitoringWellMeasurement
{
    [Key]
    public int MonitoringWellMeasurementID { get; set; }

    public int MonitoringWellID { get; set; }

    public int GeographyID { get; set; }

    public int ExtenalUniqueID { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Measurement { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime MeasurementDate { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("MonitoringWellMeasurements")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("MonitoringWellID")]
    [InverseProperty("MonitoringWellMeasurements")]
    public virtual MonitoringWell MonitoringWell { get; set; }
}
