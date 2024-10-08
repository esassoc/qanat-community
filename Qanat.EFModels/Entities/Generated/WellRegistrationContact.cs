using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WellRegistrationContact")]
[Index("WellRegistrationID", "WellRegistrationContactTypeID", Name = "AK_WellRegistrationContact_WellRegistrationID_RegistrationContactTypeID", IsUnique = true)]
public partial class WellRegistrationContact
{
    [Key]
    public int WellRegistrationContactID { get; set; }

    public int WellRegistrationID { get; set; }

    public int WellRegistrationContactTypeID { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string ContactName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string BusinessName { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string StreetAddress { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string City { get; set; }

    public int StateID { get; set; }

    [Required]
    [StringLength(10)]
    [Unicode(false)]
    public string ZipCode { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string Phone { get; set; }

    [Required]
    [StringLength(100)]
    [Unicode(false)]
    public string Email { get; set; }

    [ForeignKey("WellRegistrationID")]
    [InverseProperty("WellRegistrationContacts")]
    public virtual WellRegistration WellRegistration { get; set; }
}
