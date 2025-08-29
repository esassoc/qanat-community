using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("MeterReading")]
[Index("MeterID", "ReadingDate", Name = "AK_MeterReading_MeterID_ReadingDate", IsUnique = true)]
[Index("GeographyID", "WellID", "MeterID", "ReadingDate", Name = "IX_MeterReading_GeographyID_WellID_MeterID_ReadingDate")]
public partial class MeterReading
{
    [Key]
    public int MeterReadingID { get; set; }

    public int GeographyID { get; set; }

    public int WellID { get; set; }

    public int MeterID { get; set; }

    public int MeterReadingUnitTypeID { get; set; }

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

    [ForeignKey("GeographyID")]
    [InverseProperty("MeterReadings")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("MeterID")]
    [InverseProperty("MeterReadings")]
    public virtual Meter Meter { get; set; }

    [ForeignKey("WellID")]
    [InverseProperty("MeterReadings")]
    public virtual Well Well { get; set; }
}
