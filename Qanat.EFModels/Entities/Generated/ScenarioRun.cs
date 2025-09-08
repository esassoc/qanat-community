using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("ScenarioRun")]
public partial class ScenarioRun
{
    [Key]
    public int ScenarioRunID { get; set; }

    public int ScenarioRunStatusID { get; set; }

    public int ModelID { get; set; }

    public int ScenarioID { get; set; }

    public int UserID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastUpdateDate { get; set; }

    public int? GETRunID { get; set; }

    [StringLength(1000)]
    [Unicode(false)]
    public string GETErrorMessage { get; set; }

    [StringLength(500)]
    [Unicode(false)]
    public string ActionName { get; set; }

    [InverseProperty("ScenarioRun")]
    public virtual ICollection<ScenarioRunFileResource> ScenarioRunFileResources { get; set; } = new List<ScenarioRunFileResource>();

    [InverseProperty("ScenarioRun")]
    public virtual ICollection<ScenarioRunOutputFile> ScenarioRunOutputFiles { get; set; } = new List<ScenarioRunOutputFile>();

    [ForeignKey("UserID")]
    [InverseProperty("ScenarioRuns")]
    public virtual User User { get; set; }
}
