using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class BatchValidateWaterAccountContactAddressRequestDto
{
    [Required]
    public List<int> WaterAccountContactIDs { get; set; }
}