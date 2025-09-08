using System.ComponentModel.DataAnnotations;

namespace Qanat.Models.DataTransferObjects;

public class WellIrrigatedParcelsRequestDto
{
    [Required]
    public int WellID { get; set; }

    [Required]
    public List<int> IrrigatedParcelIDs { get; set; }
}

public class WellRegistrationIrrigatedParcelsRequestDto
{
    [Required] public int WellID { get; set; }

    public List<int> IrrigatedParcelIDs { get; set; }
}