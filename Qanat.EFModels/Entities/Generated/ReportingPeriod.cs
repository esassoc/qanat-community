using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("ReportingPeriod")]
[Index("GeographyID", "Name", Name = "AK_ReportingPeroid_GeographyID_Name", IsUnique = true)]
public partial class ReportingPeriod
{
    [Key]
    public int ReportingPeriodID { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string Name { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EndDate { get; set; }

    public bool ReadyForAccountHolders { get; set; }

    public bool IsDefault { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CoverCropSelfReportStartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CoverCropSelfReportEndDate { get; set; }

    public bool CoverCropSelfReportReadyForAccountHolders { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FallowSelfReportStartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FallowSelfReportEndDate { get; set; }

    public bool FallowSelfReportReadyForAccountHolders { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public int CreateUserID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    public int? UpdateUserID { get; set; }

    [ForeignKey("CreateUserID")]
    [InverseProperty("ReportingPeriodCreateUsers")]
    public virtual User CreateUser { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("ReportingPeriods")]
    public virtual Geography Geography { get; set; }

    [InverseProperty("ReportingPeriod")]
    public virtual ICollection<ParcelWaterAccountHistory> ParcelWaterAccountHistories { get; set; } = new List<ParcelWaterAccountHistory>();

    [InverseProperty("ReportingPeriod")]
    public virtual ICollection<StatementBatch> StatementBatches { get; set; } = new List<StatementBatch>();

    [ForeignKey("UpdateUserID")]
    [InverseProperty("ReportingPeriodUpdateUsers")]
    public virtual User UpdateUser { get; set; }

    [InverseProperty("ReportingPeriod")]
    public virtual ICollection<UsageLocationParcelHistory> UsageLocationParcelHistories { get; set; } = new List<UsageLocationParcelHistory>();

    [InverseProperty("ReportingPeriod")]
    public virtual ICollection<UsageLocation> UsageLocations { get; set; } = new List<UsageLocation>();

    [InverseProperty("ReportingPeriod")]
    public virtual ICollection<WaterAccountCoverCropStatus> WaterAccountCoverCropStatuses { get; set; } = new List<WaterAccountCoverCropStatus>();

    [InverseProperty("ReportingPeriod")]
    public virtual ICollection<WaterAccountFallowStatus> WaterAccountFallowStatuses { get; set; } = new List<WaterAccountFallowStatus>();

    [InverseProperty("ReportingPeriod")]
    public virtual ICollection<WaterAccountParcel> WaterAccountParcels { get; set; } = new List<WaterAccountParcel>();

    [InverseProperty("ReportingPeriod")]
    public virtual ICollection<WaterMeasurementSelfReport> WaterMeasurementSelfReports { get; set; } = new List<WaterMeasurementSelfReport>();
}
