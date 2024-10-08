namespace Qanat.Models.DataTransferObjects;

public class ParcelLayerUpdateDto
{
    public string ParcelLayerNameInGDB { get; set; }
    public int UploadedGDBID { get; set; }
    public int EffectiveYear { get; set; }
    public string ParcelNumberColumn { get; set; }
    public string OwnerNameColumn { get; set; }
    public string OwnerAddressColumn { get; set; }
}