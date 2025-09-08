using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("UploadedGdb")]
public partial class UploadedGdb
{
    [Key]
    public int UploadedGdbID { get; set; }

    public int UserID { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string CanonicalName { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime UploadDate { get; set; }

    public int? EffectiveYear { get; set; }

    public bool Finalized { get; set; }

    public int SRID { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("UploadedGdbs")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("UserID")]
    [InverseProperty("UploadedGdbs")]
    public virtual User User { get; set; }
}
