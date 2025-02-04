using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class FeeCalculatorInputDto
{
    [Required]
    public int WaterAccountID { get; set; }

    [Required]
    public int ReportingYear { get; set; }

    [Required]
    public int FeeStructureID { get; set; }

    public decimal? SurfaceWaterDelivered { get; set; }
    public decimal? SurfaceWaterIrrigationEfficiency { get; set; }

    public List<MLRPIncentiveDto> MLRPIncentives { get; set; } = [];
}