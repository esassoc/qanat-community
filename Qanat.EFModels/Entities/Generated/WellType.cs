using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WellType")]
public partial class WellType
{
    [Key]
    public int WellTypeID { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [StringLength(256)]
    [Unicode(false)]
    public string Name { get; set; }

    [Unicode(false)]
    public string SchemotoSchema { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    public int CreateUserID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    public int? UpdateUserID { get; set; }

    [ForeignKey("CreateUserID")]
    [InverseProperty("WellTypeCreateUsers")]
    public virtual User CreateUser { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("WellTypes")]
    public virtual Geography Geography { get; set; }

    [ForeignKey("UpdateUserID")]
    [InverseProperty("WellTypeUpdateUsers")]
    public virtual User UpdateUser { get; set; }

    [InverseProperty("WellType")]
    public virtual ICollection<Well> Wells { get; set; } = new List<Well>();
}
