using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WaterMeasurementSelfReport")]
[Index("GeographyID", "WaterAccountID", "WaterMeasurementTypeID", "ReportingPeriodID", Name = "AK_WaterMeasurementSelfReport_GeographyID_WaterAccountID_WaterMeasurementTypeID_ReportingPeriodID", IsUnique = true)]
public partial class WaterMeasurementSelfReport
{
    [Key]
    public int WaterMeasurementSelfReportID { get; set; }

    public int GeographyID { get; set; }

    public int WaterAccountID { get; set; }

    public int WaterMeasurementTypeID { get; set; }

    public int ReportingPeriodID { get; set; }

    public int WaterMeasurementSelfReportStatusID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? SubmittedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ApprovedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReturnedDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public int CreateUserID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    public int? UpdateUserID { get; set; }

    [ForeignKey("CreateUserID")]
    [InverseProperty("WaterMeasurementSelfReportCreateUsers")]
    public virtual User CreateUser { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("WaterMeasurementSelfReports")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("ReportingPeriodID")]
    [InverseProperty("WaterMeasurementSelfReports")]
    public virtual ReportingPeriod ReportingPeriod { get; set; }

    [ForeignKey("UpdateUserID")]
    [InverseProperty("WaterMeasurementSelfReportUpdateUsers")]
    public virtual User UpdateUser { get; set; }

    [ForeignKey("WaterAccountID")]
    [InverseProperty("WaterMeasurementSelfReports")]
    public virtual WaterAccount WaterAccount { get; set; }

    [InverseProperty("WaterMeasurementSelfReport")]
    public virtual ICollection<WaterMeasurementSelfReportFileResource> WaterMeasurementSelfReportFileResources { get; set; } = new List<WaterMeasurementSelfReportFileResource>();

    [InverseProperty("WaterMeasurementSelfReport")]
    public virtual ICollection<WaterMeasurementSelfReportLineItem> WaterMeasurementSelfReportLineItems { get; set; } = new List<WaterMeasurementSelfReportLineItem>();

    [ForeignKey("WaterMeasurementTypeID")]
    [InverseProperty("WaterMeasurementSelfReports")]
    public virtual WaterMeasurementType WaterMeasurementType { get; set; }
}
