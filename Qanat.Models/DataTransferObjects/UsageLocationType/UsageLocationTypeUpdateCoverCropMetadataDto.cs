using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class UsageLocationTypeUpdateCoverCropMetadataDto
{
    [Required]
    public bool CanBeSelectedInCoverCropForm { get; set; }

    [Required] 
    public bool CountsAsCoverCropped { get; set; }
}