using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WellMeter")]
public partial class WellMeter
{
    [Key]
    public int WellMeterID { get; set; }

    public int WellID { get; set; }

    public int MeterID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [ForeignKey("MeterID")]
    [InverseProperty("WellMeters")]
    public virtual Meter Meter { get; set; }

    [ForeignKey("WellID")]
    [InverseProperty("WellMeters")]
    public virtual Well Well { get; set; }
}
