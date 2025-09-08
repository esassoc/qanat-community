using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("Meter")]
[Index("GeographyID", "SerialNumber", Name = "AK_Meter_GeographyID_SerialNumber", IsUnique = true)]
[Index("GeographyID", Name = "IX_Meter_GeographyID")]
public partial class Meter
{
    [Key]
    public int MeterID { get; set; }

    public int GeographyID { get; set; }

    public int MeterStatusID { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string SerialNumber { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string DeviceName { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Make { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string ModelNumber { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("Meters")]
    public virtual Geography Geography { get; set; }

    [InverseProperty("Meter")]
    public virtual ICollection<MeterReadingMonthlyInterpolation> MeterReadingMonthlyInterpolations { get; set; } = new List<MeterReadingMonthlyInterpolation>();

    [InverseProperty("Meter")]
    public virtual ICollection<MeterReading> MeterReadings { get; set; } = new List<MeterReading>();

    [InverseProperty("Meter")]
    public virtual ICollection<WellMeter> WellMeters { get; set; } = new List<WellMeter>();
}
