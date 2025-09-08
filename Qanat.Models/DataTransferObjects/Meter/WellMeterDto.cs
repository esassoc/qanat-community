namespace Qanat.Models.DataTransferObjects;

public class WellMeterDto
{
    public int GeographyID { get; set; }
    public int WellMeterID { get; set; }
    public int WellID { get; set; }
    public string WellName { get; set; }
    public int MeterID { get; set; }
    public string MeterSerialNumber { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? WaterAccountNumber { get; set; }
}