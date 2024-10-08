//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[AllocationPlanPeriod]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class AllocationPlanPeriodExtensionMethods
    {
        public static AllocationPlanPeriodSimpleDto AsSimpleDto(this AllocationPlanPeriod allocationPlanPeriod)
        {
            var dto = new AllocationPlanPeriodSimpleDto()
            {
                AllocationPlanPeriodID = allocationPlanPeriod.AllocationPlanPeriodID,
                AllocationPlanID = allocationPlanPeriod.AllocationPlanID,
                AllocationPeriodName = allocationPlanPeriod.AllocationPeriodName,
                AllocationAcreFeetPerAcre = allocationPlanPeriod.AllocationAcreFeetPerAcre,
                NumberOfYears = allocationPlanPeriod.NumberOfYears,
                StartYear = allocationPlanPeriod.StartYear,
                EnableCarryOver = allocationPlanPeriod.EnableCarryOver,
                CarryOverNumberOfYears = allocationPlanPeriod.CarryOverNumberOfYears,
                CarryOverDepreciationRate = allocationPlanPeriod.CarryOverDepreciationRate,
                EnableBorrowForward = allocationPlanPeriod.EnableBorrowForward,
                BorrowForwardNumberOfYears = allocationPlanPeriod.BorrowForwardNumberOfYears
            };
            return dto;
        }
    }
}