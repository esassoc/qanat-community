//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AllocationPlan]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class AllocationPlanExtensionMethods
    {
        public static AllocationPlanSimpleDto AsSimpleDto(this AllocationPlan allocationPlan)
        {
            var dto = new AllocationPlanSimpleDto()
            {
                AllocationPlanID = allocationPlan.AllocationPlanID,
                GeographyID = allocationPlan.GeographyID,
                GeographyAllocationPlanConfigurationID = allocationPlan.GeographyAllocationPlanConfigurationID,
                ZoneID = allocationPlan.ZoneID,
                WaterTypeID = allocationPlan.WaterTypeID,
                LastUpdated = allocationPlan.LastUpdated
            };
            return dto;
        }
    }
}