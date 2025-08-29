using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Keyless]
public partial class vMeterReading
{
    public int GeographyID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string GeographyDisplayName { get; set; }

    public int WellID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string WellName { get; set; }

    public int MeterID { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string SerialNumber { get; set; }

    public int MeterReadingUnitTypeID { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string MeterReadingUnitTypeDisplayName { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ReadingDate { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal PreviousReading { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal CurrentReading { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal Volume { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal VolumeInAcreFeet { get; set; }

    [StringLength(5)]
    [Unicode(false)]
    public string ReaderInitials { get; set; }

    [Unicode(false)]
    public string Comment { get; set; }
}
