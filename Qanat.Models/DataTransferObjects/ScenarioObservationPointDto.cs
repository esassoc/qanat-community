using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class ScenarioObservationPointDto
{
    [Required]
    public string ObservationPointName { get; set; }

    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }
}