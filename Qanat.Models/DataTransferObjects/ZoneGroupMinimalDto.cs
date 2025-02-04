namespace Qanat.Models.DataTransferObjects;

public class ZoneGroupMinimalDto
{
    public int ZoneGroupID { get; set; }
    public int GeographyID { get; set; }
    public string ZoneGroupName { get; set; }
    public string ZoneGroupSlug { get; set; }
    public string ZoneGroupDescription { get; set; }
    public int SortOrder { get; set; }
    public List<ZoneMinimalDto> ZoneList { get; set; }
    public bool HasAllocationPlan { get; set; }
}