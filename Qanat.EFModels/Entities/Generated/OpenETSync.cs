using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("OpenETSync")]
public partial class OpenETSync
{
    [Key]
    public int OpenETSyncID { get; set; }

    public int GeographyID { get; set; }

    public int OpenETDataTypeID { get; set; }

    public int Year { get; set; }

    public int Month { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FinalizeDate { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("OpenETSyncs")]
    public virtual Geography Geography { get; set; }

    [InverseProperty("OpenETSync")]
    public virtual ICollection<OpenETSyncHistory> OpenETSyncHistories { get; set; } = new List<OpenETSyncHistory>();
}
