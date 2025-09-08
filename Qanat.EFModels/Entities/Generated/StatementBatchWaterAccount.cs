using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("StatementBatchWaterAccount")]
public partial class StatementBatchWaterAccount
{
    [Key]
    public int StatementBatchWaterAccountID { get; set; }

    public int StatementBatchID { get; set; }

    public int WaterAccountID { get; set; }

    public int? FileResourceID { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("StatementBatchWaterAccounts")]
    public virtual FileResource FileResource { get; set; }

    [ForeignKey("StatementBatchID")]
    [InverseProperty("StatementBatchWaterAccounts")]
    public virtual StatementBatch StatementBatch { get; set; }

    [ForeignKey("WaterAccountID")]
    [InverseProperty("StatementBatchWaterAccounts")]
    public virtual WaterAccount WaterAccount { get; set; }
}
