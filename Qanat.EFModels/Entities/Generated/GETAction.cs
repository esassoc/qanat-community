using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("GETAction")]
public partial class GETAction
{
    [Key]
    public int GETActionID { get; set; }

    public int GETActionStatusID { get; set; }

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

    [InverseProperty("GETAction")]
    public virtual ICollection<GETActionFileResource> GETActionFileResources { get; set; } = new List<GETActionFileResource>();

    [InverseProperty("GETAction")]
    public virtual ICollection<GETActionOutputFile> GETActionOutputFiles { get; set; } = new List<GETActionOutputFile>();

    [ForeignKey("UserID")]
    [InverseProperty("GETActions")]
    public virtual User User { get; set; }
}
