using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("ScenarioRunFileResource")]
public partial class ScenarioRunFileResource
{
    [Key]
    public int ScenarioRunFileResourceID { get; set; }

    public int ScenarioRunID { get; set; }

    public int FileResourceID { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("ScenarioRunFileResources")]
    public virtual FileResource FileResource { get; set; }

    [ForeignKey("ScenarioRunID")]
    [InverseProperty("ScenarioRunFileResources")]
    public virtual ScenarioRun ScenarioRun { get; set; }
}
