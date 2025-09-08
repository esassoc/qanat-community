using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("CustomAttribute")]
public partial class CustomAttribute
{
    [Key]
    public int CustomAttributeID { get; set; }

    public int GeographyID { get; set; }

    public int CustomAttributeTypeID { get; set; }

    [Required]
    [StringLength(60)]
    [Unicode(false)]
    public string CustomAttributeName { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("CustomAttributes")]
    public virtual Geography Geography { get; set; }
}
