using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class WaterAccountUpsertDto
{
    [Required]
    [MaxLength(255)]
    public string WaterAccountName { get; set; }
    
    public string Notes { get; set; }
}