using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities;

public static class GeographyConfigurations
{
    public static async Task ToggleWellRegistry(QanatDbContext dbContext, int geographyID, bool enabled)
    {
        var geographyConfiguration = await dbContext.GeographyConfigurations
                .SingleAsync(x => x.GeographyConfigurationID == geographyID);
        geographyConfiguration.WellRegistryEnabled = enabled;
        await dbContext.SaveChangesAsync();
    }

    public static async Task ToggleLandingPage(QanatDbContext dbContext, int geographyID, bool enabled)
    {
        var geographyConfiguration = await dbContext.GeographyConfigurations
            .SingleAsync(x => x.GeographyConfigurationID == geographyID);
        geographyConfiguration.LandingPageEnabled = enabled;
        await dbContext.SaveChangesAsync();
    }

    public static async Task ToggleMeterConfiguration(QanatDbContext dbContext, int geographyID, bool enabled)
    {
        var geographyConfiguration = await dbContext.GeographyConfigurations
            .SingleAsync(x => x.GeographyConfigurationID == geographyID);
        geographyConfiguration.MetersEnabled = enabled;
        await dbContext.SaveChangesAsync();
    }

    public static async Task<List<GeographyConfigurationDto>> GetGeographyConfigurations(QanatDbContext dbContext)
    {
        var geographyConfigurations = await dbContext.Geographies.AsNoTracking()
            .Include(x => x.GeographyConfiguration)
            .Select(x => new GeographyConfigurationDto()
            {
                GeographyID = x.GeographyID,
                GeographySlug = x.GeographyName.ToLower(),
                WellRegistryEnabled = x.GeographyConfiguration.WellRegistryEnabled,
                LandingPageEnabled = x.GeographyConfiguration.LandingPageEnabled,
            }).ToListAsync();

        return geographyConfigurations;
    }
}