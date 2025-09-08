using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("ParcelCustomAttribute")]
[Index("ParcelID", Name = "AK_ParcelCustomAttribute_Parcel", IsUnique = true)]
public partial class ParcelCustomAttribute
{
    [Key]
    public int ParcelCustomAttributeID { get; set; }

    public int ParcelID { get; set; }

    [Required]
    [Unicode(false)]
    public string CustomAttributes { get; set; }

    [ForeignKey("ParcelID")]
    [InverseProperty("ParcelCustomAttribute")]
    public virtual Parcel Parcel { get; set; }
}
