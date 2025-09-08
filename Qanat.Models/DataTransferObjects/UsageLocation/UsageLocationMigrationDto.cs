using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class UsageLocationMigrationDto
{
    [Required, MinLength(1)]
    public List<int> UsageLocationIDs { get; set; }
}