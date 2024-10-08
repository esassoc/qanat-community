using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("OpenETSyncHistory")]
public partial class OpenETSyncHistory
{
    [Key]
    public int OpenETSyncHistoryID { get; set; }

    public int OpenETSyncID { get; set; }

    public int OpenETSyncResultTypeID { get; set; }

    public int OpenETRasterCalculationResultTypeID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastCalculationDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastSuccessfulCalculationDate { get; set; }

    [Unicode(false)]
    public string LastCalculationErrorMessage { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime UpdateDate { get; set; }

    [Unicode(false)]
    public string ErrorMessage { get; set; }

    [StringLength(33)]
    [Unicode(false)]
    public string GoogleDriveRasterFileID { get; set; }

    public int? RasterFileResourceID { get; set; }

    [ForeignKey("OpenETSyncID")]
    [InverseProperty("OpenETSyncHistories")]
    public virtual OpenETSync OpenETSync { get; set; }

    [ForeignKey("RasterFileResourceID")]
    [InverseProperty("OpenETSyncHistories")]
    public virtual FileResource RasterFileResource { get; set; }
}
