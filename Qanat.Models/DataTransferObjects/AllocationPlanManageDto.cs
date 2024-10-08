namespace Qanat.Models.DataTransferObjects;

public class AllocationPlanManageDto
{
    public int AllocationPlanID { get; set; }
    public GeographyAllocationPlanConfigurationSimpleDto GeographyAllocationPlanConfiguration { get; set; }
    public ZoneMinimalDto Zone { get; set; }
    public WaterTypeSimpleDto WaterType { get; set; }
    public List<AllocationPlanPeriodSimpleDto> AllocationPlanPeriods { get; set; }
    public DateTime LastUpdated { get; set; }
}