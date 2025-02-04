namespace Qanat.Models.DataTransferObjects;

public class ParcelOwnershipUpdateDto
{
    public int WaterAccountID { get; set; }
    public int ParcelID { get; set; }
    public bool ToBeInactivated { get; set; }
    public int EffectiveYear { get; set; }
}