using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class UpdateWaterAccountParcelsDto
{
    public List<int> ParcelIDs { get; set; }
    [Required]
    public int EffectiveYear { get; set; }
}