using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class WellLocationDto
{
    [Required]
    public int WellID { get; set; }

    [Required]
    [Display(Name = "Longitude")]
    public double? Longitude { get; set; }

    [Required]
    [Display(Name = "Latitude")]
    public double? Latitude { get; set; }

    public int? ParcelID { get; set; }
    public string ParcelGeoJson { get; set; }
    public string ParcelNumber { get; set; }
    public BoundingBoxDto BoundingBox { get; set; }
    public int GeographyID { get; set; }
}

public class WellLocationPreviewDto
{
    public int WellID { get; set; }
    public int? ParcelID { get; set; }
    public string ParcelNumber { get; set; }
}