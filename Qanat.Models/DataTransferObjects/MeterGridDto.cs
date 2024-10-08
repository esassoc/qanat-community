using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class MeterGridDto
{
    public int MeterID { get; set; }

    [Required]
    public string SerialNumber { get; set; }
    public string DeviceName { get; set; }
    public string Make { get; set; }
    public string ModelNumber { get; set; }

    [Required]
    public int GeographyID { get; set; }

    [Required]
    public int MeterStatusID { get; set; }
    public MeterStatusSimpleDto MeterStatus { get; set; }

    public List<WellLinkDisplayDto> WellIDs { get; set; }
}