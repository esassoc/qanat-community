using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Qanat.EFModels.Entities;

[Table("ParcelStaging")]
[Index("GeographyID", Name = "IX_ParcelStaging_GeographyID")]
public partial class ParcelStaging
{
    [Key]
    public int ParcelStagingID { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [StringLength(64)]
    [Unicode(false)]
    public string ParcelNumber { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry Geometry { get; set; }

    [Required]
    [StringLength(200)]
    [Unicode(false)]
    public string OwnerName { get; set; }

    [Column(TypeName = "geometry")]
    public Geometry Geometry4326 { get; set; }

    [Required]
    [StringLength(200)]
    [Unicode(false)]
    public string OwnerAddress { get; set; }

    public double Acres { get; set; }

    public bool HasConflict { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("ParcelStagings")]
    public virtual Geography Geography { get; set; }
}
