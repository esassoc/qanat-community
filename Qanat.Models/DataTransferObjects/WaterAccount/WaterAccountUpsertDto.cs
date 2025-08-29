using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class WaterAccountUpsertDto
{
    [MaxLength(255)]
    [DefaultValue("")]
    public string WaterAccountName { get; set; }
    
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

    public string Notes { get; set; }

    public bool? PrefersPhysicalCommunication { get; set; } 
}