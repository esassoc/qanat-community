using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class WaterMeasurementSelfReportCreateDto
{
    [Required]
    public int WaterMeasurementTypeID { get; set; }

    [Required]
    public int ReportingYear { get; set; }
}