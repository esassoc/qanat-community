namespace Qanat.Models.DataTransferObjects;

public class ZoneGroupDto
{
    public int ZoneGroupID { get; set; }
    public int GeographyID { get; set; }
    public string ZoneGroupName { get; set; }
    public string ZoneGroupSlug { get; set; }
    public string ZoneGroupDescription { get; set; }
    public int SortOrder { get; set; }
    public List<ZoneDetailedDto> ZoneList { get; set; }
    public bool HasAllocationPlan { get; set; }
    public bool DisplayToAccountHolders { get; set; }
}