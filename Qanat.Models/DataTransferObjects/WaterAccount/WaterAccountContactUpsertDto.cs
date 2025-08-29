using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class WaterAccountContactUpsertDto
{
    [Required]
    [MaxLength(255)]
    [DefaultValue("")]
    public string ContactName { get; set; }

    [MaxLength(100)]
    public string ContactEmail { get; set; }

    [MaxLength(30)]
    public string ContactPhoneNumber { get; set; }

    [Required]
    [MaxLength(500)]
    [DefaultValue("")]
    public string Address { get; set; }

    [MaxLength(100)]
    public string SecondaryAddress { get; set; }

    [Required]
    [MaxLength(100)]
    public string City { get; set; }

    [Required]
    [MaxLength(20)]
    public string State { get; set; }

    [Required]
    [MaxLength(20)]
    public string ZipCode { get; set; }

    public bool? PrefersPhysicalCommunication { get; set; }

    public bool AddressValidated { get; set; }

    public string AddressValidationJson { get; set; }

    // used when a contact is created from the Water Account detail page and should be automatically linked to the Water Account
    public int? WaterAccountID { get; set; }
}