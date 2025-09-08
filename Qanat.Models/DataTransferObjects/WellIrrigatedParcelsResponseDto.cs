namespace Qanat.Models.DataTransferObjects;

public class WellIrrigatedParcelsResponseDto
{
    public int WellID { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public int GeographyID { get; set; }
    public List<ParcelDisplayDto> IrrigatedParcels { get; set; }
}