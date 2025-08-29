namespace Qanat.Models.DataTransferObjects;

public class WaterAccountSearchResultDto
{
    public int WaterAccountID { get; set; }
    public int WaterAccountNumber { get; set; }
    public string WaterAccountName { get; set; }
    public string Notes { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string WaterAccountPIN { get; set; }
    public DateTime CreateDate { get; set; }
    public string ContactName { get; set; }
    public string FullAddress { get; set; }
    public List<WaterAccountUserMinimalDto> Users { get; set; }
    public List<ParcelDisplayDto> Parcels { get; set; }
    public string WaterAccountNameAndNumber { get; set; }
}