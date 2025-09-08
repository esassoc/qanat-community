using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public class AllocationPlanPeriods
{
    public static AllocationPlanPeriod CreateNewForAllocationPlan(QanatDbContext dbContext, int allocationPlanID,
        AllocationPlanPeriodUpsertDto upsertDto)
    {
        var newAllocationPlanPeriod = new AllocationPlanPeriod()
        {
            AllocationPlanID = allocationPlanID,
            AllocationPeriodName = upsertDto.AllocationPeriodName,
            AllocationAcreFeetPerAcre = upsertDto.AllocationAcreFeetPerAcre,
            NumberOfYears = upsertDto.NumberOfYears,
            StartYear = upsertDto.StartYear,
            EnableCarryOver = upsertDto.EnableCarryOver,
            CarryOverNumberOfYears = upsertDto.CarryOverNumberOfYears,
            CarryOverDepreciationRate = upsertDto.CarryOverDepreciationRate,
            EnableBorrowForward = upsertDto.EnableBorrowForward,
            BorrowForwardNumberOfYears = upsertDto.BorrowForwardNumberOfYears,
        };
        dbContext.AllocationPlanPeriods.Add(newAllocationPlanPeriod);
        dbContext.SaveChanges();
        dbContext.Entry(newAllocationPlanPeriod).Reload();
        return newAllocationPlanPeriod;
    }

    public static void DeleteAllocationPlanPeriod(QanatDbContext dbContext, int allocationPlanPeriodID)
    {
        var allocationPlanPeriod = dbContext.AllocationPlanPeriods.Single(x => x.AllocationPlanPeriodID == allocationPlanPeriodID);
        dbContext.AllocationPlanPeriods.Remove(allocationPlanPeriod);
        dbContext.SaveChanges();
    }

    public static void UpdateAllocationPlanPeriod(QanatDbContext dbContext, int allocationPlanPeriodID, AllocationPlanPeriodUpsertDto upsertDto)
    {
        var allocationPlanPeriod = dbContext.AllocationPlanPeriods.Single(x => x.AllocationPlanPeriodID == allocationPlanPeriodID);
        allocationPlanPeriod.AllocationAcreFeetPerAcre = upsertDto.AllocationAcreFeetPerAcre;

        allocationPlanPeriod.AllocationPeriodName = upsertDto.AllocationPeriodName;
        allocationPlanPeriod.AllocationAcreFeetPerAcre = upsertDto.AllocationAcreFeetPerAcre;
        allocationPlanPeriod.NumberOfYears = upsertDto.NumberOfYears;
        allocationPlanPeriod.StartYear = upsertDto.StartYear;
        allocationPlanPeriod.EnableCarryOver = upsertDto.EnableCarryOver;
        allocationPlanPeriod.CarryOverNumberOfYears = upsertDto.CarryOverNumberOfYears;
        allocationPlanPeriod.CarryOverDepreciationRate = upsertDto.CarryOverDepreciationRate;
        allocationPlanPeriod.EnableBorrowForward = upsertDto.EnableBorrowForward;
        allocationPlanPeriod.BorrowForwardNumberOfYears = upsertDto.BorrowForwardNumberOfYears;
        dbContext.SaveChanges();
    }

    public static bool AllocationPlanPeriodWithinRange(AllocationPlanPeriod allocationPlanPeriod, int startYear, int endYear)
    {
        return allocationPlanPeriod.StartYear >= startYear && allocationPlanPeriod.StartYear <= endYear;
    }

    public static void DeleteConflictingAllocationPlanPeriods(QanatDbContext dbContext, GeographyAllocationPlanConfigurationDto geographyAllocationPlanConfigurationDto)
    {
        var allocationPlans = dbContext.AllocationPlans
            .Include(x => x.AllocationPlanPeriods)
            .Include(x => x.Zone)
            .Where(x => x.GeographyAllocationPlanConfigurationID == geographyAllocationPlanConfigurationDto.GeographyAllocationPlanConfigurationID);

        var allocationPlanPeriodsToDelete = new List<AllocationPlanPeriod>();
        foreach (var allocationPlan in allocationPlans)
        {
            if (!allocationPlan.AllocationPlanPeriods.Any()) continue;

            // get all periods from any AllocationPlans to be deleted
            if (allocationPlan.Zone.ZoneGroupID != geographyAllocationPlanConfigurationDto.ZoneGroupID || !geographyAllocationPlanConfigurationDto.WaterTypeIDs.Contains(allocationPlan.WaterTypeID))
            {
                allocationPlanPeriodsToDelete.AddRange(allocationPlan.AllocationPlanPeriods.ToList());
                continue;
            }

            // get any additional periods that conflict with time range
            var conflictingAllocationPlanPeriods = allocationPlan.AllocationPlanPeriods
                .Where(x => !AllocationPlanPeriodWithinRange(x, geographyAllocationPlanConfigurationDto.StartYear.Value, geographyAllocationPlanConfigurationDto.EndYear.Value)).ToList();

            allocationPlanPeriodsToDelete.AddRange(conflictingAllocationPlanPeriods);
        }

        dbContext.AllocationPlanPeriods.RemoveRange(allocationPlanPeriodsToDelete);
        dbContext.SaveChanges();
    }
}