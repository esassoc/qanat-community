using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class MeterGridDto
{
    public int MeterID { get; set; }

    [Required]
    [MaxLength(255)]
    public string SerialNumber { get; set; }
    [MaxLength(255)]
    public string DeviceName { get; set; }
    [MaxLength(255)]
    public string Make { get; set; }
    [MaxLength(255)]
    public string ModelNumber { get; set; }

    [Required]
    public int GeographyID { get; set; }

    [Required]
    public int MeterStatusID { get; set; }
    public MeterStatusSimpleDto MeterStatus { get; set; }

    public int? WellID { get; set; }
}