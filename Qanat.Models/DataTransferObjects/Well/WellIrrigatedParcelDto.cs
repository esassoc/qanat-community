using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class WellRegistrationIrrigatedParcelsResponseDto
{
    [Required]
    public int WellRegistrationID { get; set; }

    [Required]
    [Display(Name = "Longitude")]
    public double? Longitude { get; set; }

    [Required]
    [Display(Name = "Latitude")]
    public double? Latitude { get; set; }

    [Required]
    public int GeographyID { get; set; }

    public List<ParcelDisplayDto> IrrigatedParcels { get; set; }
}