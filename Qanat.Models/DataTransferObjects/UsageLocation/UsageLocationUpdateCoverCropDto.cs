using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class UsageLocationUpdateCoverCropDto
{
    [Required]
    public int UsageLocationTypeID { get; set; }

    public string CoverCropNote { get; set; }
}