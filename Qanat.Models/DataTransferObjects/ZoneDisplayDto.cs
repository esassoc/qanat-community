namespace Qanat.Models.DataTransferObjects;

public class ZoneDisplayDto
{
    public int ZoneID { get; set; }
    public int ZoneGroupID { get; set; }
    public string ZoneName { get; set; }
    public string ZoneColor { get; set; }
    public string ZoneAccentColor { get; set; }
    public bool ZoneGroupDisplayToAccountHolders { get; set; }
}