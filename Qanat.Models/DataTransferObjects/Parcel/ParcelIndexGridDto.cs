namespace Qanat.Models.DataTransferObjects;

public class ParcelIndexGridDto
{
    public int GeographyID { get; set; }
    public int ParcelID { get; set; }
    public string ParcelNumber { get; set; }
    public double ParcelArea { get; set; }
    public int ParcelStatusID { get; set; }
    public string ParcelStatusDisplayName { get; set; }
    public string OwnerName { get; set; }
    public string OwnerAddress { get; set; }
    public int? WaterAccountID { get; set; }
    public int? WaterAccountNumber { get; set; }
    public string WaterAccountName { get; set; }
    public List<WellLinkDisplayDto> WellsOnParcel { get; set; }
    public List<WellLinkDisplayDto> IrrigatedByWells { get; set; }
    public string ZoneIDs { get; set; }
    public Dictionary<string, string> CustomAttributes { get; set; }
}