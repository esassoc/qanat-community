using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class GeographyAllocationPlanConfigurationExtensionMethods
    {
        public static GeographyAllocationPlanConfigurationSimpleDto AsSimpleDto(this GeographyAllocationPlanConfiguration geographyAllocationPlanConfiguration)
        {
            var dto = new GeographyAllocationPlanConfigurationSimpleDto()
            {
                GeographyAllocationPlanConfigurationID = geographyAllocationPlanConfiguration.GeographyAllocationPlanConfigurationID,
                GeographyID = geographyAllocationPlanConfiguration.GeographyID,
                ZoneGroupID = geographyAllocationPlanConfiguration.ZoneGroupID,
                StartYear = geographyAllocationPlanConfiguration.StartYear,
                EndYear = geographyAllocationPlanConfiguration.EndYear,
                IsActive = geographyAllocationPlanConfiguration.IsActive,
                IsVisibleToLandowners = geographyAllocationPlanConfiguration.IsVisibleToLandowners,
                IsVisibleToPublic = geographyAllocationPlanConfiguration.IsVisibleToPublic,
                AllocationPlansDescription = geographyAllocationPlanConfiguration.AllocationPlansDescription
            };
            return dto;
        }
    }
}