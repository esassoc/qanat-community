using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static partial class AllocationPlanExtensionMethods
{
    public static AllocationPlanMinimalDto AsAllocationPlanMinimalDto(this AllocationPlan allocationPlan)
    {
        var dto = new AllocationPlanMinimalDto()
        {
            AllocationPlanID = allocationPlan.AllocationPlanID,
            GeographyID = allocationPlan.GeographyID,
            GeographyAllocationPlanID = allocationPlan.GeographyAllocationPlanConfigurationID,
            ZoneID = allocationPlan.ZoneID,
            WaterTypeID = allocationPlan.WaterTypeID,
            LastUpdated = allocationPlan.LastUpdated,
            WaterTypeName = allocationPlan.WaterType.WaterTypeName,
            WaterTypeSlug = allocationPlan.WaterType.WaterTypeSlug,
            ZoneName = allocationPlan.Zone.ZoneName,
            ZoneSlug = allocationPlan.Zone.ZoneSlug,
            ZoneColor = allocationPlan.Zone.ZoneColor,
            ZoneGroupName = allocationPlan.Zone.ZoneGroup?.ZoneGroupName,
            ZoneGroupSlug = allocationPlan.Zone.ZoneGroup?.ZoneGroupSlug,
            AllocationPeriodsCount = allocationPlan.AllocationPlanPeriods.Count
        };
        return dto;
    }

}