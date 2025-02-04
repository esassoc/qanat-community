namespace Qanat.Models.DataTransferObjects;

public class ParcelDetailDto
{
    public int ParcelID { get; set; }
    public int GeographyID { get; set; }
    public string GeographyName { get; set; }
    public int? WaterAccountID { get; set; }
    public string ParcelNumber { get; set; }
    public double ParcelArea { get; set; }
    public int ParcelStatusID { get; set; }
    public string OwnerAddress { get; set; }
    public string OwnerName { get; set; }
    public WaterAccountDisplayDto WaterAccount { get; set; }
    public ParcelStatusSimpleDto ParcelStatus { get; set; }
    public List<ZoneMinimalDto> Zones { get; set; }
    public List<WellSimpleDto> WellsOnParcel { get; set; }
    public List<WellSimpleDto> IrrigatedByWells { get; set; }
}