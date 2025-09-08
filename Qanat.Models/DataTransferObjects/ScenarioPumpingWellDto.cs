using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class ScenarioPumpingWellDto
{
    [Required]
    public string PumpingWellName { get; set; }

    [Required]
    [DisplayName("Estimated Extraction")]
    [Range(0,1000)]
    public double? EstimatedExtraction { get; set; }

    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }
}