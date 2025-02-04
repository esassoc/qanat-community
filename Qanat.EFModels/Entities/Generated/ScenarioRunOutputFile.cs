using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("ScenarioRunOutputFile")]
[Index("ScenarioRunOutputFileTypeID", "ScenarioRunID", Name = "AK_ScenarioRunOutputFile_Unique_ScenarioRunOutputFileTypeID_ScenarioRunID", IsUnique = true)]
public partial class ScenarioRunOutputFile
{
    [Key]
    public int ScenarioRunOutputFileID { get; set; }

    public int ScenarioRunOutputFileTypeID { get; set; }

    public int ScenarioRunID { get; set; }

    public int FileResourceID { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("ScenarioRunOutputFiles")]
    public virtual FileResource FileResource { get; set; }

    [ForeignKey("ScenarioRunID")]
    [InverseProperty("ScenarioRunOutputFiles")]
    public virtual ScenarioRun ScenarioRun { get; set; }
}
