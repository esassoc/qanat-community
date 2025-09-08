using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class RemoveWellMeterRequestDto
{
    [Required]
    [Display(Name = "Well")]
    public int WellID { get; set; }

    [Required]
    [Display(Name = "Meter")]
    public int MeterID { get; set; }

    [Required]
    [Display(Name = "End Date")]
    public DateTime EndDate { get; set; }
}