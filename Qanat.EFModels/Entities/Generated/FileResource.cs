using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("FileResource")]
[Index("FileResourceGUID", Name = "AK_FileResource_FileResourceGUID", IsUnique = true)]
public partial class FileResource
{
    [Key]
    public int FileResourceID { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string OriginalBaseFilename { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string OriginalFileExtension { get; set; }

    public Guid FileResourceGUID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string FileResourceCanonicalName { get; set; }

    public int CreateUserID { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [ForeignKey("CreateUserID")]
    [InverseProperty("FileResources")]
    public virtual User CreateUser { get; set; }

    [InverseProperty("RasterFileResource")]
    public virtual ICollection<OpenETSyncHistory> OpenETSyncHistories { get; set; } = new List<OpenETSyncHistory>();

    [InverseProperty("FileResource")]
    public virtual ICollection<ScenarioRunFileResource> ScenarioRunFileResources { get; set; } = new List<ScenarioRunFileResource>();

    [InverseProperty("FileResource")]
    public virtual ICollection<ScenarioRunOutputFile> ScenarioRunOutputFiles { get; set; } = new List<ScenarioRunOutputFile>();

    [InverseProperty("FileResource")]
    public virtual ICollection<StatementBatchWaterAccount> StatementBatchWaterAccounts { get; set; } = new List<StatementBatchWaterAccount>();

    [InverseProperty("FileResource")]
    public virtual ICollection<WaterMeasurementSelfReportFileResource> WaterMeasurementSelfReportFileResources { get; set; } = new List<WaterMeasurementSelfReportFileResource>();

    [InverseProperty("FileResource")]
    public virtual ICollection<WellFileResource> WellFileResources { get; set; } = new List<WellFileResource>();

    [InverseProperty("FileResource")]
    public virtual ICollection<WellRegistrationFileResource> WellRegistrationFileResources { get; set; } = new List<WellRegistrationFileResource>();
}
