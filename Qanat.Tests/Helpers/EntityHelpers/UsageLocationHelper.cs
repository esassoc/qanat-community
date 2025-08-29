using System;
using System.Linq;
using Qanat.EFModels.Entities;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Qanat.Tests.Helpers.EntityHelpers;

public static class UsageLocationHelper
{
    public static async Task<UsageLocation> AddUsageLocationAsync(int geographyID, int parcelID, int reportingPeriodID, double area)
    {
        var defaultUsageLocationType = await AssemblySteps.QanatDbContext.UsageLocationTypes.AsNoTracking()
            .SingleAsync(x => x.GeographyID == geographyID && x.IsDefault);

        var addUsageLocationResult = await AssemblySteps.QanatDbContext.UsageLocations.AddAsync(new UsageLocation()
        {
            GeographyID = geographyID,
            ParcelID = parcelID,
            ReportingPeriodID = reportingPeriodID,
            UsageLocationTypeID = defaultUsageLocationType.UsageLocationTypeID,
            Name = Guid.NewGuid().ToString(),
            Area = area,
            CreateDate = DateTime.UtcNow,
            CreateUserID = Users.QanatSystemAdminUserID
        });

        await AssemblySteps.QanatDbContext.SaveChangesAsync();

        var usageLocation = addUsageLocationResult.Entity;
        await AssemblySteps.QanatDbContext.Entry(usageLocation).ReloadAsync();
        return usageLocation;
    }
}