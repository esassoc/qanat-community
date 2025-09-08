using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class UsageLocationUpdateFallowingDto
{
    [Required]
    public int UsageLocationTypeID { get; set; }

    public string FallowingNote { get; set; }
}