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

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    public int CreateUserID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    public int? UpdateUserID { get; set; }

    [ForeignKey("CreateUserID")]
    [InverseProperty("ReportingPeriodCreateUsers")]
    public virtual User CreateUser { get; set; }

    [InverseProperty("DefaultReportingPeriod")]
    public virtual ICollection<Geography> Geographies { get; set; } = new List<Geography>();

    [ForeignKey("GeographyID")]
    [InverseProperty("ReportingPeriods")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("UpdateUserID")]
    [InverseProperty("ReportingPeriodUpdateUsers")]
    public virtual User UpdateUser { get; set; }
}
