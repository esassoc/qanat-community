using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class AllocationPlanExtensionMethods
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