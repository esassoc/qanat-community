namespace Qanat.Models.DataTransferObjects;

public class ZoneDetailedDto
{
    public int ZoneID { get; set; }
    public int? ZoneGroupID { get; set; }
    public string ZoneName { get; set; }
    public string ZoneSlug { get; set; }
    public string ZoneDescription { get; set; }
    public string ZoneColor { get; set; }
    public string ZoneAccentColor { get; set; }
    public int SortOrder { get; set; }
    public int TotalParcels { get; set; }
    public double TotalArea { get; set; }
    public string ZoneGroupName { get; set; }
}