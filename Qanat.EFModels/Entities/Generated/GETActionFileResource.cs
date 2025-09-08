using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("GETActionFileResource")]
public partial class GETActionFileResource
{
    [Key]
    public int GETActionFileResourceID { get; set; }

    public int GETActionID { get; set; }

    public int FileResourceID { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("GETActionFileResources")]
    public virtual FileResource FileResource { get; set; }

    [ForeignKey("GETActionID")]
    [InverseProperty("GETActionFileResources")]
    public virtual GETAction GETAction { get; set; }
}
