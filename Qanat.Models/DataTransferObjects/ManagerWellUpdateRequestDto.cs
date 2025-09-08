using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class ManagerWellUpdateRequestDto
{
    [Required]
    public int WellID { get; set; }

    public string StateWCRNumber { get; set; }

    public string CountyWellPermitNumber { get; set; }

    public DateOnly? DateDrilled { get; set; }

    public int? WellDepth { get; set; }

    public string Notes { get; set; }

    [Required]
    [Display(Name = "Well Status")]
    public int WellStatusID { get; set; }
}