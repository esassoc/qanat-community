using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WaterAccountCoverCropStatus")]
[Index("GeographyID", "WaterAccountID", "ReportingPeriodID", Name = "AK_WaterAccountCoverCropStatus_GeographyID_WaterAccountID_ReportingPeriodID", IsUnique = true)]
public partial class WaterAccountCoverCropStatus
{
    [Key]
    public int WaterAccountCoverCropStatusID { get; set; }

    public int GeographyID { get; set; }

    public int WaterAccountID { get; set; }

    public int ReportingPeriodID { get; set; }

    public int SelfReportStatusID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? SubmittedDate { get; set; }

    public int? SubmittedByUserID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ApprovedDate { get; set; }

    public int? ApprovedByUserID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ReturnedDate { get; set; }

    public int? ReturnedByUserID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public int CreateUserID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    public int? UpdateUserID { get; set; }

    [ForeignKey("ApprovedByUserID")]
    [InverseProperty("WaterAccountCoverCropStatusApprovedByUsers")]
    public virtual User ApprovedByUser { get; set; }

    [ForeignKey("CreateUserID")]
    [InverseProperty("WaterAccountCoverCropStatusCreateUsers")]
    public virtual User CreateUser { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("WaterAccountCoverCropStatuses")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("ReportingPeriodID")]
    [InverseProperty("WaterAccountCoverCropStatuses")]
    public virtual ReportingPeriod ReportingPeriod { get; set; }

    [ForeignKey("ReturnedByUserID")]
    [InverseProperty("WaterAccountCoverCropStatusReturnedByUsers")]
    public virtual User ReturnedByUser { get; set; }

    [ForeignKey("SubmittedByUserID")]
    [InverseProperty("WaterAccountCoverCropStatusSubmittedByUsers")]
    public virtual User SubmittedByUser { get; set; }

    [ForeignKey("UpdateUserID")]
    [InverseProperty("WaterAccountCoverCropStatusUpdateUsers")]
    public virtual User UpdateUser { get; set; }

    [ForeignKey("WaterAccountID")]
    [InverseProperty("WaterAccountCoverCropStatuses")]
    public virtual WaterAccount WaterAccount { get; set; }
}
