namespace Qanat.Models.DataTransferObjects;

public class WaterAccountSummaryDto
{
    public int WaterAccountID { get; set; }
    public int WaterAccountNumber { get; set; }
    public string GeographyName { get; set; }
    public List<ZoneDisplayDto> Zones { get; set; }
    public int NumOfParcels { get; set; }
    public double Area { get; set; }
}