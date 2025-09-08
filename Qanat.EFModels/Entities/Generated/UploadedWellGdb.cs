using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("UploadedWellGdb")]
public partial class UploadedWellGdb
{
    [Key]
    public int UploadedWellGdbID { get; set; }

    public int UserID { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string CanonicalName { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime UploadDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EffectiveDate { get; set; }

    public bool Finalized { get; set; }

    public int SRID { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("UploadedWellGdbs")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("UserID")]
    [InverseProperty("UploadedWellGdbs")]
    public virtual User User { get; set; }
}
