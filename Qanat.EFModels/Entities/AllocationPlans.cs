using Microsoft.EntityFrameworkCore;
using Qanat.Common.Util;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public class AllocationPlans
{
    private static IQueryable<AllocationPlan> GetImpl(QanatDbContext dbContext)
    {
        return dbContext.AllocationPlans
            .Include(x => x.WaterType)
            .Include(x => x.Zone).ThenInclude(x => x.ZoneGroup)
            .Include(x => x.AllocationPlanPeriods).AsNoTracking();
    }

    public static List<AllocationPlanMinimalDto> ListByGeographyIDAsMinimalDto(QanatDbContext dbContext, int geographyID)
    {
        return GetImpl(dbContext)
            .Where(x => x.GeographyID == geographyID)
            .OrderBy(x => x.WaterType.SortOrder)
            .Select(x => x.AsAllocationPlanMinimalDto())
            .ToList();
    }

    public static List<AllocationPlanMinimalDto> ListByGeographyIDAndZoneIDsAsSimpleDto(QanatDbContext dbContext, int geographyID, List<int> zoneIDs)
    {
        return GetImpl(dbContext).Where(x => x.GeographyID == geographyID && zoneIDs.Contains(x.ZoneID))
            .Select(x => x.AsAllocationPlanMinimalDto()).ToList();
    }

    private static IQueryable<AllocationPlan> GetManageImpl(QanatDbContext dbContext)
    {
        return dbContext.AllocationPlans
            .Include(x => x.Geography).ThenInclude(x => x.GeographyAllocationPlanConfiguration)
            .Include(x => x.WaterType)
            .Include(x => x.Zone)
            .ThenInclude(x => x.ZoneGroup)
            .Include(x => x.AllocationPlanPeriods).AsNoTracking();
    }

    public static AllocationPlanManageDto GetAllocationPlanManageDto(QanatDbContext dbContext, int geographyID, string waterTypeSlug,
        string zoneSlug)
    {
        var allocationPlan = GetManageImpl(dbContext)
            .SingleOrDefault(x => x.GeographyID == geographyID && x.Zone.ZoneSlug == zoneSlug && x.WaterType.WaterTypeSlug == waterTypeSlug);
        return allocationPlan.AsManageDto();
    }

    public static AllocationPlanManageDto GetAllocationPlanManageDto(QanatDbContext dbContext, int allocationPlanID)
    {
        var allocationPlan = GetManageImpl(dbContext)
            .SingleOrDefault(x => x.AllocationPlanID == allocationPlanID);
        return allocationPlan.AsManageDto();
    }

    public static List<AllocationPlanManageDto> GetAllocationPlanManageDtos(QanatDbContext dbContext, IEnumerable<int> allocationPlanIDs)
    {
        var allocationPlans = GetManageImpl(dbContext)
            .Where(x => allocationPlanIDs.Contains(x.AllocationPlanID));
        return allocationPlans.Select(x => x.AsManageDto()).ToList();
    }

    public static void Create(QanatDbContext dbContext, int geographyAllocationPlanID, GeographyAllocationPlanConfigurationDto geographyAllocationPlanConfigurationDto)
    {
        var zones = dbContext.Zones
            .Where(x => x.ZoneGroupID == geographyAllocationPlanConfigurationDto.ZoneGroupID.Value).ToList();


        foreach (var zone in zones)
        {
            var newPlans = geographyAllocationPlanConfigurationDto.WaterTypeIDs.Select(x => new AllocationPlan()
            {
                GeographyID = geographyAllocationPlanConfigurationDto.GeographyID.Value,
                GeographyAllocationPlanConfigurationID = geographyAllocationPlanID,
                WaterTypeID = x,
                ZoneID = zone.ZoneID
            });
            dbContext.AllocationPlans.AddRange(newPlans);
        }

        dbContext.SaveChanges();
    }

    public static void MergeAllocationPlans(QanatDbContext dbContext, int geographyID, GeographyAllocationPlanConfigurationDto geographyAllocationPlanConfigurationDto)
    {
        AllocationPlanPeriods.DeleteConflictingAllocationPlanPeriods(dbContext, geographyAllocationPlanConfigurationDto);

        var zones = dbContext.Zones.AsNoTracking()
            .Where(x => x.ZoneGroupID == geographyAllocationPlanConfigurationDto.ZoneGroupID.Value).ToList();

        var updatedAllocationPlans = new List<AllocationPlan>();
        zones.ForEach(zone =>
        {
            updatedAllocationPlans.AddRange(
                geographyAllocationPlanConfigurationDto.WaterTypeIDs.Select(x => new AllocationPlan()
                {
                    GeographyAllocationPlanConfigurationID = geographyAllocationPlanConfigurationDto.GeographyAllocationPlanConfigurationID.Value,
                    WaterTypeID = x,
                    ZoneID = zone.ZoneID,
                    GeographyID = geographyID
                }));
        });

        var allInDb = dbContext.AllocationPlans;
        var existingAllocationPlans = allInDb.Where(x => x.GeographyID == geographyID).ToList();

        existingAllocationPlans.Merge(updatedAllocationPlans, allInDb,
            (x, y) => x.WaterTypeID == y.WaterTypeID && x.ZoneID == y.ZoneID);

        dbContext.SaveChanges();
    }

    public static void MarkLastUpdated(QanatDbContext dbContext, int allocationPlanID)
    {
        var allocationPlan = dbContext.AllocationPlans.FirstOrDefault(x => x.AllocationPlanID == allocationPlanID);
        if (allocationPlan != null)
        {
            allocationPlan.LastUpdated = DateTime.UtcNow;
        }
        dbContext.SaveChanges();
    }

    public static void CopyFromTo(QanatDbContext dbContext, int copyFromAllocationPlanID, int copyToAllocationPlanID)
    {
        var copyFrom = dbContext.AllocationPlans
            .Include(x => x.AllocationPlanPeriods).AsNoTracking()
            .Single(x => x.AllocationPlanID == copyFromAllocationPlanID);

        var copyTo = dbContext.AllocationPlans
            .Include(x => x.AllocationPlanPeriods)
            .Single(x => x.AllocationPlanID == copyToAllocationPlanID);

        // delete any periods on the "copy to" allocation plan
        dbContext.AllocationPlanPeriods.RemoveRange(copyTo.AllocationPlanPeriods);

        var newPeriods = copyFrom.AllocationPlanPeriods.Select(x => new AllocationPlanPeriod()
        {
            AllocationPlanID = copyToAllocationPlanID,
            AllocationPeriodName = x.AllocationPeriodName,
            AllocationAcreFeetPerAcre = x.AllocationAcreFeetPerAcre,
            NumberOfYears = x.NumberOfYears,
            StartYear = x.StartYear,
            EnableCarryOver = x.EnableCarryOver,
            CarryOverNumberOfYears = x.CarryOverNumberOfYears,
            CarryOverDepreciationRate = x.CarryOverDepreciationRate,
            EnableBorrowForward = x.EnableBorrowForward,
            BorrowForwardNumberOfYears = x.BorrowForwardNumberOfYears,
        });

        dbContext.AllocationPlanPeriods.AddRange(newPeriods);
        dbContext.SaveChanges();
    }
}