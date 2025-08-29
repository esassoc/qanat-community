using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class ManagerWellUpdateRequestDto
{
    [Required]
    public int WellID { get; set; }

    public string StateWCRNumber { get; set; }

    public string CountyWellPermitNumber { get; set; }

    public DateOnly? DateDrilled { get; set; }

    public string Notes { get; set; }

    [Required]
    [Display(Name = "Well Status")]
    public int WellStatusID { get; set; }

    public int? WellDepth { get; set; }
    public int? CasingDiameter { get; set; }
    public int? TopOfPerforations { get; set; }
    public int? BottomOfPerforations { get; set; }
    public string ElectricMeterNumber { get; set; }
}