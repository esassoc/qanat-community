using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class UsageLocationTypeUpdateFallowMetadataDto
{

    [Required]
    public bool CanBeSelectedInFallowForm { get; set; }

    [Required]
    public bool CountsAsFallowed { get; set; }
}