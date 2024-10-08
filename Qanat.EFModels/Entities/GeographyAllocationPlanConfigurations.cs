using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public class GeographyAllocationPlanConfigurations
{
    private static IQueryable<GeographyAllocationPlanConfiguration> GetImpl(QanatDbContext dbContext)
    {
        return dbContext.GeographyAllocationPlanConfigurations
            .Include(x => x.AllocationPlans).ThenInclude(x => x.WaterType)
            .Include(x => x.AllocationPlans).ThenInclude(x => x.Zone)
            .Include(x => x.AllocationPlans).ThenInclude(x => x.AllocationPlanPeriods)
            .AsNoTracking();
    }

    public static GeographyAllocationPlanConfiguration GetByGeographyID(QanatDbContext dbContext, int geographyID)
    {
        return GetImpl(dbContext).SingleOrDefault(x => x.GeographyID == geographyID);
    }

    public static GeographyAllocationPlanConfigurationDto GetByGeographyIDAsConfigurationDto(QanatDbContext dbContext, int geographyID)
    {
        return GetByGeographyID(dbContext, geographyID)?.AsConfigurationDto();
    }

    public static GeographyAllocationPlanConfiguration GetByID(QanatDbContext dbContext, int geographyAllocationPlanConfigurationID)
    {
        return GetImpl(dbContext).SingleOrDefault(x => x.GeographyAllocationPlanConfigurationID == geographyAllocationPlanConfigurationID);
    }

    public static GeographyAllocationPlanConfigurationDto GetByIDAsConfigurationDto(QanatDbContext dbContext, int geographyAllocationPlanConfigurationID)
    {
        return GetByID(dbContext, geographyAllocationPlanConfigurationID)?.AsConfigurationDto();
    }

    public static GeographyAllocationPlanConfigurationDto Create(QanatDbContext dbContext, GeographyAllocationPlanConfigurationDto geographyAllocationPlanConfigurationDto)
    {
        var geographyAllocationPlan = new GeographyAllocationPlanConfiguration()
        {
            GeographyID = geographyAllocationPlanConfigurationDto.GeographyID.Value,
            ZoneGroupID = geographyAllocationPlanConfigurationDto.ZoneGroupID.Value,
            StartYear = geographyAllocationPlanConfigurationDto.StartYear.Value,
            EndYear = geographyAllocationPlanConfigurationDto.EndYear.Value,
            IsActive = geographyAllocationPlanConfigurationDto.IsActive.Value,
            IsVisibleToLandowners = geographyAllocationPlanConfigurationDto.IsVisibleToLandowners.Value,
            IsVisibleToPublic = geographyAllocationPlanConfigurationDto.IsVisibleToPublic.Value,
            AllocationPlansDescription = geographyAllocationPlanConfigurationDto.AllocationPlansDescription
        };

        dbContext.GeographyAllocationPlanConfigurations.Add(geographyAllocationPlan);
        dbContext.SaveChanges();
        dbContext.Entry(geographyAllocationPlan).Reload();

        AllocationPlans.Create(dbContext, geographyAllocationPlan.GeographyAllocationPlanConfigurationID, geographyAllocationPlanConfigurationDto);

        return GetByIDAsConfigurationDto(dbContext, geographyAllocationPlan.GeographyAllocationPlanConfigurationID);
    }

    public static void Update(QanatDbContext dbContext, int geographyID, GeographyAllocationPlanConfigurationDto geographyAllocationPlanConfigurationDto)
    {
        var geographyAllocationPlan = dbContext.GeographyAllocationPlanConfigurations.SingleOrDefault(x => x.GeographyID == geographyID);

        geographyAllocationPlan.ZoneGroupID = geographyAllocationPlanConfigurationDto.ZoneGroupID.Value;
        geographyAllocationPlan.StartYear = geographyAllocationPlanConfigurationDto.StartYear.Value;
        geographyAllocationPlan.EndYear = geographyAllocationPlanConfigurationDto.EndYear.Value;
        geographyAllocationPlan.IsActive = geographyAllocationPlanConfigurationDto.IsActive.Value;
        geographyAllocationPlan.IsVisibleToLandowners = geographyAllocationPlanConfigurationDto.IsVisibleToLandowners.Value;
        geographyAllocationPlan.IsVisibleToPublic = geographyAllocationPlanConfigurationDto.IsVisibleToPublic.Value;
        geographyAllocationPlan.AllocationPlansDescription = geographyAllocationPlanConfigurationDto.AllocationPlansDescription;

        dbContext.SaveChanges();
    }

    public static List<AllocationPlanPreviewChangesDto> PreviewConfigurationUpdates(QanatDbContext dbContext, int geographyID, GeographyAllocationPlanConfigurationDto geographyAllocationPlanConfigurationDto)
    {
        var geographyAllocationPlanConfiguration = GetByGeographyID(dbContext, geographyID);

        var allocationPlanPreviewChangesDtos = new List<AllocationPlanPreviewChangesDto>();
        foreach (var allocationPlan in geographyAllocationPlanConfiguration.AllocationPlans)
        {
            var toDelete = allocationPlan.Zone.ZoneGroupID != geographyAllocationPlanConfigurationDto.ZoneGroupID ||
                           !geographyAllocationPlanConfigurationDto.WaterTypeIDs.Contains(allocationPlan.WaterTypeID);

            List<AllocationPlanPeriod> allocationPeriodsToDelete;
            if (toDelete)
            {
                allocationPeriodsToDelete = allocationPlan.AllocationPlanPeriods.ToList();
            }
            else
            {
                allocationPeriodsToDelete = allocationPlan.AllocationPlanPeriods
                    .Where(x => !AllocationPlanPeriods.AllocationPlanPeriodWithinRange(x, geographyAllocationPlanConfigurationDto.StartYear.Value, geographyAllocationPlanConfigurationDto.EndYear.Value)).ToList();
            }

            if (!toDelete && !allocationPeriodsToDelete.Any()) continue;

            allocationPlanPreviewChangesDtos.Add(new AllocationPlanPreviewChangesDto()
            {
                AllocationPlanDisplayName = $"{allocationPlan.WaterType.WaterTypeName} / {allocationPlan.Zone.ZoneName}",
                ToDelete = toDelete,
                TotalPeriodsCount = allocationPlan.AllocationPlanPeriods.Count,
                PeriodsToDeleteCount = allocationPeriodsToDelete.Count

            });
        }

        return allocationPlanPreviewChangesDtos;
    }
}