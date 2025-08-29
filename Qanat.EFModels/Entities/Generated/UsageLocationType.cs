using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("UsageLocationType")]
[Index("GeographyID", "Name", Name = "AK_UsageLocationType_GeographyID_Name", IsUnique = true)]
public partial class UsageLocationType
{
    [Key]
    public int UsageLocationTypeID { get; set; }

    public int GeographyID { get; set; }

    public int? WaterMeasurementTypeID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string Name { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string Definition { get; set; }

    public bool CanBeRemoteSensed { get; set; }

    public bool IsIncludedInUsageCalculation { get; set; }

    public bool IsDefault { get; set; }

    [StringLength(7)]
    [Unicode(false)]
    public string ColorHex { get; set; }

    public int SortOrder { get; set; }

    public bool CanBeSelectedInCoverCropForm { get; set; }

    public bool CountsAsCoverCropped { get; set; }

    public bool CanBeSelectedInFallowForm { get; set; }

    public bool CountsAsFallowed { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public int CreateUserID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    public int? UpdateUserID { get; set; }

    [ForeignKey("CreateUserID")]
    [InverseProperty("UsageLocationTypeCreateUsers")]
    public virtual User CreateUser { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("UsageLocationTypes")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("UpdateUserID")]
    [InverseProperty("UsageLocationTypeUpdateUsers")]
    public virtual User UpdateUser { get; set; }

    [InverseProperty("UsageLocationType")]
    public virtual ICollection<UsageLocationHistory> UsageLocationHistories { get; set; } = new List<UsageLocationHistory>();

    [InverseProperty("UsageLocationType")]
    public virtual ICollection<UsageLocation> UsageLocations { get; set; } = new List<UsageLocation>();

    [ForeignKey("WaterMeasurementTypeID")]
    [InverseProperty("UsageLocationTypes")]
    public virtual WaterMeasurementType WaterMeasurementType { get; set; }
}
