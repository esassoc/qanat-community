using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("MeterReadingMonthlyInterpolation")]
[Index("MeterID", "Date", Name = "AK_MeterReadingMonthlyInterpolation_MeterID_Date", IsUnique = true)]
[Index("GeographyID", "WellID", "MeterID", "Date", Name = "IX_MeterReadingMonthlyInterpolation_GeographyID_WellID_MeterID_Date")]
public partial class MeterReadingMonthlyInterpolation
{
    [Key]
    public int MeterReadingMonthlyInterpolationID { get; set; }

    public int GeographyID { get; set; }

    public int WellID { get; set; }

    public int MeterID { get; set; }

    public int MeterReadingUnitTypeID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal InterpolatedVolume { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal InterpolatedVolumeInAcreFeet { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("MeterReadingMonthlyInterpolations")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("MeterID")]
    [InverseProperty("MeterReadingMonthlyInterpolations")]
    public virtual Meter Meter { get; set; }

    [ForeignKey("WellID")]
    [InverseProperty("MeterReadingMonthlyInterpolations")]
    public virtual Well Well { get; set; }
}
