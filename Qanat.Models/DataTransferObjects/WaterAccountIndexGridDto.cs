namespace Qanat.Models.DataTransferObjects;

public class WaterAccountIndexGridDto {
    public int WaterAccountID { get; set; }
    public int WaterAccountNumber { get; set; }
    public string WaterAccountName { get; set; }
    public string ContactName { get; set; }
    public string ContactAddress { get; set; }
    public string Notes { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string WaterAccountPIN { get; set; }
    public DateTime? WaterAccountPINLastUsed { get; set; }
    public List<ParcelDisplayDto> Parcels { get; set; }
    public List<WaterAccountUserMinimalDto> Users { get; set; }
}