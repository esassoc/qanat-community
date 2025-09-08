namespace Qanat.Models.DataTransferObjects;

public class WaterAccountParcelsRequestChangesDto
{
    public List<WaterAccountRequestChangesDto> WaterAccounts { get; set; }
    public List<WaterAccountRequestChangesParcelDto> ParcelsToRemove { get; set; }
    public int Year { get; set; }
}

public class WaterAccountRequestChangesDto
{
    public int WaterAccountID { get; set; }
    public string WaterAccountName { get; set; }
    public int WaterAccountNumber { get; set; }
    public WaterAccountContactDto WaterAccountContact { get; set; }
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