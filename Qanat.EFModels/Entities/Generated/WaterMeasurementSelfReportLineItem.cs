using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WaterMeasurementSelfReportLineItem")]
[Index("WaterMeasurementSelfReportID", "ParcelID", Name = "AK_WaterMeasurementSelfReportLineItem_WaterMeasurementSelfReportID_UsageEntityID", IsUnique = true)]
public partial class WaterMeasurementSelfReportLineItem
{
    [Key]
    public int WaterMeasurementSelfReportLineItemID { get; set; }

    public int WaterMeasurementSelfReportID { get; set; }

    public int ParcelID { get; set; }

    public int IrrigationMethodID { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal? JanuaryOverrideValueInAcreFeet { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal? FebruaryOverrideValueInAcreFeet { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal? MarchOverrideValueInAcreFeet { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal? AprilOverrideValueInAcreFeet { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal? MayOverrideValueInAcreFeet { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal? JuneOverrideValueInAcreFeet { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal? JulyOverrideValueInAcreFeet { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal? AugustOverrideValueInAcreFeet { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal? SeptemberOverrideValueInAcreFeet { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal? OctoberOverrideValueInAcreFeet { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal? NovemberOverrideValueInAcreFeet { get; set; }

    [Column(TypeName = "decimal(20, 4)")]
    public decimal? DecemberOverrideValueInAcreFeet { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public int CreateUserID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    public int? UpdateUserID { get; set; }

    [ForeignKey("CreateUserID")]
    [InverseProperty("WaterMeasurementSelfReportLineItemCreateUsers")]
    public virtual User CreateUser { get; set; }

    [ForeignKey("IrrigationMethodID")]
    [InverseProperty("WaterMeasurementSelfReportLineItems")]
    public virtual IrrigationMethod IrrigationMethod { get; set; }

    [ForeignKey("ParcelID")]
    [InverseProperty("WaterMeasurementSelfReportLineItems")]
    public virtual Parcel Parcel { get; set; }

    [ForeignKey("UpdateUserID")]
    [InverseProperty("WaterMeasurementSelfReportLineItemUpdateUsers")]
    public virtual User UpdateUser { get; set; }

    [ForeignKey("WaterMeasurementSelfReportID")]
    [InverseProperty("WaterMeasurementSelfReportLineItems")]
    public virtual WaterMeasurementSelfReport WaterMeasurementSelfReport { get; set; }
}
