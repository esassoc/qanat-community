using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class CreateWaterAccountFromSuggestionDto
{
    [Required]
    public List<int> ParcelIDList { get; set; }
    [Required]
    public int ReportingPeriodID { get; set; }
    [Required]
    public string WaterAccountName { get; set; }

    public string ContactName { get; set; }
    public string ContactAddress { get; set; }
}