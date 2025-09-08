using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public partial class AllocationPlanExtensionMethods
{
    public static AllocationPlanManageDto AsManageDto(this AllocationPlan allocationPlan)
    {
        return new AllocationPlanManageDto()
        {
            AllocationPlanID = allocationPlan.AllocationPlanID,
            GeographyAllocationPlanConfiguration = allocationPlan.Geography.GeographyAllocationPlanConfiguration.AsSimpleDto(),
            WaterType = allocationPlan.WaterType.AsSimpleDto(),
            Zone = allocationPlan.Zone.AsZoneMinimalDto(),
            AllocationPlanPeriods = allocationPlan.AllocationPlanPeriods.Select(x => x.AsSimpleDto()).OrderBy(x => x.StartYear).ThenBy(x => x.AllocationPlanPeriodID).ToList(),
            LastUpdated = allocationPlan.LastUpdated,
        };
    }
}