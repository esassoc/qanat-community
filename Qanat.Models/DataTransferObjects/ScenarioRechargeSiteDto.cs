using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class ScenarioRechargeSiteDto
{
    [Required]
    public string RechargeSiteName { get; set; }

    [Required]
    [DisplayName("Estimated Extraction")]
    [Range(0, 1000)]
    public double? EstimatedVolume { get; set; }

    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }
}