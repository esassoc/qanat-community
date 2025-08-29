using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class WellParcelDto
{
    [Required]
    public int WellID { get; set; }

    [Required]
    public int ParcelID { get; set; }
}