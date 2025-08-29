using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class ParcelUpdateStatusDto
{
    [Required]
    public List<int> ParcelIDs { get; set; }
    [Required]
    public int ParcelStatusID { get; set; }
}