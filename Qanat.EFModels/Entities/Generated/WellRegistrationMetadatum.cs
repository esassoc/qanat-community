using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WellRegistrationMetadatum")]
[Index("WellRegistrationID", Name = "AK_WellRegistrationMetadatum_WellRegistrationID", IsUnique = true)]
public partial class WellRegistrationMetadatum
{
    [Key]
    public int WellRegistrationMetadatumID { get; set; }

    public int WellRegistrationID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string WellName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string StateWellNumber { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string StateWellCompletionNumber { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string CountyWellPermit { get; set; }

    public DateOnly? DateDrilled { get; set; }

    public int? WellDepth { get; set; }

    public int? CasingDiameter { get; set; }

    public int? TopOfPerforations { get; set; }

    public int? BottomOfPerforations { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string ManufacturerOfWaterMeter { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string SerialNumberOfWaterMeter { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string ElectricMeterNumber { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal? PumpDischargeDiameter { get; set; }

    [Column(TypeName = "decimal(10, 4)")]
    public decimal? MotorHorsePower { get; set; }

    public int? FuelTypeID { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string FuelOther { get; set; }

    public int? MaximumFlow { get; set; }

    public bool? IsEstimatedMax { get; set; }

    public int? TypicalPumpFlow { get; set; }

    public bool? IsEstimatedTypical { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string PumpTestBy { get; set; }

    public DateOnly? PumpTestDatePerformed { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string PumpManufacturer { get; set; }

    public int? PumpYield { get; set; }

    public int? PumpStaticLevel { get; set; }

    public int? PumpingLevel { get; set; }

    [ForeignKey("WellRegistrationID")]
    [InverseProperty("WellRegistrationMetadatum")]
    public virtual WellRegistration WellRegistration { get; set; }
}
