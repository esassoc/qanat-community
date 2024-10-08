using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("ReportingPeriod")]
[Index("GeographyID", Name = "AK_GeographyID", IsUnique = true)]
public partial class ReportingPeriod
{
    [Key]
    public int ReportingPeriodID { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public string ReportingPeriodName { get; set; }

    public int StartMonth { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Interval { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("ReportingPeriod")]
    public virtual Geography Geography { get; set; }
}
