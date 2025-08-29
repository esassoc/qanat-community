using Microsoft.EntityFrameworkCore;

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
}