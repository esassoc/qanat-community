namespace Qanat.Models.DataTransferObjects;

public class ParcelPopupDto
{
    public int ParcelID { get; set; }
    public string GeographyDisplayName { get; set; }
    public string GeographyName { get; set; }
    public string AllocationZoneName { get; set; }
    public string AllocationZoneGroupName { get; set; }
    public string AllocationZoneColor { get; set; }
    public string ParcelNumber { get; set; }
    public double ParcelArea { get; set; }
    public string WaterAccountName { get; set; }
    public int? WaterAccountNumber { get; set; }
    public int? WaterAccountID { get; set; }
}