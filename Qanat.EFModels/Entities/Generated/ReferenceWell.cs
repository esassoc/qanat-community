using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Qanat.EFModels.Entities;

[Table("ReferenceWell")]
[Index("GeographyID", "WellName", Name = "AK_ReferenceWell_GeographyID_WellName", IsUnique = true)]
public partial class ReferenceWell
{
    [Key]
    public int ReferenceWellID { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [Column(TypeName = "geometry")]
    public Geometry LocationPoint4326 { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string WellName { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string CountyWellPermitNo { get; set; }

    public int? WellDepth { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string StateWCRNumber { get; set; }

    public DateOnly? DateDrilled { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("ReferenceWells")]
    public virtual Geography Geography { get; set; }

    [InverseProperty("ReferenceWell")]
    public virtual ICollection<WellRegistration> WellRegistrations { get; set; } = new List<WellRegistration>();
}
