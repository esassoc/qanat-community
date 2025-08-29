using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WellFileResource")]
public partial class WellFileResource
{
    [Key]
    public int WellFileResourceID { get; set; }

    public int WellID { get; set; }

    public int FileResourceID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string FileDescription { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("WellFileResources")]
    public virtual FileResource FileResource { get; set; }

    [ForeignKey("WellID")]
    [InverseProperty("WellFileResources")]
    public virtual Well Well { get; set; }
}
