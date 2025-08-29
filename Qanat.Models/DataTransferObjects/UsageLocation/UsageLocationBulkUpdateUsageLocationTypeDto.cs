using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class UsageLocationBulkUpdateUsageLocationTypeDto
{
    [Required]
    public int UsageLocationTypeID { get; set; }

    [Required]
    public List<int> UsageLocationIDs { get; set; }

    [Required]
    public string Note { get; set; }
}