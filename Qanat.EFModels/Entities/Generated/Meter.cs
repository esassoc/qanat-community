using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("Meter")]
public partial class Meter
{
    [Key]
    public int MeterID { get; set; }

    [Required]
    [StringLength(25)]
    [Unicode(false)]
    public string SerialNumber { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string DeviceName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string Make { get; set; }

    [StringLength(25)]
    [Unicode(false)]
    public string ModelNumber { get; set; }

    public int GeographyID { get; set; }

    public int MeterStatusID { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("Meters")]
    public virtual Geography Geography { get; set; }

    [InverseProperty("Meter")]
    public virtual ICollection<WellMeter> WellMeters { get; set; } = new List<WellMeter>();
}
