using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("UsageLocation")]
[Index("GeographyID", "ReportingPeriodID", "Name", Name = "AK_UsageLocation_GeographyID_ReportingPeriodID_Name", IsUnique = true)]
[Index("CreateUserID", Name = "IX_UsageLocation_CreateUserID")]
[Index("GeographyID", "ParcelID", Name = "IX_UsageLocation_GeographyID_ParcelID")]
[Index("ReportingPeriodID", Name = "IX_UsageLocation_ReportingPeriodID")]
[Index("UpdateUserID", Name = "IX_UsageLocation_UpdateUserID")]
[Index("UsageLocationTypeID", Name = "IX_UsageLocation_UsageLocationTypeID")]
public partial class UsageLocation
{
    [Key]
    public int UsageLocationID { get; set; }

    public int GeographyID { get; set; }

    public int? UsageLocationTypeID { get; set; }

    public int ParcelID { get; set; }

    public int ReportingPeriodID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string Name { get; set; }

    public double Area { get; set; }

    [Unicode(false)]
    public string CoverCropNote { get; set; }

    [Unicode(false)]
    public string FallowNote { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public int CreateUserID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    public int? UpdateUserID { get; set; }

    [ForeignKey("CreateUserID")]
    [InverseProperty("UsageLocationCreateUsers")]
    public virtual User CreateUser { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("UsageLocations")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("ParcelID")]
    [InverseProperty("UsageLocations")]
    public virtual Parcel Parcel { get; set; }

    [ForeignKey("ReportingPeriodID")]
    [InverseProperty("UsageLocations")]
    public virtual ReportingPeriod ReportingPeriod { get; set; }

    [ForeignKey("UpdateUserID")]
    [InverseProperty("UsageLocationUpdateUsers")]
    public virtual User UpdateUser { get; set; }

    [InverseProperty("UsageLocation")]
    public virtual ICollection<UsageLocationCrop> UsageLocationCrops { get; set; } = new List<UsageLocationCrop>();

    [InverseProperty("UsageLocation")]
    public virtual UsageLocationGeometry UsageLocationGeometry { get; set; }

    [InverseProperty("UsageLocation")]
    public virtual ICollection<UsageLocationHistory> UsageLocationHistories { get; set; } = new List<UsageLocationHistory>();

    [InverseProperty("UsageLocation")]
    public virtual ICollection<UsageLocationParcelHistory> UsageLocationParcelHistories { get; set; } = new List<UsageLocationParcelHistory>();

    [ForeignKey("UsageLocationTypeID")]
    [InverseProperty("UsageLocations")]
    public virtual UsageLocationType UsageLocationType { get; set; }

    [InverseProperty("UsageLocation")]
    public virtual ICollection<WaterMeasurement> WaterMeasurements { get; set; } = new List<WaterMeasurement>();
}
