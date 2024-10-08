using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WellUploadGDBColumn")]
[Index("CountyPermitNumber", Name = "AK_WellUploadGDBColumn_CountyPermitNumber", IsUnique = true)]
[Index("DateDrilled", Name = "AK_WellUploadGDBColumn_DateDrilled", IsUnique = true)]
[Index("Latitude", Name = "AK_WellUploadGDBColumn_Latitude", IsUnique = true)]
[Index("Longitude", Name = "AK_WellUploadGDBColumn_Longitude", IsUnique = true)]
[Index("WellName", Name = "AK_WellUploadGDBColumn_OwnerZipCode", IsUnique = true)]
[Index("StateWCRNumber", Name = "AK_WellUploadGDBColumn_StateWCRNUmber", IsUnique = true)]
[Index("WellDepth", Name = "AK_WellUploadGDBColumn_WellDepth", IsUnique = true)]
public partial class WellUploadGDBColumn
{
    [Key]
    public int WellUploadGDBColumnID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string CountyPermitNumber { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string DateDrilled { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string WellDepth { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string Latitude { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string Longitude { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string StateWCRNumber { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string WellName { get; set; }
}
