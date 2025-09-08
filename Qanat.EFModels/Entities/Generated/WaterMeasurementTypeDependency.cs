using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WaterMeasurementTypeDependency")]
public partial class WaterMeasurementTypeDependency
{
    [Key]
    public int WaterMeasurementTypeDependencyID { get; set; }

    public int GeographyID { get; set; }

    public int WaterMeasurementTypeID { get; set; }

    public int DependsOnWaterMeasurementTypeID { get; set; }

    [ForeignKey("DependsOnWaterMeasurementTypeID")]
    [InverseProperty("WaterMeasurementTypeDependencyDependsOnWaterMeasurementTypes")]
    public virtual WaterMeasurementType DependsOnWaterMeasurementType { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("WaterMeasurementTypeDependencies")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("WaterMeasurementTypeID")]
    [InverseProperty("WaterMeasurementTypeDependencyWaterMeasurementTypes")]
    public virtual WaterMeasurementType WaterMeasurementType { get; set; }
}
