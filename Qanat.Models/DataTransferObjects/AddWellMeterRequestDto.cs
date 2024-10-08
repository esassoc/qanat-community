using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class AddWellMeterRequestDto
{
    [Required]
    [Display(Name = "Well")]
    public int WellID { get; set; }

    [Required]
    [Display(Name = "Meter")]
    public int MeterID { get; set; }

    [Required]
    [Display(Name = "Start Date")]
    public DateTime StartDate { get; set; }
}