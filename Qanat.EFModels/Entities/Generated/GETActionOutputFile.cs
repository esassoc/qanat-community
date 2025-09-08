using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("GETActionOutputFile")]
[Index("GETActionOutputFileTypeID", "GETActionID", Name = "AK_GETActionOutputFile_Unique_GETActionOutputFileTypeID_GETActionID", IsUnique = true)]
public partial class GETActionOutputFile
{
    [Key]
    public int GETActionOutputFileID { get; set; }

    public int GETActionOutputFileTypeID { get; set; }

    public int GETActionID { get; set; }

    public int FileResourceID { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("GETActionOutputFiles")]
    public virtual FileResource FileResource { get; set; }

    [ForeignKey("GETActionID")]
    [InverseProperty("GETActionOutputFiles")]
    public virtual GETAction GETAction { get; set; }
}
