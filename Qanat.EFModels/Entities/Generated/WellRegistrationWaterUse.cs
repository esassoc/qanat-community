using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WellRegistrationWaterUse")]
[Index("WellRegistrationID", "WellRegistrationWaterUseTypeID", Name = "AK_WellRegistrationWaterUse_WellRegistrationID_WellRegistrationWaterUseTypeID", IsUnique = true)]
public partial class WellRegistrationWaterUse
{
    [Key]
    public int WellRegistrationWaterUseID { get; set; }

    public int WellRegistrationID { get; set; }

    public int WellRegistrationWaterUseTypeID { get; set; }

    [StringLength(200)]
    [Unicode(false)]
    public string WellRegistrationWaterUseDescription { get; set; }

    [ForeignKey("WellRegistrationID")]
    [InverseProperty("WellRegistrationWaterUses")]
    public virtual WellRegistration WellRegistration { get; set; }
}
