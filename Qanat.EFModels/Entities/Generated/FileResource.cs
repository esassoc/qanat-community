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

    [InverseProperty("FileResource")]
    public virtual ICollection<GETActionFileResource> GETActionFileResources { get; set; } = new List<GETActionFileResource>();

    [InverseProperty("FileResource")]
    public virtual ICollection<GETActionOutputFile> GETActionOutputFiles { get; set; } = new List<GETActionOutputFile>();

    [InverseProperty("RasterFileResource")]
    public virtual ICollection<OpenETSyncHistory> OpenETSyncHistories { get; set; } = new List<OpenETSyncHistory>();

    [InverseProperty("FileResource")]
    public virtual ICollection<WellRegistrationFileResource> WellRegistrationFileResources { get; set; } = new List<WellRegistrationFileResource>();
}
