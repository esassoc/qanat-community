//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[GeographyConfiguration]

using Qanat.Models.DataTransferObjects;

namespace Qanat.EFModels.Entities
{
    public static partial class GeographyConfigurationExtensionMethods
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