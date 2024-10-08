namespace Qanat.Models.DataTransferObjects;

public class WaterAccountRequestChangesDto
{
    public int WaterAccountID { get; set; }
    public string WaterAccountName { get; set; }
    public int WaterAccountNumber { get; set; }
    public string ContactName { get; set; }
    public string ContactAddress { get; set; }
    public List<WaterAccountRequestChangesParcelDto> Parcels { get; set; }
}

public class WaterAccountRequestChangesParcelDto
{
    public int ParcelID { get; set; }
    public string ParcelNumber { get; set; }
    public int WaterAccountID { get; set; }
    public string WaterAccountName { get; set; }
    public ZoneDisplayDto AllocationZone { get; set; }
}