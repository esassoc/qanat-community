using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Qanat.EFModels.Entities;

[Table("WaterAccountContact")]
[Index("GeographyID", Name = "IX_WaterAccountContact_GeographyID")]
public partial class WaterAccountContact
{
    [Key]
    public int WaterAccountContactID { get; set; }

    public int GeographyID { get; set; }

    [Required]
    [StringLength(255)]
    [Unicode(false)]
    public string ContactName { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string ContactEmail { get; set; }

    [StringLength(30)]
    [Unicode(false)]
    public string ContactPhoneNumber { get; set; }

    [Required]
    [StringLength(500)]
    [Unicode(false)]
    public string Address { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string SecondaryAddress { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string City { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string State { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string ZipCode { get; set; }

    [Required]
    [StringLength(747)]
    [Unicode(false)]
    public string FullAddress { get; set; }

    public bool PrefersPhysicalCommunication { get; set; }

    public bool AddressValidated { get; set; }

    [Unicode(false)]
    public string AddressValidationJson { get; set; }

    [ForeignKey("GeographyID")]
    [InverseProperty("WaterAccountContacts")]
    public virtual Geography Geography { get; set; }

    [InverseProperty("WaterAccountContact")]
    public virtual ICollection<WaterAccount> WaterAccounts { get; set; } = new List<WaterAccount>();
}
