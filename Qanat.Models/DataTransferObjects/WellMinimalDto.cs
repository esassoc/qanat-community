namespace Qanat.Models.DataTransferObjects;

public class WellMinimalDto
{
    public int WellID { get; set; }
    public int GeographyID { get; set; }
    public string WellName { get; set; }
    public int? ParcelID { get; set; }
    public string StateWCRNumber { get; set; }
    public string CountyWellPermitNumber { get; set; }
    public DateOnly? DateDrilled { get; set; }
    public int? WellDepth { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int WellStatusID { get; set; }
    public string WellStatusDisplayName { get; set; }
    public string ParcelNumber { get; set; }
    public List<ParcelMinimalDto> IrrigatesParcels { get; set; }
    public string Notes { get; set; }
}