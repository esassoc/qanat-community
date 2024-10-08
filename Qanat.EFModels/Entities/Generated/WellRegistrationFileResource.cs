using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WellRegistrationFileResource")]
public partial class WellRegistrationFileResource
{
    [Key]
    public int WellRegistrationFileResourceID { get; set; }

    public int WellRegistrationID { get; set; }

    public int FileResourceID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string FileDescription { get; set; }

    [ForeignKey("FileResourceID")]
    [InverseProperty("WellRegistrationFileResources")]
    public virtual FileResource FileResource { get; set; }

    [ForeignKey("WellRegistrationID")]
    [InverseProperty("WellRegistrationFileResources")]
    public virtual WellRegistration WellRegistration { get; set; }
}
