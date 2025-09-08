using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class WellRegistrationLocationDto
{
    [Required]
    public int WellRegistrationID { get; set; }

    [Required]
    [Display(Name = "Longitude")]
    public double? Longitude { get; set; }

    [Required]
    [Display(Name = "Latitude")]
    public double? Latitude { get; set; }

    public int? ParcelID { get; set; }
    public int? ReferenceWellID { get; set; }
    public string ParcelGeoJson { get; set; }
    public string ParcelNumber { get; set; }
    public BoundingBoxDto BoundingBox { get; set; }
}