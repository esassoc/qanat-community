using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WaterMeasurementType")]
[Index("WaterMeasurementTypeName", "GeographyID", Name = "AK_WaterMeasurementType_WaterMeasurementTypeName_GeographyID", IsUnique = true)]
public partial class WaterMeasurementType
{
    [Key]
    public int WaterMeasurementTypeID { get; set; }

    public int GeographyID { get; set; }

    public int WaterMeasurementCategoryTypeID { get; set; }

    public bool IsActive { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string WaterMeasurementTypeName { get; set; }

    public int SortOrder { get; set; }

    public bool IsUserEditable { get; set; }

    public bool ShowToLandowner { get; set; }

    public int? WaterMeasurementCalculationTypeID { get; set; }

    [Unicode(false)]
    public string CalculationJSON { get; set; }

    [InverseProperty("SourceOfRecordWaterMeasurementType")]
    public virtual ICollection<Geography> Geographies { get; set; } = new List<Geography>();

    [ForeignKey("GeographyID")]
    [InverseProperty("WaterMeasurementTypes")]
    public virtual Geography Geography { get; set; }

    [InverseProperty("DependsOnWaterMeasurementType")]
    public virtual ICollection<WaterMeasurementTypeDependency> WaterMeasurementTypeDependencyDependsOnWaterMeasurementTypes { get; set; } = new List<WaterMeasurementTypeDependency>();

    [InverseProperty("WaterMeasurementType")]
    public virtual ICollection<WaterMeasurementTypeDependency> WaterMeasurementTypeDependencyWaterMeasurementTypes { get; set; } = new List<WaterMeasurementTypeDependency>();

    [InverseProperty("WaterMeasurementType")]
    public virtual ICollection<WaterMeasurement> WaterMeasurements { get; set; } = new List<WaterMeasurement>();
}
