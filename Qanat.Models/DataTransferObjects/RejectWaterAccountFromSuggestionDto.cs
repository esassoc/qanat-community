using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class RejectWaterAccountFromSuggestionDto
{
    [Required]
    public List<int> ParcelIDs { get; set; }
}