namespace Qanat.Models.DataTransferObjects;

public class AllocationPlanMinimalDto
{
    public int AllocationPlanID { get; set; }
    public int GeographyID { get; set; }
    public int GeographyAllocationPlanID { get; set; }
    public int ZoneID { get; set; }
    public int WaterTypeID { get; set; }
    public DateTime LastUpdated { get; set; }
    public string WaterTypeName { get; set; }
    public string WaterTypeSlug { get; set; }
    public string ZoneName { get; set; }
    public string ZoneSlug { get; set; }
    public string ZoneColor { get; set; }
    public string ZoneGroupName { get; set; }
    public string ZoneGroupSlug { get; set; }
    public int AllocationPeriodsCount { get; set; }
}