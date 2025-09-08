using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static class GeographyConfigurationExtensionMethods
    {
        public static GeographyConfigurationSimpleDto AsSimpleDto(this GeographyConfiguration geographyConfiguration)
        {
            var dto = new GeographyConfigurationSimpleDto()
            {
                GeographyConfigurationID = geographyConfiguration.GeographyConfigurationID,
                WellRegistryEnabled = geographyConfiguration.WellRegistryEnabled,
                LandingPageEnabled = geographyConfiguration.LandingPageEnabled,
                MetersEnabled = geographyConfiguration.MetersEnabled,
                ZonePrecipMultipliersEnabled = geographyConfiguration.ZonePrecipMultipliersEnabled
            };
            return dto;
        }
    }
}