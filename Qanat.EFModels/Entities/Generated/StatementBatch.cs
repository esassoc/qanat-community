using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("StatementBatch")]
[Index("GeographyID", "StatementBatchName", Name = "AK_StatementBatch_GeographyID_StatementBatchName", IsUnique = true)]
public partial class StatementBatch
{
    [Key]
    public int StatementBatchID { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string StatementBatchName { get; set; }

    public int StatementTemplateID { get; set; }

    public int ReportingPeriodID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime LastUpdated { get; set; }

    public int UpdateUserID { get; set; }

    public bool StatementsGenerated { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("StatementBatches")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("ReportingPeriodID")]
    [InverseProperty("StatementBatches")]
    public virtual ReportingPeriod ReportingPeriod { get; set; }

    [InverseProperty("StatementBatch")]
    public virtual ICollection<StatementBatchWaterAccount> StatementBatchWaterAccounts { get; set; } = new List<StatementBatchWaterAccount>();

    [ForeignKey("StatementTemplateID")]
    [InverseProperty("StatementBatches")]
    public virtual StatementTemplate StatementTemplate { get; set; }

    [ForeignKey("UpdateUserID")]
    [InverseProperty("StatementBatches")]
    public virtual User UpdateUser { get; set; }
}
