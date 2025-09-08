using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class WaterAccountParcelsUpdateDto
{
    [Required]
    public int ReportingPeriodID { get; set; }

    [Required]
    public List<int> ParcelIDs { get; set; } = new();
}