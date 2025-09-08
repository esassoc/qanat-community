namespace Qanat.Models.DataTransferObjects;

public class OnboardWaterAccountPINDto
{
    public string WaterAccountPIN { get; set; }
    public int WaterAccountNumber { get; set; }
    public List<ParcelMinimalDto> Parcels { get; set; }
}