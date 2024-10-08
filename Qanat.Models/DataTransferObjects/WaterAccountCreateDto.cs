using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Qanat.Models.DataTransferObjects;

public class WaterAccountCreateDto
{
    [Required]
    [MinLength(10)]
    [MaxLength(255)]
    [DefaultValue("")]
    public string WaterAccountName { get; set; }

    [Required]
    [MaxLength(255)]
    [DefaultValue("")]
    public string ContactName { get; set; }

    [Required]
    [MaxLength(500)]
    [DefaultValue("")]
    public string ContactAddress { get; set; }
}