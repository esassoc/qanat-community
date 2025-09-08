namespace Qanat.Models.DataTransferObjects;

public class WaterAccountIndexGridDto {
    public int WaterAccountID { get; set; }
    public int WaterAccountNumber { get; set; }
    public string WaterAccountName { get; set; }
    public string ContactName { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhoneNumber { get; set; }
    public string FullAddress { get; set; }
    public string Address { get; set; }
    public string SecondaryAddress { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string Notes { get; set; }
    public bool PrefersPhysicalCommunication { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string WaterAccountPIN { get; set; }
    public DateTime? WaterAccountPINLastUsed { get; set; }
    public List<ParcelDisplayDto> Parcels { get; set; }
    public WaterAccountContactDto WaterAccountContact { get; set; }
    public List<WaterAccountUserMinimalDto> Users { get; set; }
}