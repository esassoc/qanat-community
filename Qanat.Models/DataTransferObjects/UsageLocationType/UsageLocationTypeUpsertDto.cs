using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class UsageLocationTypeUpsertDto
{
    public int? UsageLocationTypeID { get; set; }

    [Required]
    public string Name { get; set; }
    public string Definition { get; set; }
    public bool CanBeRemoteSensed { get; set; }
    public bool IsIncludedInUsageCalculation { get; set; }
    public bool IsDefault { get; set; }
    public string ColorHex { get; set; }

    [Required]
    public int SortOrder { get; set; }
}