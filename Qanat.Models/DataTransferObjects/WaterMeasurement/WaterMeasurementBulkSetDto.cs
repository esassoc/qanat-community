using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class WaterMeasurementBulkSetDto
{
    [Required]
    public int? WaterMeasurementTypeID { get; set; }

    [Required]
    public int? Year { get; set; }

    [Required]
    public int? Month { get; set; }

    [Required]
    public decimal ValueInAcreFeetPerAcre { get; set; }

    [MaxLength(500)]
    public string Comment { get; set; }
}