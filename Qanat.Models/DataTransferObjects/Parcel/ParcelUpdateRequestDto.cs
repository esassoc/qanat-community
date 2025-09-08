using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class ParcelUpdateOwnershipRequestDto
{
    [Required]
    public int ParcelID { get; set; }

    [Required]
    public string OwnerName { get; set; }

    [Required]
    public string OwnerAddress { get; set; }
}

public class ParcelAcreUpdateDto
{
    [Required]
    public double Acres { get; set; }
}