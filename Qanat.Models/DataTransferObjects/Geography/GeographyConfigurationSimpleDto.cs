namespace Qanat.Models.DataTransferObjects
{
    public class GeographyConfigurationSimpleDto
    {
        public int GeographyConfigurationID { get; set; }
        public bool WellRegistryEnabled { get; set; }
        public bool LandingPageEnabled { get; set; }
        public bool MetersEnabled { get; set; }
        public bool ZonePrecipMultipliersEnabled { get; set; }
    }
}