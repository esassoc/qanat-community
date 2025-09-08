using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class GeographyAllocationPlanExtensionMethods
{
    public static GeographyAllocationPlanConfigurationDto AsConfigurationDto(
        this GeographyAllocationPlanConfiguration geographyAllocationPlanConfiguration)
    {
        return new GeographyAllocationPlanConfigurationDto()
        {
            GeographyAllocationPlanConfigurationID = geographyAllocationPlanConfiguration.GeographyAllocationPlanConfigurationID,
            GeographyID = geographyAllocationPlanConfiguration.GeographyID,
            ZoneGroupID = geographyAllocationPlanConfiguration.ZoneGroupID,
            StartYear = geographyAllocationPlanConfiguration.StartYear,
            EndYear = geographyAllocationPlanConfiguration.EndYear,
            IsActive = geographyAllocationPlanConfiguration.IsActive,
            IsVisibleToLandowners = geographyAllocationPlanConfiguration.IsVisibleToLandowners,
            IsVisibleToPublic = geographyAllocationPlanConfiguration.IsVisibleToPublic,
            AllocationPlansDescription = geographyAllocationPlanConfiguration.AllocationPlansDescription,
            WaterTypeIDs = geographyAllocationPlanConfiguration.AllocationPlans.Select(x => x.WaterTypeID).Distinct().ToList()
        };
    }
}