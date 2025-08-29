using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WaterMeasurementType")]
[Index("GeographyID", "ShortName", Name = "AK_WaterMeasurementType_GeographyID_ShortName", IsUnique = true)]
[Index("GeographyID", "WaterMeasurementTypeName", Name = "AK_WaterMeasurementType_GeographyID_WaterMeasurementTypeName", IsUnique = true)]
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

    [Required]
    [StringLength(31)]
    [Unicode(false)]
    public string ShortName { get; set; }

    public int SortOrder { get; set; }

    public bool IsUserEditable { get; set; }

    public bool IsSelfReportable { get; set; }

    public bool ShowToLandowner { get; set; }

    public int? WaterMeasurementCalculationTypeID { get; set; }

    [Unicode(false)]
    public string CalculationJSON { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("WaterMeasurementTypes")]
    public virtual Geography Geography { get; set; }

    [InverseProperty("SourceOfRecordWaterMeasurementType")]
    public virtual ICollection<Geography> GeographySourceOfRecordWaterMeasurementTypes { get; set; } = new List<Geography>();

    [InverseProperty("WaterBudgetSlotAWaterMeasurementType")]
    public virtual ICollection<Geography> GeographyWaterBudgetSlotAWaterMeasurementTypes { get; set; } = new List<Geography>();

    [InverseProperty("WaterBudgetSlotBWaterMeasurementType")]
    public virtual ICollection<Geography> GeographyWaterBudgetSlotBWaterMeasurementTypes { get; set; } = new List<Geography>();

    [InverseProperty("WaterBudgetSlotCWaterMeasurementType")]
    public virtual ICollection<Geography> GeographyWaterBudgetSlotCWaterMeasurementTypes { get; set; } = new List<Geography>();

    [InverseProperty("WaterMeasurementType")]
    public virtual ICollection<UsageLocationType> UsageLocationTypes { get; set; } = new List<UsageLocationType>();

    [InverseProperty("WaterMeasurementType")]
    public virtual ICollection<WaterMeasurementSelfReport> WaterMeasurementSelfReports { get; set; } = new List<WaterMeasurementSelfReport>();

    [InverseProperty("DependsOnWaterMeasurementType")]
    public virtual ICollection<WaterMeasurementTypeDependency> WaterMeasurementTypeDependencyDependsOnWaterMeasurementTypes { get; set; } = new List<WaterMeasurementTypeDependency>();

    [InverseProperty("WaterMeasurementType")]
    public virtual ICollection<WaterMeasurementTypeDependency> WaterMeasurementTypeDependencyWaterMeasurementTypes { get; set; } = new List<WaterMeasurementTypeDependency>();

    [InverseProperty("WaterMeasurementType")]
    public virtual ICollection<WaterMeasurement> WaterMeasurements { get; set; } = new List<WaterMeasurement>();
}
